using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MobileITJ.Models
{
    public class JobApplication : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int WorkerUserId { get; set; }
        public decimal NegotiatedRate { get; set; }

        private ApplicationStatus _status = ApplicationStatus.Pending;
        public ApplicationStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public DateTime DateApplied { get; set; }

        private DateTime? _clockInTime;
        private TimeSpan _totalTimeSpent;

        public DateTime? ClockInTime
        {
            get => _clockInTime;
            set => SetProperty(ref _clockInTime, value);
        }

        public TimeSpan TotalTimeSpent
        {
            get => _totalTimeSpent;
            set => SetProperty(ref _totalTimeSpent, value);
        }

        // --- 👇 ADD THESE RATING & PAYMENT PROPERTIES 👇 ---
        private bool _isRated = false;
        private int _rating = 0;
        private string _review = "";
        private bool _isPaid = false;

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

        public bool IsPaid
        {
            get => _isPaid;
            set => SetProperty(ref _isPaid, value);
        }
        // --- END OF NEW ---


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