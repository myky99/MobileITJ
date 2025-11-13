using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Services;
using MobileITJ.Models;
using System.Linq;

namespace MobileITJ.ViewModels
{
    public class CreateJobViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;

        private string _jobDescription = "";
        private string _location = "";
        private string _skillSetText = "";
        private decimal _ratePerHour;
        private int _workersNeeded;

        public string JobDescription { get => _jobDescription; set => SetProperty(ref _jobDescription, value); }
        public string Location { get => _location; set => SetProperty(ref _location, value); }
        public string SkillSetText { get => _skillSetText; set => SetProperty(ref _skillSetText, value); }
        public decimal RatePerHour { get => _ratePerHour; set => SetProperty(ref _ratePerHour, value); }
        public int WorkersNeeded { get => _workersNeeded; set => SetProperty(ref _workersNeeded, value); }

        public Command SubmitJobCommand { get; }

        // --- Navigation Commands for Tabs ---
        public Command NavigateCreateJobCommand { get; }
        public Command NavigateViewMyJobsCommand { get; }
        public Command NavigateRateWorkerCommand { get; }
        public Command NavigateViewReportsCommand { get; }

        public CreateJobViewModel(IAuthenticationService auth)
        {
            _auth = auth;
            SubmitJobCommand = new Command(async () => await OnSubmitJobAsync());

            // --- Tab Navigation ---
            NavigateCreateJobCommand = new Command(async () => await Shell.Current.GoToAsync("../CreateJobPage"));
            NavigateViewMyJobsCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewMyJobsPage"));
            NavigateRateWorkerCommand = new Command(async () => await Shell.Current.GoToAsync("../RateWorkerPage"));
            NavigateViewReportsCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewMyJobReportsPage"));
        }

        private async Task OnSubmitJobAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            ErrorMessage = "";

            try
            {
                var skills = _skillSetText.Split(',')
                                          .Select(s => s.Trim())
                                          .Where(s => !string.IsNullOrEmpty(s))
                                          .ToList();

                if (string.IsNullOrWhiteSpace(JobDescription) || string.IsNullOrWhiteSpace(Location) || WorkersNeeded <= 0 || RatePerHour <= 0)
                {
                    ErrorMessage = "Please fill in all fields with valid values.";
                    IsBusy = false;
                    return;
                }

                var newJob = new Job
                {
                    JobDescription = JobDescription,
                    Location = Location,
                    RatePerHour = RatePerHour,
                    WorkersNeeded = WorkersNeeded,
                    SkillsNeeded = skills
                };

                var (success, message) = await _auth.CreateJobAsync(newJob);

                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "Job posted successfully!", "OK");

                    // Clear the form
                    JobDescription = "";
                    Location = "";
                    SkillSetText = "";
                    RatePerHour = 0;
                    WorkersNeeded = 0;
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