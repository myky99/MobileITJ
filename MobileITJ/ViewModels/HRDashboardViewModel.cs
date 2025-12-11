using MobileITJ.Services;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace MobileITJ.ViewModels
{
    public class HRDashboardViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;
        private string _welcomeText = "Welcome, HR!";
        public string WelcomeText { get => _welcomeText; set => SetProperty(ref _welcomeText, value); }

        public Command CreateWorkerCommand { get; }
        public Command ViewWorkersCommand { get; }
        public Command ViewJobReportsCommand { get; }
        public Command ViewCustomersCommand { get; }
        public Command ChangePasswordCommand { get; }
        public Command LogoutCommand { get; }

        public HRDashboardViewModel(IAuthenticationService auth)
        {
            _auth = auth;

            CreateWorkerCommand = new Command(async () => await Shell.Current.GoToAsync("CreateWorkerPage"));
            ViewWorkersCommand = new Command(async () => await Shell.Current.GoToAsync("ViewWorkersPage"));
            ViewJobReportsCommand = new Command(async () => await Shell.Current.GoToAsync("ViewJobsReportPage"));
            ViewCustomersCommand = new Command(async () => await Shell.Current.GoToAsync("ViewCustomersPage"));

            ChangePasswordCommand = new Command(async () => await Shell.Current.GoToAsync("ChangePasswordPage"));
            LogoutCommand = new Command(async () => await OnLogoutAsync());
        }

        public async Task OnAppearing()
        {
            var user = await _auth.GetCurrentUserAsync();
            WelcomeText = $"Welcome, {user?.FirstName ?? "HR"}!";
        }

        private async Task OnLogoutAsync()
        {
            // Confirm logout
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Logout", 
                "Are you sure you want to logout?", 
                "Yes", 
                "No");

            if (!confirm) return;

            // Perform logout
            await _auth.LogoutAsync();
            
            // Clear navigation stack and go to login
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}