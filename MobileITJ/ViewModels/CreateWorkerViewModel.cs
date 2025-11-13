using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Services;
using MobileITJ.Models;
using System.Linq;
using System.Collections.Generic;

namespace MobileITJ.ViewModels
{
    public class CreateWorkerViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;

        private string _firstName = "";
        private string _lastName = "";
        private string _email = "";
        private string _skillSetText = "";
        private decimal _ratePerHour;

        public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }
        public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }
        public string Email { get => _email; set => SetProperty(ref _email, value); }
        public string SkillSetText { get => _skillSetText; set => SetProperty(ref _skillSetText, value); }
        public decimal RatePerHour { get => _ratePerHour; set => SetProperty(ref _ratePerHour, value); }

        // --- 👇 We no longer need these properties 👇 ---
        // private string _generatedWorkerId = "";
        // private string _generatedTempPassword = "";
        // private bool _isWorkerCreated = false;
        // public string GeneratedWorkerId { ... }
        // public string GeneratedTempPassword { ... }
        // public bool IsWorkerCreated { ... }
        // --- END OF REMOVAL ---

        public Command SubmitWorkerCommand { get; }
        public Command NavigateCreateWorkerCommand { get; }
        public Command NavigateViewWorkersCommand { get; }
        public Command NavigateJobReportsCommand { get; }
        public Command NavigateCustomersCommand { get; }

        public CreateWorkerViewModel(IAuthenticationService auth)
        {
            _auth = auth;

            SubmitWorkerCommand = new Command(async () => await OnCreateWorkerAsync());

            NavigateCreateWorkerCommand = new Command(async () => await Shell.Current.GoToAsync("../CreateWorkerPage"));
            NavigateViewWorkersCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewWorkersPage"));
            NavigateJobReportsCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewJobsReportPage"));
            NavigateCustomersCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewCustomersPage"));
        }

        private async Task OnCreateWorkerAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            ErrorMessage = "";
            // IsWorkerCreated = false; // No longer needed

            try
            {
                var skills = _skillSetText.Split(',')
                                          .Select(s => s.Trim())
                                          .Where(s => !string.IsNullOrEmpty(s))
                                          .ToList();

                if (skills.Count == 0)
                {
                    ErrorMessage = "Please enter at least one skill.";
                    IsBusy = false;
                    return;
                }

                var (success, message, workerId, tempPassword) = await _auth.CreateWorkerAsync(FirstName, LastName, Email, skills, RatePerHour);

                if (success)
                {
                    // --- 👇 THIS IS THE NEW POP-UP LOGIC 👇 ---
                    string popupMessage = $"The worker's email is: {Email}\nTemporary Password is: {tempPassword}";
                    await Application.Current.MainPage.DisplayAlert("Worker Created", popupMessage, "OK");

                    // Clear the form
                    FirstName = "";
                    LastName = "";
                    Email = "";
                    SkillSetText = "";
                    RatePerHour = 0;
                    // --- END OF NEW LOGIC ---
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