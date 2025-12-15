using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace MobileITJ.Models
{
    // This model combines Job and Application data for the UI
    public class MyJobDetail : INotifyPropertyChanged
    {
        private bool _isClockedIn;
        private TimeSpan _totalTimeSpent;

        public Job Job { get; set; }
        public int ApplicationId { get; set; }

        // 👇 NEW: Store the status (Pending/Accepted)
        public ApplicationStatus Status { get; set; }

        public bool IsClockedIn
        {
            get => _isClockedIn;
            set => SetProperty(ref _isClockedIn, value);
        }

        public TimeSpan TotalTimeSpent
        {
            get => _totalTimeSpent;
            set => SetProperty(ref _totalTimeSpent, value);
        }

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