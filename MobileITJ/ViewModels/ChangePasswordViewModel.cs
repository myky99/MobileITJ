using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Services;
using MobileITJ.Models;

namespace MobileITJ.ViewModels;
public class ChangePasswordViewModel : BaseViewModel
{
    private readonly IAuthenticationService _auth;
    private string _currentPassword = "";
    private string _newPassword = "";

    public string CurrentPassword { get => _currentPassword; set => SetProperty(ref _currentPassword, value); }
    public string NewPassword { get => _newPassword; set => SetProperty(ref _newPassword, value); }

    public Command ChangeCommand { get; }

    public ChangePasswordViewModel(IAuthenticationService auth)
    {
        _auth = auth;
        ChangeCommand = new Command(async () => await OnChangeAsync());
    }

    private async Task OnChangeAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = "";

        try
        {
            var user = await _auth.GetCurrentUserAsync();
            if (user == null)
            {
                ErrorMessage = "Not logged in";
                IsBusy = false;
                return;
            }

            var (success, message) = await _auth.ChangePasswordAsync(user.Id, CurrentPassword, NewPassword);

            if (success)
            {
                await Application.Current.MainPage.DisplayAlert("Success", message, "OK");

                CurrentPassword = "";
                NewPassword = "";

                // --- 👇 THIS IS THE FIX 👇 ---
                // If it's a worker, send them to their dashboard.
                if (user.UserType == UserType.Worker)
                {
                    await Shell.Current.GoToAsync("//WorkerDashboardPage");
                }
                else
                {
                    // For any other user, just go back.
                    await Shell.Current.GoToAsync("..");
                }
                // --- END OF FIX ---
            }
            else
            {
                ErrorMessage = message;
            }
        }
        finally { IsBusy = false; }
    }
}