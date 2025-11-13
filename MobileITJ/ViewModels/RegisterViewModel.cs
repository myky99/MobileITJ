using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Models;
using MobileITJ.Services;
using System.Collections.ObjectModel;

namespace MobileITJ.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";

        public UserType SelectedUserType { get; set; } = UserType.Customer;
        public ObservableCollection<UserType> UserTypes { get; } = new ObservableCollection<UserType>() { UserType.Customer, UserType.Worker, UserType.HR };

        public Command RegisterCommand { get; }
        public Command NavigateLoginCommand { get; }

        public RegisterViewModel(IAuthenticationService auth)
        {
            _auth = auth;
            RegisterCommand = new Command(async () => await OnRegisterAsync());

            // --- 👇 THIS IS THE FIX 👇 ---
            NavigateLoginCommand = new Command(async () => await Shell.Current.GoToAsync("//LoginPage"));
            // --- END OF FIX ---
        }

        private async Task OnRegisterAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            ErrorMessage = "";

            try
            {
                var (success, message) = await _auth.RegisterAsync(FirstName, LastName, Email, Password, UserType.Customer);

                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "Account created successfully! Please login.", "OK");

                    // --- 👇 THIS IS THE FIX 👇 ---
                    await Shell.Current.GoToAsync("//LoginPage");
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
}