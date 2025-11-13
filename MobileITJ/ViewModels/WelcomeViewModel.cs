using Microsoft.Maui.Controls;
using MobileITJ.ViewModels;
using System.Threading.Tasks;

namespace MobileITJ.ViewModels
{
    // These attributes tell the page how to receive
    // parameters from the Login page
    [QueryProperty(nameof(UserName), "userName")]
    [QueryProperty(nameof(NextRoute), "nextRoute")]
    public class WelcomeViewModel : BaseViewModel
    {
        private string _userName = "User";
        private string _nextRoute = "//LoginPage"; // A safe fallback
        private string _welcomeMessage = "Welcome!";

        public string UserName
        {
            get => _userName;
            set
            {
                // When UserName is set, update the WelcomeMessage
                SetProperty(ref _userName, value);
                WelcomeMessage = $"Welcome, {_userName}!";
            }
        }

        // This property will hold the route we need to go to next
        // (e.g., "//CustomerDashboardPage")
        public string NextRoute { get => _nextRoute; set => SetProperty(ref _nextRoute, value); }

        public string WelcomeMessage { get => _welcomeMessage; set => SetProperty(ref _welcomeMessage, value); }

        public Command ContinueCommand { get; }

        public WelcomeViewModel()
        {
            ContinueCommand = new Command(async () => await OnContinueAsync());
        }

        private async Task OnContinueAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            // Navigate to the correct dashboard (that was passed in)
            await Shell.Current.GoToAsync(NextRoute);

            IsBusy = false;
        }
    }
}