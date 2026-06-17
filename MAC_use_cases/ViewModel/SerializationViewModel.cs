using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MAC_use_cases.Model;
using MAC_use_cases.Serialization;
using Microsoft.Win32;
using Siemens.Automation.ModularApplicationCreatorBasics.ViewModels;

namespace MAC_use_cases.ViewModel
{
    public class SerializationViewModel : INotifyPropertyChanged
    {
        private const string FileFilter = "JSON Files|*.json|All Files|*.*";
        private const string FileExtension = ".json";

        private readonly MAC_use_casesEM _module;

        public SerializationViewModel(MAC_use_casesEM module)
        {
            _module = module;
            ExportCommand = new RelayCommand(ExecuteExport);
            ImportCommand = new RelayCommand(ExecuteImport);
        }

        public ICommand ExportCommand { get; }
        public ICommand ImportCommand { get; }

        public event EventHandler ImportCompleted;
        public event PropertyChangedEventHandler PropertyChanged;

        private void ExecuteExport()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Export Module",
                Filter = FileFilter,
                DefaultExt = FileExtension,
                FileName = _module.Name + FileExtension
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                MAC_use_casesSerializer.ExportModule(_module, saveFileDialog.FileName);
            }
        }

        private void ExecuteImport()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Import Module",
                Filter = FileFilter,
                DefaultExt = FileExtension
            };

            if (openFileDialog.ShowDialog() == true)
            {
                if (MAC_use_casesSerializer.ImportModuleFromFile(_module, openFileDialog.FileName))
                {
                    ImportCompleted?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
