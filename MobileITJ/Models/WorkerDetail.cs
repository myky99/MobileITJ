using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MobileITJ.Models
{
    // This model combines User and WorkerProfile data for the UI
    public class WorkerDetail : INotifyPropertyChanged
    {
        private bool _isActive;
        private List<string> _skills = new List<string>();

        public int UserId { get; set; }
        public string WorkerId { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public decimal RatePerHour { get; set; }

        public string FullName => $"{FirstName} {LastName}";
        public string SkillsDisplay => string.Join(", ", _skills);

        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        public List<string> Skills
        {
            get => _skills;
            set
            {
                if (SetProperty(ref _skills, value))
                {
                    // When Skills list changes, also notify that SkillsDisplay needs to update
                    OnPropertyChanged(nameof(SkillsDisplay));
                }
            }
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