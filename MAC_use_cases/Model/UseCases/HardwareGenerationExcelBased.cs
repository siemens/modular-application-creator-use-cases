using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Siemens.Automation.ModularApplicationCreator.Core;
using Siemens.Automation.ModularApplicationCreatorBasics.Logging;
using Siemens.Engineering;
using Excel = Microsoft.Office.Interop.Excel;

namespace MAC_use_cases.Model.UseCases;

/// <summary>
///     Handles the generation of hardware configurations based on Excel file inputs.
///     This class provides functionality to read device information from Excel sheets
///     and create corresponding hardware configurations in TIA Portal projects.
/// </summary>
public class HardwareGenerationExcelBased
{
    /// <summary>
    ///     Reads device information from a specified Excel file.
    ///     \image html ExampleExcelFile.png
    /// </summary>
    /// <param name="filePath">The full path to the Excel file containing device information.</param>
    /// <returns>A list of DeviceInfo objects containing the parsed device information.</returns>
    /// <exception cref="Exception">Thrown when required columns are missing in the Excel file.</exception>
    /// <remarks>
    ///     The Excel file must contain the following required columns:
    ///     - OrderNumber
    ///     - Version
    ///     - Name
    ///     - DeviceName
    ///     The "Type" column is optional.
    /// </remarks>
    private static List<DeviceInfo> ReadExcelFile(string filePath)
    {
        var devices = new List<DeviceInfo>();
        Excel.Application xlApp = null;
        Excel.Workbook xlWorkbook = null;
        Excel.Worksheet xlWorksheet = null;
        Excel.Range xlRange = null;

        try
        {
            xlApp = new Excel.Application();
            xlWorkbook = xlApp.Workbooks.Open(filePath);
            xlWorksheet = xlWorkbook.Sheets[1];
            xlRange = xlWorksheet.UsedRange;

            var rowCount = xlRange.Rows.Count;
            var colCount = xlRange.Columns.Count;

            // Create a mapping of column indices to ensure flexibility in column order
            var columnMap = new Dictionary<string, int>();
            for (var j = 1; j <= colCount; j++)
            {
                if (xlRange.Cells[1, j].Value2 != null)
                {
                    columnMap[xlRange.Cells[1, j].Value2.ToString()] = j;
                }
            }

            // Validate required columns exist (excluding Type as it's optional)
            string[] requiredColumns = { "OrderNumber", "Version", "Name", "DeviceName" };
            foreach (var column in requiredColumns)
            {
                if (!columnMap.ContainsKey(column))
                {
                    throw new Exception($"Required column '{column}' not found in Excel file");
                }
            }

            // Read data rows
            for (var i = 2; i <= rowCount; i++) // Start from row 2 to skip headers
            {
                if (xlRange.Cells[i, columnMap["OrderNumber"]].Value2 == null)
                {
                    continue;
                }

                var device = new DeviceInfo
                {
                    OrderNumber = GetCellValueAsString(xlRange.Cells[i, columnMap["OrderNumber"]]),
                    Version = GetCellValueAsString(xlRange.Cells[i, columnMap["Version"]]),
                    Name = GetCellValueAsString(xlRange.Cells[i, columnMap["Name"]]),
                    DeviceName = GetCellValueAsString(xlRange.Cells[i, columnMap["DeviceName"]])
                };

                // Handle Type column separately as it's optional
                if (columnMap.TryGetValue("Type", out var value))
                {
                    device.Type = GetCellValueAsString(xlRange.Cells[i, value]);
                }

                // Only add device if required fields are not empty
                if (!string.IsNullOrWhiteSpace(device.OrderNumber) &&
                    !string.IsNullOrWhiteSpace(device.Version) &&
                    !string.IsNullOrWhiteSpace(device.Name) &&
                    !string.IsNullOrWhiteSpace(device.DeviceName))
                {
                    devices.Add(device);
                }
            }
        }
        finally
        {
            // Clean up COM objects
            if (xlRange != null)
            {
                Marshal.ReleaseComObject(xlRange);
            }

            if (xlWorksheet != null)
            {
                Marshal.ReleaseComObject(xlWorksheet);
            }

            if (xlWorkbook != null)
            {
                xlWorkbook.Close(false);
                Marshal.ReleaseComObject(xlWorkbook);
            }

            if (xlApp != null)
            {
                xlApp.Quit();
                Marshal.ReleaseComObject(xlApp);
            }
        }

        return devices;
    }

    /// <summary>
    ///     Converts an Excel cell value to a string, handling null values.
    /// </summary>
    /// <param name="cell">The Excel range cell to read.</param>
    /// <returns>The cell value as a trimmed string, or empty string if the cell is null.</returns>
    private static string GetCellValueAsString(Excel.Range cell)
    {
        return cell?.Value2 == null ? string.Empty : (string)cell.Value2.ToString().Trim();
    }

    /// <summary>
    ///     Creates new devices in the TIA Portal project based on information from an Excel sheet.
    ///     \image html CreateNewDevicesFromExcelSheet.png
    /// </summary>
    /// <param name="module">The MAC use cases module instance.</param>
    /// <param name="tiaProject">The target TIA Portal project.</param>
    /// <param name="excelFilePath">Path to the Excel file containing device specifications.</param>
    /// <exception cref="FileNotFoundException">Thrown when the specified Excel file is not found.</exception>
    /// <exception cref="Exception">Thrown when there's an error in creating devices from the Excel file.</exception>
    /// <remarks>
    ///     This method:
    ///     - Validates the Excel file existence
    ///     - Reads device information from the Excel file
    ///     - Creates devices in the TIA Portal project
    ///     - Logs the progress and any errors that occur during device creation
    /// </remarks>
    public static void CreateNewDevicesFromExcelSheet(MAC_use_casesEM module, Project tiaProject, string excelFilePath)
    {
        try
        {
            if (!File.Exists(excelFilePath))
            {
                throw new FileNotFoundException("Excel file not found", excelFilePath);
            }

            var deviceInfos = ReadExcelFile(excelFilePath);

            foreach (var deviceInfo in deviceInfos)
            {
                try
                {
                    MacManagement.LoggingService.LogMessage(LogTypes.GenerationInfo,
                        $"Processing device: {deviceInfo}", module.Name);

                    var typeIdentifier = string.IsNullOrWhiteSpace(deviceInfo.Type)
                        ? $"OrderNumber:{deviceInfo.OrderNumber}/{deviceInfo.Version}"
                        : $"OrderNumber:{deviceInfo.OrderNumber}/{deviceInfo.Version}/{deviceInfo.Type}";

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
            throw new Exception($"Error creating devices from Excel file: {ex.Message}", ex);
        }
    }

    /// <summary>
    ///     Represents device information read from the Excel file.
    /// </summary>
    private class DeviceInfo
    {
        public string OrderNumber { get; set; }
        public string Version { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string DeviceName { get; set; }

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
