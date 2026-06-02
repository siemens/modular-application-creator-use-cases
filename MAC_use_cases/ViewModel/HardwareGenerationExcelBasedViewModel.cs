using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Win32;
using Siemens.Automation.ModularApplicationCreatorBasics.ViewModels;

namespace MAC_use_cases.ViewModel
{
    public class HardwareGenerationExcelBasedViewModel : INotifyPropertyChanged
    {
        private string _importSource;

        public HardwareGenerationExcelBasedViewModel()
        {
            BrowseImportFile = new RelayCommand(ExecuteBrowseImportFile);
            ImportSource = GetDefaultExcelFilePath();
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

        public ICommand BrowseImportFile { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private static string GetDefaultExcelFilePath()
        {
            return Path.Combine(GetInitialDirectory(), "HardwareGenerationExcelBased.xlsx");
        }

        private static string GetAssemblyLocation()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        private static string GetInitialDirectory()
        {
            return Path.GetFullPath(Path.Combine(GetAssemblyLocation(), "..", "..", "contentFiles", "any", "net48",
                "AdditionalContent"));
        }

        private void ExecuteBrowseImportFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files|*.xlsx;*.xls|All Files|*.*",
                Title = "Select Excel File",
                InitialDirectory = GetInitialDirectory(),
                FileName = GetDefaultExcelFilePath()
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
