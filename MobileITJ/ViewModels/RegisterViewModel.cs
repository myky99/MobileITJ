using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Models;
using MobileITJ.Services;

namespace MobileITJ.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;
        private string _firstName = "";
        private string _lastName = "";
        private string _email = "";
        private string _password = "";
        private UserType _selectedUserType = UserType.Customer;
        private bool _isUserTypeSelected = false;

        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public UserType SelectedUserType
        {
            get => _selectedUserType;
            set => SetProperty(ref _selectedUserType, value);
        }

        public bool IsUserTypeSelected
        {
            get => _isUserTypeSelected;
            set => SetProperty(ref _isUserTypeSelected, value);
        }

        public Command<string> SelectUserTypeCommand { get; }
        public Command RegisterCommand { get; }
        public Command NavigateLoginCommand { get; }
        public Command BackToSelectionCommand { get; }

        public RegisterViewModel(IAuthenticationService auth)
        {
            _auth = auth;
            SelectUserTypeCommand = new Command<string>(OnSelectUserType);
            RegisterCommand = new Command(async () => await OnRegisterAsync());
            NavigateLoginCommand = new Command(async () => await Shell.Current.GoToAsync("//LoginPage"));
            BackToSelectionCommand = new Command(OnBackToSelection);
        }

        private void OnSelectUserType(string userType)
        {
            ErrorMessage = "";
            
            switch (userType?.ToLower())
            {
                case "customer":
                    SelectedUserType = UserType.Customer;
                    break;
                case "worker":
                    SelectedUserType = UserType.Worker;
                    break;
                case "hr":
                    SelectedUserType = UserType.HR;
                    break;
                default:
                    return;
            }

            IsUserTypeSelected = true;
        }

        private void OnBackToSelection()
        {
            IsUserTypeSelected = false;
            ErrorMessage = "";
            // Clear form fields
            FirstName = "";
            LastName = "";
            Email = "";
            Password = "";
        }

        private async Task OnRegisterAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            ErrorMessage = "";

            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
                {
                    ErrorMessage = "Please enter your first and last name.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(Email))
                {
                    ErrorMessage = "Please enter your email.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Please enter a password.";
                    return;
                }

                if (Password.Length < 6)
                {
                    ErrorMessage = "Password must be at least 6 characters.";
                    return;
                }

                // Use SelectedUserType
                var (success, message) = await _auth.RegisterAsync(FirstName, LastName, Email, Password, SelectedUserType);

                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "Account created successfully! Please login.", "OK");
                    await Shell.Current.GoToAsync("//LoginPage");
                }
                else
                {
                    ErrorMessage = message;
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}   