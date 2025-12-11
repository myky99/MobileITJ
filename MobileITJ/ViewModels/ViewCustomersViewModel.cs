using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Services;
using MobileITJ.Models;
using System.Collections.ObjectModel;

namespace MobileITJ.ViewModels
{
    public class ViewCustomersViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;
        public ObservableCollection<User> Customers { get; } = new ObservableCollection<User>();
        public Command LoadCustomersCommand { get; }
        public Command LogoutCommand { get; }

        public Command NavigateCreateWorkerCommand { get; }
        public Command NavigateViewWorkersCommand { get; }
        public Command NavigateJobReportsCommand { get; }
        public Command NavigateCustomersCommand { get; }

        public ViewCustomersViewModel(IAuthenticationService auth)
        {
            _auth = auth;
            LoadCustomersCommand = new Command(async () => await OnLoadCustomersAsync());
            LogoutCommand = new Command(async () => await OnLogoutAsync());

            NavigateCreateWorkerCommand = new Command(async () => await Shell.Current.GoToAsync("../CreateWorkerPage"));
            NavigateViewWorkersCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewWorkersPage"));
            NavigateJobReportsCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewJobsReportPage"));
            NavigateCustomersCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewCustomersPage"));
        }

        public async Task OnAppearing()
        {
            await OnLoadCustomersAsync();
        }

        private async Task OnLoadCustomersAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                Customers.Clear();
                var customers = await _auth.GetAllCustomersAsync();
                foreach (var customer in customers)
                {
                    Customers.Add(customer);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnLogoutAsync()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Logout", 
                "Are you sure you want to logout?", 
                "Yes", 
                "No");

            if (!confirm) return;

            await _auth.LogoutAsync();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}