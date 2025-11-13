using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Services;
using MobileITJ.Models;
using System;

namespace MobileITJ.ViewModels;
public class LoginViewModel : BaseViewModel
{
    private readonly IAuthenticationService _auth;
    private string _email = "";
    private string _password = "";

    public string Email { get => _email; set => SetProperty(ref _email, value); }
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
        ErrorMessage = "";

        try
        {
            var (success, message, userType, isFirstLogin) = await _auth.LoginAsync(Email, Password);

            if (!success)
            {
                // --- 👇 THIS IS THE NEW LOGIC 👇 ---
                if (message == "Account deactivated.")
                {
                    await Shell.Current.GoToAsync("DeactivatedAccountPage");
                }
                else
                {
                    ErrorMessage = message;
                }
                // --- END OF NEW LOGIC ---

                IsBusy = false;
                return;
            }

            if (isFirstLogin)
            {
                await Application.Current.MainPage.DisplayAlert("Welcome!", "Please update your temporary password.", "OK");
                await Shell.Current.GoToAsync("ChangePasswordPage");
                IsBusy = false;
                return;
            }

            if (userType == null)
            {
                ErrorMessage = "User data missing after login.";
                IsBusy = false;
                return;
            }

            string userName = (await _auth.GetCurrentUserAsync())?.FirstName ?? "User";
            string nextRoute = "";

            switch (userType)
            {
                case UserType.Customer:
                    nextRoute = "//CustomerDashboardPage";
                    break;
                case UserType.Worker:
                    nextRoute = "//WorkerDashboardPage";
                    break;
                case UserType.HR:
                    nextRoute = "//HRDashboardPage";
                    break;
            }

            await Shell.Current.GoToAsync($"WelcomePage?userName={userName}&nextRoute={Uri.EscapeDataString(nextRoute)}");
        }
        finally { IsBusy = false; }
    }
}