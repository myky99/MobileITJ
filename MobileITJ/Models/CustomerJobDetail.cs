using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MobileITJ.Models
{
    // This model combines Job with its application status
    public class CustomerJobDetail : INotifyPropertyChanged
    {
        private Job _job;
        public Job Job
        {
            get => _job;
            set => SetProperty(ref _job, value);
        }

        private string _slotsDisplay;
        public string SlotsDisplay
        {
            get => _slotsDisplay;
            set => SetProperty(ref _slotsDisplay, value);
        }

        // --- 👇 ADD THESE NEW PROPERTIES 👇 ---
        private bool _canCompleteJob;
        public bool CanCompleteJob
        {
            get => _canCompleteJob;
            set => SetProperty(ref _canCompleteJob, value);
        }

        private JobStatus _status;
        public JobStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }
        // --- END OF NEW ---

        // --- INotifyPropertyChanged Implementation ---
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value)) return false;
            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}