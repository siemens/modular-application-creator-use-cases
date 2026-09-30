using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Siemens.Automation.ModularApplicationCreator.Core;
using Siemens.Automation.ModularApplicationCreatorBasics.Logging;
using Siemens.Engineering;

namespace MAC_use_cases.Model.UseCases
{
    /// <summary>
    ///     Handles the generation of hardware configurations based on a CSV file.
    ///     This class provides functionality to read device information from a CSV file
    ///     and create corresponding hardware configurations in TIA Portal projects.
    ///     No Microsoft Excel installation is required.
    /// </summary>
    /// <remarks>
    ///     CSV format:
    ///     - The first row contains the column headers; the order of the columns does not matter.
    ///     - Required columns: OrderNumber, Version, Name, DeviceName. The column Type is optional.
    ///     - Separator ';' (default of Excel with a German locale) or ',' - detected from the header row.
    ///     - Encoding UTF-8 with or without BOM. Files that aren't valid UTF-8 are read with the
    ///       Windows ANSI code page, e.g. "CSV (semicolon/comma delimited)" saved by Excel.
    ///     - Values can be enclosed in double quotes; a double quote inside a quoted value is written as "".
    ///     - Empty rows and rows with missing required values are skipped.
    ///
    ///     Example (AdditionalContent/HardwareGeneration.csv):
    ///     \code{.unparsed}
    ///     OrderNumber;Version;Type;Name;DeviceName
    ///     6ES7 211-1BD30-0XB0;V2.0;;Name_1;DeviceName_1
    ///     6SL3040-1MA00-0Axx;V4.8;G130;Name_2;DeviceName_2
    ///     \endcode
    ///     \image html ExampleCsvFile.png
    /// </remarks>
    public class HardwareGenerationFileBased
    {
        private const char DefaultSeparator = ';';
        private const char AlternativeSeparator = ',';
        private const string TypeColumn = "Type";

        private static readonly string[] RequiredColumns = { "OrderNumber", "Version", "Name", "DeviceName" };

        private static readonly string[] UnsupportedExcelExtensions = { ".xlsx", ".xlsm", ".xls" };

        /// <summary>
        ///     Creates new devices in the TIA Portal project based on information from a CSV file.
        ///     \image html CreateNewDevicesFromCsvFile.png
        /// </summary>
        /// <param name="module">The MAC use cases module instance.</param>
        /// <param name="tiaProject">The target TIA Portal project.</param>
        /// <param name="csvFilePath">Path to the CSV file containing device specifications.</param>
        /// <exception cref="Exception">
        ///     Thrown when the CSV file can't be read (e.g. a required column is missing).
        /// </exception>
        /// <remarks>
        ///     This method:
        ///     - Logs a warning and returns if the file doesn't exist or is an Excel file
        ///       (e.g. a path saved in an older module configuration), so the generation continues
        ///     - Reads device information from the CSV file
        ///     - Creates devices in the TIA Portal project
        ///     - Logs the progress and any errors that occur during device creation
        /// </remarks>
        public static void CreateNewDevicesFromCsvFile(MAC_use_casesEM module, Project tiaProject,
            string csvFilePath)
        {
            var warning = GetImportSourceWarning(csvFilePath);
            if (warning != null)
            {
                MacManagement.LoggingService.LogMessage(LogTypes.GenerationWarning, warning, module.Name);
                return;
            }

            try
            {
                var deviceInfos = ReadDevicesFromCsvFile(csvFilePath);

                foreach (var deviceInfo in deviceInfos)
                {
                    try
                    {
                        MacManagement.LoggingService.LogMessage(LogTypes.GenerationInfo,
                            $"Processing device: {deviceInfo}", module.Name);

                        var typeIdentifier = deviceInfo.GetTypeIdentifier();

                        MacManagement.LoggingService.LogMessage(LogTypes.GenerationInfo,
                            $"Creating device with identifier: {typeIdentifier}", module.Name);

                        HardwareGeneration.GetOrCreateDevice(tiaProject, typeIdentifier, deviceInfo.Name,
                            deviceInfo.DeviceName);

                        MacManagement.LoggingService.LogMessage(LogTypes.GenerationInfo,
                            $"Successfully added device: {deviceInfo.DeviceName}", module.Name);
                    }
                    catch (Exception ex)
                    {
                        MacManagement.LoggingService.LogMessage(LogTypes.GenerationError,
                            $"Failed to process device {deviceInfo}. Error: {ex.Message}", module.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating devices from CSV file: {ex.Message}", ex);
            }
        }

        /// <summary>
        ///     Reads device information from a CSV file.
        /// </summary>
        /// <param name="filePath">The full path to the CSV file containing device information.</param>
        /// <returns>The devices of all rows that contain the required values.</returns>
        /// <exception cref="InvalidDataException">Thrown when the file is empty or a required column is missing.</exception>
        public static List<DeviceInfo> ReadDevicesFromCsvFile(string filePath)
        {
            return ReadDevicesFromCsv(ReadCsvText(filePath));
        }

        /// <summary>
        ///     Reads device information from CSV content.
        /// </summary>
        /// <param name="csvContent">The CSV content, including the header row.</param>
        /// <returns>The devices of all rows that contain the required values.</returns>
        /// <exception cref="InvalidDataException">Thrown when the content is empty or a required column is missing.</exception>
        public static List<DeviceInfo> ReadDevicesFromCsv(string csvContent)
        {
            if (csvContent == null)
            {
                throw new ArgumentNullException(nameof(csvContent));
            }

            var content = csvContent.TrimStart('\uFEFF');
            var records = ParseRecords(content, DetectSeparator(content));

            var headerIndex = records.FindIndex(record => !IsEmptyRecord(record));
            if (headerIndex < 0)
            {
                throw new InvalidDataException("The CSV file is empty.");
            }

            // Create a mapping of column indices to ensure flexibility in column order
            var columnMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var header = records[headerIndex];
            for (var i = 0; i < header.Count; i++)
            {
                var columnName = header[i].Trim();
                if (columnName.Length > 0 && !columnMap.ContainsKey(columnName))
                {
                    columnMap[columnName] = i;
                }
            }

            foreach (var column in RequiredColumns)
            {
                if (!columnMap.ContainsKey(column))
                {
                    throw new InvalidDataException($"Required column '{column}' not found in CSV file.");
                }
            }

            var hasTypeColumn = columnMap.TryGetValue(TypeColumn, out var typeIndex);
            var devices = new List<DeviceInfo>();

            foreach (var record in records.Skip(headerIndex + 1))
            {
                if (IsEmptyRecord(record))
                {
                    continue;
                }

                var device = new DeviceInfo
                {
                    OrderNumber = GetValue(record, columnMap["OrderNumber"]),
                    Version = GetValue(record, columnMap["Version"]),
                    Type = hasTypeColumn ? GetValue(record, typeIndex) : string.Empty,
                    Name = GetValue(record, columnMap["Name"]),
                    DeviceName = GetValue(record, columnMap["DeviceName"])
                };

                // Only add device if required fields are not empty
                if (!string.IsNullOrWhiteSpace(device.OrderNumber) &&
                    !string.IsNullOrWhiteSpace(device.Version) &&
                    !string.IsNullOrWhiteSpace(device.Name) &&
                    !string.IsNullOrWhiteSpace(device.DeviceName))
                {
                    devices.Add(device);
                }
            }

            return devices;
        }

        /// <summary>
        ///     Checks whether the import source can be used for the hardware generation.
        /// </summary>
        /// <param name="csvFilePath">The path of the CSV file.</param>
        /// <returns>
        ///     A warning message if the file is an Excel file or doesn't exist; null if the file can be imported.
        /// </returns>
        public static string? GetImportSourceWarning(string csvFilePath)
        {
            if (IsExcelFile(csvFilePath))
            {
                return $"Excel files are no longer supported for the hardware generation: '{csvFilePath}'. " +
                       "Save the sheet as CSV and select the CSV file instead. No devices were created from this file.";
            }

            if (!File.Exists(csvFilePath))
            {
                return $"CSV file for the hardware generation not found: '{csvFilePath}'. " +
                       "Select an existing CSV file. No devices were created from this file.";
            }

            return null;
        }

        /// <summary>
        ///     Checks whether the file is an Excel workbook, which is no longer supported.
        /// </summary>
        /// <param name="filePath">The path of the file.</param>
        /// <returns>True if the file extension belongs to an Excel workbook.</returns>
        public static bool IsExcelFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }

            var extension = Path.GetExtension(filePath.Trim());
            return UnsupportedExcelExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

        private static string ReadCsvText(string filePath)
        {
            var bytes = File.ReadAllBytes(filePath);
            try
            {
                return new UTF8Encoding(false, true).GetString(bytes);
            }
            catch (DecoderFallbackException)
            {
                // Excel saves "CSV (semicolon/comma delimited)" in the ANSI code page, not in UTF-8.
                return Encoding.Default.GetString(bytes);
            }
        }

        private static char DetectSeparator(string content)
        {
            var semicolons = 0;
            var commas = 0;
            var inQuotes = false;

            foreach (var c in content)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (!inQuotes && (c == '\r' || c == '\n'))
                {
                    if (semicolons + commas > 0)
                    {
                        break;
                    }
                }
                else if (!inQuotes && c == DefaultSeparator)
                {
                    semicolons++;
                }
                else if (!inQuotes && c == AlternativeSeparator)
                {
                    commas++;
                }
            }

            return commas > semicolons ? AlternativeSeparator : DefaultSeparator;
        }

        private static List<List<string>> ParseRecords(string content, char separator)
        {
            var records = new List<List<string>>();
            var record = new List<string>();
            var field = new StringBuilder();
            var inQuotes = false;

            for (var i = 0; i < content.Length; i++)
            {
                var c = content[i];

                if (inQuotes)
                {
                    if (c != '"')
                    {
                        field.Append(c);
                    }
                    else if (i + 1 < content.Length && content[i + 1] == '"')
                    {
                        field.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else if (c == '"' && field.ToString().Trim().Length == 0)
                {
                    field.Clear();
                    inQuotes = true;
                }
                else if (c == separator)
                {
                    record.Add(field.ToString());
                    field.Clear();
                }
                else if (c == '\r' || c == '\n')
                {
                    if (c == '\r' && i + 1 < content.Length && content[i + 1] == '\n')
                    {
                        i++;
                    }

                    record.Add(field.ToString());
                    field.Clear();
                    records.Add(record);
                    record = new List<string>();
                }
                else
                {
                    field.Append(c);
                }
            }

            if (inQuotes)
            {
                throw new InvalidDataException("The CSV file contains a quoted value without a closing quote.");
            }

            if (field.Length > 0 || record.Count > 0)
            {
                record.Add(field.ToString());
                records.Add(record);
            }

            return records;
        }

        private static bool IsEmptyRecord(List<string> record)
        {
            return record.All(string.IsNullOrWhiteSpace);
        }

        private static string GetValue(List<string> record, int index)
        {
            return index < record.Count ? record[index].Trim() : string.Empty;
        }

        /// <summary>
        ///     Represents device information read from the CSV file.
        /// </summary>
        public class DeviceInfo
        {
            /// <summary>The order number (MLFB) of the device, e.g. 6ES7 511-1AK00-0AB0.</summary>
            public string OrderNumber { get; set; } = string.Empty;

            /// <summary>The firmware version of the device, e.g. V1.8.</summary>
            public string Version { get; set; } = string.Empty;

            /// <summary>Optional type suffix of the type identifier, e.g. S120.</summary>
            public string Type { get; set; } = string.Empty;

            /// <summary>The name of the device item in TIA Portal.</summary>
            public string Name { get; set; } = string.Empty;

            /// <summary>The name of the device in TIA Portal.</summary>
            public string DeviceName { get; set; } = string.Empty;

            /// <summary>
            ///     Builds the TIA Portal type identifier of the device.
            /// </summary>
            /// <returns>OrderNumber:&lt;OrderNumber&gt;/&lt;Version&gt;[/&lt;Type&gt;]</returns>
            public string GetTypeIdentifier()
            {
                return string.IsNullOrWhiteSpace(Type)
                    ? $"OrderNumber:{OrderNumber}/{Version}"
                    : $"OrderNumber:{OrderNumber}/{Version}/{Type}";
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return $"Device Information:\n" +
                       $"  Name: {Name}\n" +
                       $"  Type: {Type}\n" +
                       $"  Order Number: {OrderNumber}\n" +
                       $"  Version: {Version}\n" +
                       $"  Device Name: {DeviceName}";
            }
        }
    }
}
