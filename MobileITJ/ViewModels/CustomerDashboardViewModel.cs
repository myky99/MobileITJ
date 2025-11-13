using MobileITJ.Services;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace MobileITJ.ViewModels
{
    public class CustomerDashboardViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;
        private string _welcomeText = "Welcome, Customer!";
        public string WelcomeText { get => _welcomeText; set => SetProperty(ref _welcomeText, value); }

        public Command CreateJobCommand { get; }
        public Command ViewMyJobsCommand { get; }
        public Command RateWorkerCommand { get; }
        public Command ViewReportsCommand { get; }
        public Command ChangePasswordCommand { get; }
        public Command LogoutCommand { get; }

        public CustomerDashboardViewModel(IAuthenticationService auth)
        {
            _auth = auth;

            // --- 👇 ALL 4 ROUTES ARE NOW FIXED (Relative Navigation) 👇 ---
            CreateJobCommand = new Command(async () => await Shell.Current.GoToAsync("CreateJobPage"));
            ViewMyJobsCommand = new Command(async () => await Shell.Current.GoToAsync("ViewMyJobsPage"));
            RateWorkerCommand = new Command(async () => await Shell.Current.GoToAsync("RateWorkerPage"));
            ViewReportsCommand = new Command(async () => await Shell.Current.GoToAsync("ViewMyJobReportsPage"));
            // --- END OF FIX ---

            ChangePasswordCommand = new Command(async () => await Shell.Current.GoToAsync("ChangePasswordPage"));
            LogoutCommand = new Command(async () => await OnLogoutAsync());
        }

        public async Task OnAppearing()
        {
            var user = await _auth.GetCurrentUserAsync();
            WelcomeText = $"Welcome, {user?.FirstName ?? "Customer"}!";
        }

        private async Task OnLogoutAsync()
        {
            await _auth.LogoutAsync();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}