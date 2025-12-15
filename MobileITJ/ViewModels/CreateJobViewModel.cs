using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Services;
using MobileITJ.Models;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MobileITJ.ViewModels
{
    public class CreateJobViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;

        // 👇 NEW: Job Title Variable
        private string _jobTitle = "";

        private string _jobDescription = "";
        private string _location = "";
        private decimal? _ratePerHour;
        private int? _workersNeeded;

        public ObservableCollection<string> AvailableSkills { get; } = new ObservableCollection<string>();

        private string _selectedSkill;
        public string SelectedSkill
        {
            get => _selectedSkill;
            set
            {
                SetProperty(ref _selectedSkill, value);
                IsCustomSkillVisible = value == "Something Else";
            }
        }

        private string _customSkillEntry;
        public string CustomSkillEntry { get => _customSkillEntry; set => SetProperty(ref _customSkillEntry, value); }

        private bool _isCustomSkillVisible;
        public bool IsCustomSkillVisible { get => _isCustomSkillVisible; set => SetProperty(ref _isCustomSkillVisible, value); }

        // 👇 NEW: Public Property for Binding
        public string JobTitle { get => _jobTitle; set => SetProperty(ref _jobTitle, value); }

        public string JobDescription { get => _jobDescription; set => SetProperty(ref _jobDescription, value); }
        public string Location { get => _location; set => SetProperty(ref _location, value); }
        public decimal? RatePerHour { get => _ratePerHour; set => SetProperty(ref _ratePerHour, value); }
        public int? WorkersNeeded { get => _workersNeeded; set => SetProperty(ref _workersNeeded, value); }

        public Command SubmitJobCommand { get; }

        // Navigation commands...
        public Command NavigateCreateJobCommand { get; }
        public Command NavigateViewMyJobsCommand { get; }
        public Command NavigateRateWorkerCommand { get; }
        public Command NavigateViewReportsCommand { get; }

        public CreateJobViewModel(IAuthenticationService auth)
        {
            _auth = auth;
            SubmitJobCommand = new Command(async () => await OnSubmitJobAsync());

            NavigateCreateJobCommand = new Command(async () => await Shell.Current.GoToAsync("../CreateJobPage"));
            NavigateViewMyJobsCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewMyJobsPage"));
            NavigateRateWorkerCommand = new Command(async () => await Shell.Current.GoToAsync("../RateWorkerPage"));
            NavigateViewReportsCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewMyJobReportsPage"));

            Task.Run(async () => await LoadCategoriesAsync());
        }

        private async Task LoadCategoriesAsync()
        {
            var cats = await _auth.GetSkillCategoriesAsync();
            if (cats != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    AvailableSkills.Clear();
                    foreach (var c in cats) AvailableSkills.Add(c);
                });
            }
        }

        private async Task OnSubmitJobAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            ErrorMessage = "";

            try
            {
                string finalSkill = SelectedSkill;
                if (string.IsNullOrEmpty(finalSkill))
                {
                    ErrorMessage = "Please select a required skill.";
                    IsBusy = false;
                    return;
                }
                if (finalSkill == "Something Else")
                {
                    if (string.IsNullOrWhiteSpace(CustomSkillEntry))
                    {
                        ErrorMessage = "Please specify the skill.";
                        IsBusy = false;
                        return;
                    }
                    finalSkill = CustomSkillEntry.Trim();
                }

                // 👇 UPDATED VALIDATION: Check JobTitle
                if (string.IsNullOrWhiteSpace(JobTitle) ||
                    string.IsNullOrWhiteSpace(JobDescription) ||
                    string.IsNullOrWhiteSpace(Location) ||
                    WorkersNeeded == null || WorkersNeeded <= 0 ||
                    RatePerHour == null || RatePerHour <= 0)
                {
                    ErrorMessage = "Please fill in all fields with valid values.";
                    IsBusy = false;
                    return;
                }

                var newJob = new Job
                {
                    // 👇 SAVE TITLE
                    Title = JobTitle,
                    JobDescription = JobDescription,
                    Location = Location,
                    RatePerHour = RatePerHour.Value,
                    WorkersNeeded = WorkersNeeded.Value,
                    SkillsNeeded = new List<string> { finalSkill }
                };

                var (success, message) = await _auth.CreateJobAsync(newJob);

                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "Job posted successfully!", "OK");

                    // Clear form
                    JobTitle = ""; // 👈 Clear Title
                    JobDescription = "";
                    Location = "";
                    SelectedSkill = null;
                    CustomSkillEntry = "";
                    RatePerHour = null;
                    WorkersNeeded = null;
                }
                else
                {
                    ErrorMessage = message;
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}