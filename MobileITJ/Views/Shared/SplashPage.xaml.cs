using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using MobileITJ.Services; // 👈 --- ADD THIS ---

namespace MobileITJ.Views.Shared
{
    public partial class SplashPage : ContentPage
    {
        private readonly IAuthenticationService _authService; // 👈 --- ADD THIS ---

        // 👇 --- MODIFY THE CONSTRUCTOR --- 👇
        public SplashPage(IAuthenticationService authService)
        {
            InitializeComponent();
            _authService = authService; // 👈 --- ADD THIS ---
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // This now loads your json files asynchronously
            await _authService.InitializeAsync();

            // Go to LoginPage
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}