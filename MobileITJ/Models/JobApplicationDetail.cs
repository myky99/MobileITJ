using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MobileITJ.Models
{
    public class JobApplicationDetail : INotifyPropertyChanged
    {
        private ApplicationStatus _status;
        private bool _isRated;
        private int _rating;
        private string _review = "";
        private TimeSpan _totalTimeSpent;
        private bool _isPaid;

        // --- 👇 ADD THESE TWO NEW PROPERTIES 👇 ---
        private bool _isClockedIn;
        private JobStatus _jobStatus;
        // --- END OF NEW ---

        public int ApplicationId { get; set; }
        public int JobId { get; set; }
        public int WorkerUserId { get; set; }

        public string WorkerName { get; set; } = "";
        public decimal NegotiatedRate { get; set; }

        public ApplicationStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public bool IsRated
        {
            get => _isRated;
            set => SetProperty(ref _isRated, value);
        }

        public int Rating
        {
            get => _rating;
            set => SetProperty(ref _rating, value);
        }

        public string Review
        {
            get => _review;
            set => SetProperty(ref _review, value);
        }

        public TimeSpan TotalTimeSpent
        {
            get => _totalTimeSpent;
            set => SetProperty(ref _totalTimeSpent, value, nameof(TotalPay));
        }

        public bool IsPaid
        {
            get => _isPaid;
            set => SetProperty(ref _isPaid, value);
        }

        // --- 👇 ADD THESE NEW GETTERS/SETTERS 👇 ---
        public bool IsClockedIn
        {
            get => _isClockedIn;
            set => SetProperty(ref _isClockedIn, value);
        }

        public JobStatus JobStatus
        {
            get => _jobStatus;
            set => SetProperty(ref _jobStatus, value);
        }
        // --- END OF NEW ---

        public double TotalPay => TotalTimeSpent.TotalHours * (double)NegotiatedRate;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", params string[] otherProperties)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value)) return false;
            backingStore = value;
            OnPropertyChanged(propertyName);
            foreach (var prop in otherProperties)
            {
                OnPropertyChanged(prop);
            }
            return true;
        }
    }
}