using MobileITJ.Services;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace MobileITJ.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;

        private string _email;
        public string Email { get => _email; set => SetProperty(ref _email, value); }

        private string _password;
        public string Password { get => _password; set => SetProperty(ref _password, value); }

        public Command LoginCommand { get; }
        public Command NavigateRegisterCommand { get; }

        public LoginViewModel(IAuthenticationService auth)
        {
            _auth = auth;
            LoginCommand = new Command(async () => await OnLoginAsync());
            NavigateRegisterCommand = new Command(async () => await Shell.Current.GoToAsync("//RegisterPage"));
        }

        private async Task OnLoginAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            // 1. Validate Input
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please enter email and password.", "OK");
                IsBusy = false;
                return;
            }

            // 2. Attempt Login
            var (success, message, userType, isFirstLogin) = await _auth.LoginAsync(Email, Password);

            if (!success)
            {
                await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
                IsBusy = false;
                return;
            }

            // 3. Handle First Time Login
            if (isFirstLogin)
            {
                await Application.Current.MainPage.DisplayAlert("Welcome", "This is your first login. Please change your password.", "OK");
                await Shell.Current.GoToAsync("ChangePasswordPage");
                IsBusy = false;
                return;
            }

            // 4. Initialize the Main App (Enable Tabs & Global Logout)
            Application.Current.MainPage = new AppShell(_auth);

            // Allow a tiny delay for the new Shell to initialize
            await Task.Delay(100);

            // 5. Get User Name for the Splash Screen
            var currentUser = await _auth.GetCurrentUserAsync();
            string userName = currentUser?.FirstName ?? "User";

            // 6. Determine the Final Destination (Dashboard)
            string targetDashboard = userType switch
            {
                Models.UserType.HR => "//HRDashboardPage",
                Models.UserType.Customer => "//CustomerDashboardPage",
                Models.UserType.Worker => "//WorkerDashboardPage",
                _ => "//LoginPage"
            };

            // 7. 👇 THE FIX: Go to WelcomePage FIRST, passing the Name and Next Route 👇
            // This makes the "Hello [Name]" screen appear before the dashboard.
            await Shell.Current.GoToAsync($"WelcomePage?userName={userName}&nextRoute={targetDashboard}");

            IsBusy = false;
        }
    }
}