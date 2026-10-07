using System.IO;
using MAC_use_cases.Serialization;
using MAC_use_cases.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace MAC_use_cases.Tests.Unit.Tests
{
    [TestFixture]
    public class ImportSourceSerializationTests
    {
        private const string ConfigDirectory = @"C:\Data\Config\ModulConfig";

        private static JObject CreateConfig(string importSource)
        {
            return new JObject
            {
                ["HardwareGenerationExcelBasedViewModel"] = new JObject
                {
                    ["ImportSource"] = importSource,
                    ["ExportRelativeImportSource"] = true
                }
            };
        }

        private static string GetImportSource(JObject json)
        {
            return (string)json["HardwareGenerationExcelBasedViewModel"]["ImportSource"];
        }

        [TestCase("HardwareGeneration.csv", @"C:\Data\Config\ModulConfig\HardwareGeneration.csv")]
        [TestCase(@"..\..\AdditionalContent\HardwareGeneration.csv", @"C:\Data\AdditionalContent\HardwareGeneration.csv")]
        [TestCase(@"D:\Other\HardwareGeneration.csv", @"D:\Other\HardwareGeneration.csv")]
        public void ResolveImportSourceOnImport_ResolvesRelativePathAgainstConfigFolder(string importSource,
            string expected)
        {
            var json = CreateConfig(importSource);

            MAC_use_casesSerializer.ResolveImportSourceOnImport(json, ConfigDirectory);

            Assert.That(GetImportSource(json), Is.EqualTo(expected));
        }

        [TestCase(@"C:\Data\Config\ModulConfig\HardwareGeneration.csv", "HardwareGeneration.csv")]
        [TestCase(@"C:\Data\AdditionalContent\HardwareGeneration.csv", @"..\..\AdditionalContent\HardwareGeneration.csv")]
        [TestCase(@"D:\Other\HardwareGeneration.csv", @"D:\Other\HardwareGeneration.csv")]
        public void MakeImportSourceRelativeOnExport_WritesPathRelativeToConfigFolder(string importSource,
            string expected)
        {
            var json = CreateConfig(importSource);

            MAC_use_casesSerializer.MakeImportSourceRelativeOnExport(json, ConfigDirectory);

            Assert.That(GetImportSource(json), Is.EqualTo(expected));
        }

        [Test]
        public void ImportThenExport_SameFolder_KeepsRelativePath()
        {
            var json = CreateConfig(@"..\..\AdditionalContent\HardwareGeneration.csv");

            MAC_use_casesSerializer.ResolveImportSourceOnImport(json, ConfigDirectory);
            MAC_use_casesSerializer.MakeImportSourceRelativeOnExport(json, ConfigDirectory);

            Assert.That(GetImportSource(json), Is.EqualTo(@"..\..\AdditionalContent\HardwareGeneration.csv"));
        }

        [TestCase("{}")]
        [TestCase("{\"HardwareGenerationExcelBasedViewModel\": {}}")]
        [TestCase("{\"HardwareGenerationExcelBasedViewModel\": {\"ImportSource\": null}}")]
        [TestCase("{\"HardwareGenerationExcelBasedViewModel\": {\"$ref\": \"3\"}}")]
        public void ImportAndExport_WithoutImportSource_DoNothing(string config)
        {
            var json = JObject.Parse(config);

            MAC_use_casesSerializer.ResolveImportSourceOnImport(json, ConfigDirectory);
            MAC_use_casesSerializer.MakeImportSourceRelativeOnExport(json, ConfigDirectory);

            Assert.That(JToken.DeepEquals(json, JObject.Parse(config)), Is.True);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ExportRelativeImportSource_IsSerializedAndImported(bool value)
        {
            var exported = new HardwareGenerationExcelBasedViewModel { ExportRelativeImportSource = value };

            var json = JsonConvert.SerializeObject(exported);
            var imported = new HardwareGenerationExcelBasedViewModel { ExportRelativeImportSource = !value };
            JsonConvert.PopulateObject(json, imported);

            Assert.That(JObject.Parse(json).Value<bool>("ExportRelativeImportSource"), Is.EqualTo(value));
            Assert.That(imported.ExportRelativeImportSource, Is.EqualTo(value));
        }

        [Test]
        public void ExportRelativeImportSource_DefaultIsAbsolute()
        {
            Assert.That(new HardwareGenerationExcelBasedViewModel().ExportRelativeImportSource, Is.False);
        }

        [Test]
        public void CliExample_ImportSource_PointsToExampleCsvInRepository()
        {
            var repositoryRoot = TestContext.CurrentContext.TestDirectory;
            while (repositoryRoot != null && !File.Exists(Path.Combine(repositoryRoot, "MAC_use_cases.sln")))
            {
                repositoryRoot = Path.GetDirectoryName(repositoryRoot);
            }

            Assume.That(repositoryRoot, Is.Not.Null, "Repository root not found.");
            var configPath = Path.Combine(repositoryRoot, "MAC_use_cases", "CLI_Example", "ModulConfig",
                "MAC_use_cases.json");
            var json = JObject.Parse(File.ReadAllText(configPath));

            MAC_use_casesSerializer.ResolveImportSourceOnImport(json, Path.GetDirectoryName(configPath));

            Assert.That(GetImportSource(json),
                Is.EqualTo(Path.Combine(repositoryRoot, "MAC_use_cases", "AdditionalContent", "HardwareGeneration.csv")));
            Assert.That(File.Exists(GetImportSource(json)), Is.True);
        }
    }
}
