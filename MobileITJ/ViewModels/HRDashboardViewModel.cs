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

            // --- 👇 ALL 4 ROUTES ARE NOW FIXED (Relative Navigation) 👇 ---
            CreateWorkerCommand = new Command(async () => await Shell.Current.GoToAsync("CreateWorkerPage"));
            ViewWorkersCommand = new Command(async () => await Shell.Current.GoToAsync("ViewWorkersPage"));
            ViewJobReportsCommand = new Command(async () => await Shell.Current.GoToAsync("ViewJobsReportPage"));
            ViewCustomersCommand = new Command(async () => await Shell.Current.GoToAsync("ViewCustomersPage"));
            // --- END OF FIX ---

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
            await _auth.LogoutAsync();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}