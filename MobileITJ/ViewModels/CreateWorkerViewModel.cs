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

        public Command SubmitWorkerCommand { get; }
        public Command LogoutCommand { get; }
        public Command NavigateCreateWorkerCommand { get; }
        public Command NavigateViewWorkersCommand { get; }
        public Command NavigateJobReportsCommand { get; }
        public Command NavigateCustomersCommand { get; }

        public CreateWorkerViewModel(IAuthenticationService auth)
        {
            _auth = auth;

            SubmitWorkerCommand = new Command(async () => await OnCreateWorkerAsync());
            LogoutCommand = new Command(async () => await OnLogoutAsync());

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

            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
                {
                    ErrorMessage = "Please enter worker's first and last name.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(Email))
                {
                    ErrorMessage = "Please enter worker's email.";
                    return;
                }

                if (RatePerHour <= 0)
                {
                    ErrorMessage = "Please enter a valid rate per hour.";
                    return;
                }

                var skills = _skillSetText.Split(',')
                                          .Select(s => s.Trim())
                                          .Where(s => !string.IsNullOrEmpty(s))
                                          .ToList();

                if (skills.Count == 0)
                {
                    ErrorMessage = "Please enter at least one skill.";
                    return;
                }

                var (success, message, workerId, tempPassword) = await _auth.CreateWorkerAsync(FirstName, LastName, Email, skills, RatePerHour);

                if (success)
                {
                    string popupMessage = $"Worker ID: {workerId}\nEmail: {Email}\nTemporary Password: {tempPassword}\n\nPlease provide these credentials to the worker.";
                    await Application.Current.MainPage.DisplayAlert("Worker Created Successfully!", popupMessage, "OK");

                    // Clear the form
                    FirstName = "";
                    LastName = "";
                    Email = "";
                    SkillSetText = "";
                    RatePerHour = 0;
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

        private async Task OnLogoutAsync()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Logout", 
                "Are you sure you want to logout?", 
                "Yes", 
                "No");

            if (!confirm) return;

            await _auth.LogoutAsync();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}