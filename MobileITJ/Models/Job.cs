using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MobileITJ.Models
{
    public class Job : INotifyPropertyChanged
    {
        private int _id;
        private int _customerId;
        private string _jobDescription = "";
        private string _location = "";
        private decimal _ratePerHour;
        private List<string> _skillsNeeded = new List<string>();
        private int _workersNeeded;
        private DateTime _datePosted;
        private JobStatus _status = JobStatus.Open;

        // --- 👇 Rating properties have been REMOVED from here ---

        public int Id { get => _id; set => SetProperty(ref _id, value); }
        public int CustomerId { get => _customerId; set => SetProperty(ref _customerId, value); }
        public string JobDescription { get => _jobDescription; set => SetProperty(ref _jobDescription, value); }
        public string Location { get => _location; set => SetProperty(ref _location, value); }
        public decimal RatePerHour { get => _ratePerHour; set => SetProperty(ref _ratePerHour, value); }
        public List<string> SkillsNeeded { get => _skillsNeeded; set => SetProperty(ref _skillsNeeded, value); }
        public int WorkersNeeded { get => _workersNeeded; set => SetProperty(ref _workersNeeded, value); }
        public DateTime DatePosted { get => _datePosted; set => SetProperty(ref _datePosted, value); }
        public JobStatus Status { get => _status; set => SetProperty(ref _status, value); }


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