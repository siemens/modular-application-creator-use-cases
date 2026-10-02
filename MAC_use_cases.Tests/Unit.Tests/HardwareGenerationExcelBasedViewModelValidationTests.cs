using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using MAC_use_cases.ViewModel;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace MAC_use_cases.Tests.Unit.Tests
{
    [TestFixture]
    public class HardwareGenerationExcelBasedViewModelValidationTests
    {
        private string _tempDirectory;
        private string _existingCsv;

        [SetUp]
        public void SetUp()
        {
            _tempDirectory = Path.Combine(Path.GetTempPath(), "MAC_use_cases_Tests_" + Path.GetRandomFileName());
            Directory.CreateDirectory(_tempDirectory);
            _existingCsv = Path.Combine(_tempDirectory, "HardwareGeneration.csv");
            File.WriteAllText(_existingCsv, "Name;TypeIdentifier\n");
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(_tempDirectory, true);
        }

        private static List<string> GetImportSourceErrors(INotifyDataErrorInfo viewModel)
        {
            return viewModel.GetErrors(nameof(HardwareGenerationExcelBasedViewModel.ImportSource))
                .Cast<string>().ToList();
        }

        [Test]
        public void ImportSource_MissingFile_HasNotFoundError()
        {
            var viewModel = new HardwareGenerationExcelBasedViewModel
            {
                ImportSource = Path.Combine(_tempDirectory, "Missing.csv")
            };

            Assert.That(viewModel.HasErrors, Is.True);
            Assert.That(GetImportSourceErrors(viewModel), Has.Count.EqualTo(1));
            Assert.That(GetImportSourceErrors(viewModel)[0], Does.Contain("not found"));
            Assert.That(GetImportSourceErrors(viewModel)[0],
                Does.EndWith("No devices will be generated from this file."));
            Assert.That(GetImportSourceErrors(viewModel)[0], Does.Not.Contain("were created"));
        }

        [Test]
        public void ImportSource_ExcelFile_HasExcelError()
        {
            var excelFile = Path.Combine(_tempDirectory, "HardwareGeneration.xlsx");
            File.WriteAllText(excelFile, string.Empty);

            var viewModel = new HardwareGenerationExcelBasedViewModel { ImportSource = excelFile };

            Assert.That(viewModel.HasErrors, Is.True);
            Assert.That(GetImportSourceErrors(viewModel)[0], Does.Contain("Excel files are no longer supported"));
        }

        [Test]
        public void ImportSource_ExistingCsv_ClearsError()
        {
            var viewModel = new HardwareGenerationExcelBasedViewModel
            {
                ImportSource = Path.Combine(_tempDirectory, "Missing.csv")
            };

            viewModel.ImportSource = _existingCsv;

            Assert.That(viewModel.HasErrors, Is.False);
            Assert.That(GetImportSourceErrors(viewModel), Is.Empty);
        }

        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void ImportSource_Empty_HasNoError(string importSource)
        {
            var viewModel = new HardwareGenerationExcelBasedViewModel { ImportSource = importSource };

            Assert.That(viewModel.HasErrors, Is.False);
        }

        [Test]
        public void ImportSource_ErrorStateChanges_RaisesErrorsChangedOnlyOnChange()
        {
            var viewModel = new HardwareGenerationExcelBasedViewModel { ImportSource = _existingCsv };
            var changedProperties = new List<string>();
            viewModel.ErrorsChanged += (_, e) => changedProperties.Add(e.PropertyName);

            viewModel.ImportSource = Path.Combine(_tempDirectory, "Missing.csv");
            viewModel.ImportSource = Path.Combine(_tempDirectory, "Missing.csv");
            viewModel.ImportSource = _existingCsv;

            Assert.That(changedProperties,
                Is.EqualTo(new[]
                {
                    nameof(HardwareGenerationExcelBasedViewModel.ImportSource),
                    nameof(HardwareGenerationExcelBasedViewModel.ImportSource)
                }));
        }

        [Test]
        public void GetErrors_OtherProperty_ReturnsNoErrors()
        {
            var viewModel = new HardwareGenerationExcelBasedViewModel
            {
                ImportSource = Path.Combine(_tempDirectory, "Missing.csv")
            };

            Assert.That(viewModel.GetErrors(nameof(HardwareGenerationExcelBasedViewModel.ExportRelativeImportSource)),
                Is.Empty);
        }

        [Test]
        public void Serialization_DoesNotWriteHasErrors()
        {
            var viewModel = new HardwareGenerationExcelBasedViewModel
            {
                ImportSource = Path.Combine(_tempDirectory, "Missing.csv")
            };

            var json = JObject.FromObject(viewModel);

            Assert.That(json.ContainsKey(nameof(HardwareGenerationExcelBasedViewModel.HasErrors)), Is.False);
            Assert.That(json.ContainsKey(nameof(HardwareGenerationExcelBasedViewModel.ImportSource)), Is.True);
        }
    }
}
