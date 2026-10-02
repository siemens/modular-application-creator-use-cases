using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MAC_use_cases.Model.UseCases;
using Microsoft.Win32;
using Siemens.Automation.ModularApplicationCreatorBasics.ViewModels;

namespace MAC_use_cases.ViewModel
{
    /// <summary>
    ///     ViewModel for the hardware generation based on a CSV file.
    ///     The class name is kept for compatibility with saved module configurations.
    /// </summary>
    public class HardwareGenerationExcelBasedViewModel : INotifyPropertyChanged
    {
        private string _importSource;
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
                }
            }
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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
