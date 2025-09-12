using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MAC_use_cases.ViewModel
{
    public class RtuSettingsViewModel : INotifyPropertyChanged
    {
        private string _occupiedCoolingSetpoint = "24.0";
        private string _occupiedHeatingSetpoint = "21.0";
        private string _unoccupiedCoolingSetpoint = "28.0";
        private string _unoccupiedHeatingSetpoint = "18.0";
        private string _fanFailureDelay = "T#5s";
        private string _compressorMinRunTime = "T#3m";
        private string _compressorMinOffTime = "T#3m";
        private string _dirtyFilterDelay = "T#10s";
        private string _minFreshAirPosition = "20.0";
        private string _economizerTempDifferential = "2.0";

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string OccupiedCoolingSetpoint
        {
            get => _occupiedCoolingSetpoint;
            set { _occupiedCoolingSetpoint = value; OnPropertyChanged(); }
        }

        public string OccupiedHeatingSetpoint
        {
            get => _occupiedHeatingSetpoint;
            set { _occupiedHeatingSetpoint = value; OnPropertyChanged(); }
        }

        public string UnoccupiedCoolingSetpoint
        {
            get => _unoccupiedCoolingSetpoint;
            set { _unoccupiedCoolingSetpoint = value; OnPropertyChanged(); }
        }

        public string UnoccupiedHeatingSetpoint
        {
            get => _unoccupiedHeatingSetpoint;
            set { _unoccupiedHeatingSetpoint = value; OnPropertyChanged(); }
        }

        public string FanFailureDelay
        {
            get => _fanFailureDelay;
            set { _fanFailureDelay = value; OnPropertyChanged(); }
        }

        public string CompressorMinRunTime
        {
            get => _compressorMinRunTime;
            set { _compressorMinRunTime = value; OnPropertyChanged(); }
        }

        public string CompressorMinOffTime
        {
            get => _compressorMinOffTime;
            set { _compressorMinOffTime = value; OnPropertyChanged(); }
        }

        public string DirtyFilterDelay
        {
            get => _dirtyFilterDelay;
            set { _dirtyFilterDelay = value; OnPropertyChanged(); }
        }

        public string MinFreshAirPosition
        {
            get => _minFreshAirPosition;
            set { _minFreshAirPosition = value; OnPropertyChanged(); }
        }

        public string EconomizerTempDifferential
        {
            get => _economizerTempDifferential;
            set { _economizerTempDifferential = value; OnPropertyChanged(); }
        }
    }
}
