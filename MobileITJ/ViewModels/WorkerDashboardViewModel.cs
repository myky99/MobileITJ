using MobileITJ.Services;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace MobileITJ.ViewModels
{
    public class WorkerDashboardViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;
        private string _welcomeText = "Welcome, Worker!";
        public string WelcomeText { get => _welcomeText; set => SetProperty(ref _welcomeText, value); }

        public Command ViewAvailableJobsCommand { get; }
        public Command ViewOngoingJobsCommand { get; }
        public Command UpdateProfileCommand { get; }
        public Command ViewRatingsCommand { get; }
        public Command ChangePasswordCommand { get; }
        public Command LogoutCommand { get; }

        public WorkerDashboardViewModel(IAuthenticationService auth)
        {
            _auth = auth;

            // --- 👇 ALL 4 ROUTES ARE NOW FIXED (Relative Navigation) 👇 ---
            ViewAvailableJobsCommand = new Command(async () => await Shell.Current.GoToAsync("ViewAvailableJobsPage"));
            ViewOngoingJobsCommand = new Command(async () => await Shell.Current.GoToAsync("ViewOngoingJobsPage"));
            UpdateProfileCommand = new Command(async () => await Shell.Current.GoToAsync("UpdateWorkerProfilePage"));
            ViewRatingsCommand = new Command(async () => await Shell.Current.GoToAsync("ViewRatingsPage"));
            // --- END OF FIX ---

            ChangePasswordCommand = new Command(async () => await Shell.Current.GoToAsync("ChangePasswordPage"));
            LogoutCommand = new Command(async () => await OnLogoutAsync());
        }

        public async Task OnAppearing()
        {
            var user = await _auth.GetCurrentUserAsync();
            WelcomeText = $"Welcome, {user?.FirstName ?? "Worker"}!";
        }

        private async Task OnLogoutAsync()
        {
            await _auth.LogoutAsync();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}