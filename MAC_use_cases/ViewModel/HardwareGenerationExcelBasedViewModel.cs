using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MAC_use_cases.Model.UseCases;
using Microsoft.Win32;
using Newtonsoft.Json;
using Siemens.Automation.ModularApplicationCreatorBasics.ViewModels;

namespace MAC_use_cases.ViewModel
{
    /// <summary>
    ///     ViewModel for the hardware generation based on a CSV file.
    ///     The class name is kept for compatibility with saved module configurations.
    /// </summary>
    public class HardwareGenerationExcelBasedViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private string _importSource;
        private string? _importSourceError;
        private bool _exportRelativeImportSource;

        public HardwareGenerationExcelBasedViewModel()
        {
            BrowseImportFile = new RelayCommand(ExecuteBrowseImportFile);
            ImportSource = GetDefaultCsvFilePath();
        }

        public string ImportSource
        {
            get => _importSource;
            set
            {
                if (_importSource != value)
                {
                    _importSource = value;
                    OnPropertyChanged();
                    ValidateImportSource();
                }
            }
        }

        /// <summary>
        ///     True if <see cref="ImportSource" /> points to a missing file or an Excel file.
        ///     The error is only shown in the UI; saving, export and generation are not blocked.
        /// </summary>
        [JsonIgnore]
        public bool HasErrors => _importSourceError != null;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (_importSourceError != null &&
                (string.IsNullOrEmpty(propertyName) || propertyName == nameof(ImportSource)))
            {
                return new[] { _importSourceError };
            }

            return Enumerable.Empty<string>();
        }

        /// <summary>
        ///     Defines whether <see cref="ImportSource" /> is exported relative to the folder of the exported
        ///     module configuration .json (true) or as an absolute path (false).
        ///     The setting is saved with the project and exported/imported with the .json.
        /// </summary>
        public bool ExportRelativeImportSource
        {
            get => _exportRelativeImportSource;
            set
            {
                if (_exportRelativeImportSource != value)
                {
                    _exportRelativeImportSource = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand BrowseImportFile { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private static string GetDefaultCsvFilePath()
        {
            return Path.Combine(GetInitialDirectory(), "HardwareGeneration.csv");
        }

        private static string GetInitialDirectory()
        {
            return HardwareGenerationFileBased.GetAdditionalContentDirectory();
        }

        private void ExecuteBrowseImportFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files|*.csv|All Files|*.*",
                Title = "Select CSV File",
                InitialDirectory = GetInitialDirectory(),
                FileName = GetDefaultCsvFilePath()
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ImportSource = openFileDialog.FileName;
            }
        }

        private void ValidateImportSource()
        {
            // An empty ImportSource is skipped by the generation, so it isn't an error.
            var problem = string.IsNullOrWhiteSpace(_importSource)
                ? null
                : HardwareGenerationFileBased.GetImportSourceProblem(_importSource);
            var error = problem == null ? null : problem + " No devices will be generated from this file.";

            if (error == _importSourceError)
            {
                return;
            }

            _importSourceError = error;
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(ImportSource)));
            OnPropertyChanged(nameof(HasErrors));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
