using System.IO;
using System.Linq;
using System.Text;
using MAC_use_cases.Model.UseCases;
using NUnit.Framework;

namespace MAC_use_cases.Tests.Unit.Tests
{
    [TestFixture]
    public class HardwareGenerationFileBasedTests
    {
        private string _tempDirectory;

        [SetUp]
        public void SetUp()
        {
            _tempDirectory = Path.Combine(Path.GetTempPath(), "MAC_use_cases_Tests_" + Path.GetRandomFileName());
            Directory.CreateDirectory(_tempDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }

        [Test]
        public void ReadDevicesFromCsvFile_ShippedExample_ReturnsAllTenDevices()
        {
            var examplePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources",
                "HardwareGeneration.csv");

            var devices = HardwareGenerationFileBased.ReadDevicesFromCsvFile(examplePath);

            Assert.That(devices.Select(d => d.Name),
                Is.EqualTo(Enumerable.Range(1, 10).Select(i => $"Name_{i}")));
            Assert.That(devices[0].GetTypeIdentifier(), Is.EqualTo("OrderNumber:6ES7 211-1BD30-0XB0/V2.0"));
            Assert.That(devices[0].DeviceName, Is.EqualTo("DeviceName_1"));
            Assert.That(devices[3].GetTypeIdentifier(), Is.EqualTo("OrderNumber:6SL3040-1LA01-0Axx/V4.8/S120"));
            Assert.That(devices[9].GetTypeIdentifier(), Is.EqualTo("OrderNumber:6ES7 517-3UP00-0AB0/V3.1"));
        }

        [TestCase(';')]
        [TestCase(',')]
        public void ReadDevicesFromCsv_DetectsSeparator(char separator)
        {
            var csv = string.Join("\r\n",
                string.Join(separator.ToString(), "OrderNumber", "Version", "Type", "Name", "DeviceName"),
                string.Join(separator.ToString(), "6SL3040-1MA00-0Axx", "V4.8", "G130", "Name_2", "DeviceName_2"));

            var device = HardwareGenerationFileBased.ReadDevicesFromCsv(csv).Single();

            Assert.That(device.OrderNumber, Is.EqualTo("6SL3040-1MA00-0Axx"));
            Assert.That(device.Version, Is.EqualTo("V4.8"));
            Assert.That(device.Type, Is.EqualTo("G130"));
            Assert.That(device.Name, Is.EqualTo("Name_2"));
            Assert.That(device.DeviceName, Is.EqualTo("DeviceName_2"));
        }

        [Test]
        public void ReadDevicesFromCsv_ContentWithByteOrderMark_ReadsFirstColumn()
        {
            var csv = "\uFEFFOrderNumber;Version;Name;DeviceName\n6ES7 511-1AK00-0AB0;V1.8;Name_9;DeviceName_9";

            var device = HardwareGenerationFileBased.ReadDevicesFromCsv(csv).Single();

            Assert.That(device.OrderNumber, Is.EqualTo("6ES7 511-1AK00-0AB0"));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadDevicesFromCsvFile_Utf8WithAndWithoutBom_ReadsSpecialCharacters(bool withBom)
        {
            var path = WriteFile(
                "OrderNumber;Version;Name;DeviceName\r\n6ES7 511-1AK00-0AB0;V1.8;F\u00F6rderband_\u00DC;Ger\u00E4t_\u00DF",
                new UTF8Encoding(withBom));

            var device = HardwareGenerationFileBased.ReadDevicesFromCsvFile(path).Single();

            Assert.That(device.OrderNumber, Is.EqualTo("6ES7 511-1AK00-0AB0"));
            Assert.That(device.Name, Is.EqualTo("F\u00F6rderband_\u00DC"));
            Assert.That(device.DeviceName, Is.EqualTo("Ger\u00E4t_\u00DF"));
        }

        [Test]
        public void ReadDevicesFromCsvFile_AnsiEncodedFile_ReadsSpecialCharacters()
        {
            var path = WriteFile(
                "OrderNumber;Version;Name;DeviceName\r\n6ES7 511-1AK00-0AB0;V1.8;F\u00F6rderband;Ger\u00E4t",
                Encoding.Default);

            var device = HardwareGenerationFileBased.ReadDevicesFromCsvFile(path).Single();

            Assert.That(device.Name, Is.EqualTo("F\u00F6rderband"));
            Assert.That(device.DeviceName, Is.EqualTo("Ger\u00E4t"));
        }

        [Test]
        public void ReadDevicesFromCsv_QuotedValues_SupportsSeparatorsQuotesAndLineBreaks()
        {
            var csv = "OrderNumber;Version;Type;Name;DeviceName\r\n" +
                      "\"6ES7 511-1AK00-0AB0\";\"V1.8\";\"\";\"Name;with \"\"quotes\"\"\";\"Device\r\nName\"";

            var device = HardwareGenerationFileBased.ReadDevicesFromCsv(csv).Single();

            Assert.That(device.OrderNumber, Is.EqualTo("6ES7 511-1AK00-0AB0"));
            Assert.That(device.Type, Is.Empty);
            Assert.That(device.Name, Is.EqualTo("Name;with \"quotes\""));
            Assert.That(device.DeviceName, Is.EqualTo("Device\r\nName"));
        }

        [Test]
        public void ReadDevicesFromCsv_WithoutTypeColumn_UsesTypeIdentifierWithoutType()
        {
            var csv = "OrderNumber;Version;Name;DeviceName\n6ES7 511-1AK00-0AB0;V1.8;Name_9;DeviceName_9";

            var device = HardwareGenerationFileBased.ReadDevicesFromCsv(csv).Single();

            Assert.That(device.Type, Is.Empty);
            Assert.That(device.GetTypeIdentifier(), Is.EqualTo("OrderNumber:6ES7 511-1AK00-0AB0/V1.8"));
        }

        [Test]
        public void ReadDevicesFromCsv_ColumnsInDifferentOrderAndCase_MapsByHeader()
        {
            var csv = "devicename;TYPE;name;version;ordernumber\nDeviceName_4;S120;Name_4;V4.8;6SL3040-1LA01-0Axx";

            var device = HardwareGenerationFileBased.ReadDevicesFromCsv(csv).Single();

            Assert.That(device.GetTypeIdentifier(), Is.EqualTo("OrderNumber:6SL3040-1LA01-0Axx/V4.8/S120"));
            Assert.That(device.Name, Is.EqualTo("Name_4"));
            Assert.That(device.DeviceName, Is.EqualTo("DeviceName_4"));
        }

        [TestCase("OrderNumber")]
        [TestCase("Version")]
        [TestCase("Name")]
        [TestCase("DeviceName")]
        public void ReadDevicesFromCsv_MissingRequiredColumn_Throws(string missingColumn)
        {
            var columns = new[] { "OrderNumber", "Version", "Type", "Name", "DeviceName" }
                .Where(c => c != missingColumn);
            var csv = string.Join(";", columns) + "\nA;B;C;D";

            var exception = Assert.Throws<InvalidDataException>(() =>
                HardwareGenerationFileBased.ReadDevicesFromCsv(csv));

            Assert.That(exception.Message, Does.Contain($"'{missingColumn}'"));
        }

        [Test]
        public void ReadDevicesFromCsv_EmptyLinesAndIncompleteRows_AreSkipped()
        {
            var csv = "\r\n" +
                      "OrderNumber;Version;Type;Name;DeviceName\r\n" +
                      "\r\n" +
                      "6ES7 211-1BD30-0XB0;V2.0;;Name_1;DeviceName_1\r\n" +
                      ";;;;\r\n" +
                      "6ES7 511-1AK00-0AB0;V1.8;;;DeviceName_9\r\n" +
                      "6ES7 517-3UP00-0AB0;V3.1\r\n" +
                      "   \r\n" +
                      "6SL3040-1MA00-0Axx;V4.8;G130;Name_2;DeviceName_2\r\n";

            var devices = HardwareGenerationFileBased.ReadDevicesFromCsv(csv);

            Assert.That(devices.Select(d => d.Name), Is.EqualTo(new[] { "Name_1", "Name_2" }));
        }

        [Test]
        public void ReadDevicesFromCsv_ValuesWithSurroundingWhitespace_AreTrimmed()
        {
            var csv = "OrderNumber ; Version ; Name ; DeviceName\n 6ES7 511-1AK00-0AB0 ; V1.8 ; Name_9 ; DeviceName_9 ";

            var device = HardwareGenerationFileBased.ReadDevicesFromCsv(csv).Single();

            Assert.That(device.GetTypeIdentifier(), Is.EqualTo("OrderNumber:6ES7 511-1AK00-0AB0/V1.8"));
            Assert.That(device.DeviceName, Is.EqualTo("DeviceName_9"));
        }

        [TestCase("")]
        [TestCase("\uFEFF\r\n\r\n")]
        public void ReadDevicesFromCsv_EmptyContent_Throws(string csv)
        {
            Assert.Throws<InvalidDataException>(() => HardwareGenerationFileBased.ReadDevicesFromCsv(csv));
        }

        [Test]
        public void ReadDevicesFromCsv_UnclosedQuote_Throws()
        {
            var csv = "OrderNumber;Version;Name;DeviceName\n\"6ES7 511-1AK00-0AB0;V1.8;Name_9;DeviceName_9";

            Assert.Throws<InvalidDataException>(() => HardwareGenerationFileBased.ReadDevicesFromCsv(csv));
        }

        [TestCase(@"C:\Temp\HardwareGenerationExcelBased.xlsx", true)]
        [TestCase(@"C:\Temp\Devices.XLS", true)]
        [TestCase(@"C:\Temp\Devices.xlsm", true)]
        [TestCase(@"C:\Temp\HardwareGeneration.csv", false)]
        [TestCase(@"C:\Temp\Devices", false)]
        [TestCase("", false)]
        [TestCase(null, false)]
        public void IsExcelFile_DetectsExcelWorkbooks(string path, bool expected)
        {
            Assert.That(HardwareGenerationFileBased.IsExcelFile(path), Is.EqualTo(expected));
        }

        [Test]
        public void GetImportSourceWarning_ExistingCsvFile_ReturnsNull()
        {
            var path = WriteFile("OrderNumber;Version;Name;DeviceName", new UTF8Encoding(true));

            Assert.That(HardwareGenerationFileBased.GetImportSourceWarning(path), Is.Null);
        }

        [Test]
        public void GetImportSourceWarning_MissingFile_ReturnsWarningWithPath()
        {
            var path = Path.Combine(_tempDirectory, "missing.csv");

            Assert.That(HardwareGenerationFileBased.GetImportSourceWarning(path),
                Does.Contain("not found").And.Contain(path)
                    .And.EndWith("No devices were created from this file."));
        }

        [Test]
        public void GetImportSourceProblem_MissingFile_HasNoConsequenceSentence()
        {
            var path = Path.Combine(_tempDirectory, "missing.csv");

            Assert.That(HardwareGenerationFileBased.GetImportSourceProblem(path),
                Does.Contain("not found").And.Not.Contain("No devices"));
        }

        [Test]
        public void GetImportSourceProblem_ExistingCsvFile_ReturnsNull()
        {
            var path = WriteFile("OrderNumber;Version;Name;DeviceName", new UTF8Encoding(true));

            Assert.That(HardwareGenerationFileBased.GetImportSourceProblem(path), Is.Null);
        }

        [Test]
        public void GetImportSourceWarning_ExistingExcelFile_ReturnsExcelWarning()
        {
            var path = Path.Combine(_tempDirectory, "HardwareGenerationExcelBased.xlsx");
            File.WriteAllBytes(path, new byte[] { 0x50, 0x4B });

            Assert.That(HardwareGenerationFileBased.GetImportSourceWarning(path),
                Does.Contain("Excel files are no longer supported"));
        }

        [TestCase("HardwareGeneration.csv")]
        [TestCase(" HardwareGeneration.csv ")]
        [TestCase(@".\HardwareGeneration.csv")]
        [TestCase(@"Sub\..\HardwareGeneration.csv")]
        public void ResolveImportSource_RelativePath_ResolvesAgainstBaseDirectory(string importSource)
        {
            Assert.That(HardwareGenerationFileBased.ResolveImportSource(importSource, _tempDirectory),
                Is.EqualTo(Path.Combine(_tempDirectory, "HardwareGeneration.csv")));
        }

        [Test]
        public void ResolveImportSource_RelativePathToSubfolder_ResolvesAgainstBaseDirectory()
        {
            Assert.That(HardwareGenerationFileBased.ResolveImportSource(@"Plants\Line1.csv", _tempDirectory),
                Is.EqualTo(Path.Combine(_tempDirectory, "Plants", "Line1.csv")));
        }

        [TestCase(@"C:\Temp\HardwareGeneration.csv")]
        [TestCase(@"\\server\share\HardwareGeneration.csv")]
        public void ResolveImportSource_AbsolutePath_ReturnsUnchanged(string importSource)
        {
            Assert.That(HardwareGenerationFileBased.ResolveImportSource(importSource, _tempDirectory),
                Is.EqualTo(importSource));
        }

        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        [TestCase("Invalid|Name.csv")]
        public void ResolveImportSource_EmptyOrInvalidValue_ReturnsUnchanged(string importSource)
        {
            Assert.That(HardwareGenerationFileBased.ResolveImportSource(importSource, _tempDirectory),
                Is.EqualTo(importSource));
        }

        [Test]
        public void ResolveImportSource_RelativePathToExistingFile_CanBeImported()
        {
            var expectedPath = WriteFile("OrderNumber;Version;Name;DeviceName\n6ES7 511-1AK00-0AB0;V1.8;Name_9;D_9",
                new UTF8Encoding(true));

            var resolvedPath = HardwareGenerationFileBased.ResolveImportSource("devices.csv", _tempDirectory);

            Assert.That(resolvedPath, Is.EqualTo(expectedPath));
            Assert.That(HardwareGenerationFileBased.GetImportSourceWarning(resolvedPath), Is.Null);
            Assert.That(HardwareGenerationFileBased.ReadDevicesFromCsvFile(resolvedPath).Single().Name,
                Is.EqualTo("Name_9"));
        }

        [Test]
        public void ResolveImportSource_RelativePathToMissingFile_WarningShowsResolvedPath()
        {
            var resolvedPath = HardwareGenerationFileBased.ResolveImportSource("missing.csv", _tempDirectory);

            Assert.That(HardwareGenerationFileBased.GetImportSourceWarning(resolvedPath),
                Does.Contain("not found").And.Contain(Path.Combine(_tempDirectory, "missing.csv")));
        }

        [TestCase(@"C:\Data\Config", @"C:\Data\Config\HardwareGeneration.csv", "HardwareGeneration.csv")]
        [TestCase(@"C:\Data\Config\", @"C:\Data\Config\Csv\Line1.csv", @"Csv\Line1.csv")]
        [TestCase(@"C:\Data\Config\ModulConfig", @"C:\Data\AdditionalContent\HardwareGeneration.csv",
            @"..\..\AdditionalContent\HardwareGeneration.csv")]
        [TestCase(@"c:\data\config", @"C:\Data\Config\HardwareGeneration.csv", "HardwareGeneration.csv")]
        [TestCase(@"\\server\share\config", @"\\server\share\csv\Line1.csv", @"..\csv\Line1.csv")]
        public void MakeRelativeImportSource_PathOnSameRoot_ReturnsRelativePath(string baseDirectory,
            string importSource, string expected)
        {
            Assert.That(HardwareGenerationFileBased.MakeRelativeImportSource(importSource, baseDirectory),
                Is.EqualTo(expected));
        }

        [TestCase(@"C:\Data\Config", @"D:\Data\HardwareGeneration.csv")]
        [TestCase(@"C:\Data\Config", @"\\server\share\HardwareGeneration.csv")]
        [TestCase(@"C:\Data\Config", "HardwareGeneration.csv")]
        [TestCase(@"C:\Data\Config", "")]
        [TestCase(@"C:\Data\Config", null)]
        [TestCase(@"C:\Data\Config", "Invalid|Name.csv")]
        public void MakeRelativeImportSource_OtherRootRelativeOrInvalid_ReturnsUnchanged(string baseDirectory,
            string importSource)
        {
            Assert.That(HardwareGenerationFileBased.MakeRelativeImportSource(importSource, baseDirectory),
                Is.EqualTo(importSource));
        }

        [TestCase("HardwareGeneration.csv")]
        [TestCase(@"Csv\Line1.csv")]
        [TestCase(@"..\..\AdditionalContent\HardwareGeneration.csv")]
        public void MakeRelativeImportSource_AfterResolve_ReturnsOriginalRelativePath(string relativePath)
        {
            var resolved = HardwareGenerationFileBased.ResolveImportSource(relativePath, _tempDirectory);

            Assert.That(HardwareGenerationFileBased.MakeRelativeImportSource(resolved, _tempDirectory),
                Is.EqualTo(relativePath));
        }

        [Test]
        public void GetAdditionalContentDirectory_IsContentFilesFolderOfThePackage()
        {
            var assemblyDirectory = Path.GetDirectoryName(typeof(HardwareGenerationFileBased).Assembly.Location);
            var expected = Path.GetFullPath(Path.Combine(assemblyDirectory, "..", "..", "contentFiles", "any",
                "net48", "AdditionalContent"));

            Assert.That(HardwareGenerationFileBased.GetAdditionalContentDirectory(), Is.EqualTo(expected));
        }

        private string WriteFile(string content, Encoding encoding)
        {
            var path = Path.Combine(_tempDirectory, "devices.csv");
            File.WriteAllText(path, content, encoding);
            return path;
        }
    }
}
