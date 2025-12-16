using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Services; // Ensure this is here

namespace MobileITJ.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private bool _isBusy;
        private string _errorMessage = "";

        public bool IsBusy { get => _isBusy; set { _isBusy = value; OnPropertyChanged(); } }
        public string ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged(); } }

        // 👇 GLOBAL LOGOUT COMMAND (Inherited by all pages)
        public Command LogoutGlobalCommand { get; }

        public BaseViewModel()
        {
            LogoutGlobalCommand = new Command(async () => await OnLogoutGlobalAsync());
        }

        private async Task OnLogoutGlobalAsync()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
            if (!confirm) return;

            // 1. Get Auth Service dynamically (so we don't break your existing constructors)
            var authService = Application.Current.Handler.MauiContext.Services.GetService<IAuthenticationService>();

            if (authService != null)
            {
                await authService.LogoutAsync();
            }

            // 2. Navigate to Login
            await Shell.Current.GoToAsync("//LoginPage");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value)) return false;
            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}