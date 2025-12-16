using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Models;
using MobileITJ.Services;
using System.Collections.Generic;

namespace MobileITJ.ViewModels
{
    public class ViewWorkersViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;
        public ObservableCollection<WorkerDetail> Workers { get; } = new ObservableCollection<WorkerDetail>();

        public Command LoadWorkersCommand { get; }
        public Command<WorkerDetail> GoToDetailsCommand { get; } // New Command
        public Command LogoutCommand { get; }

        public Command NavigateCreateWorkerCommand { get; }
        public Command NavigateViewWorkersCommand { get; }
        public Command NavigateJobReportsCommand { get; }
        public Command NavigateCustomersCommand { get; }

        public ViewWorkersViewModel(IAuthenticationService auth)
        {
            _auth = auth;
            LoadWorkersCommand = new Command(async () => await OnLoadWorkersAsync());
            LogoutCommand = new Command(async () => await OnLogoutAsync());

            // Navigation Commands
            NavigateCreateWorkerCommand = new Command(async () => await Shell.Current.GoToAsync("../CreateWorkerPage"));
            NavigateViewWorkersCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewWorkersPage"));
            NavigateJobReportsCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewJobsReportPage"));
            NavigateCustomersCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewCustomersPage"));

            // New: Navigate to Details Page
            GoToDetailsCommand = new Command<WorkerDetail>(async (worker) => await OnGoToDetailsAsync(worker));
        }

        public async Task OnAppearing()
        {
            await OnLoadWorkersAsync();
        }

        private async Task OnLoadWorkersAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                Workers.Clear();
                var workers = await _auth.GetAllWorkersAsync();
                foreach (var worker in workers)
                {
                    Workers.Add(worker);
                }
            }
            finally { IsBusy = false; }
        }

        private async Task OnGoToDetailsAsync(WorkerDetail worker)
        {
            if (worker == null) return;

            // Pass the worker object to the new page using a Dictionary
            var navigationParameter = new Dictionary<string, object>
            {
                { "Worker", worker }
            };

            // Navigate to the registered route
            await Shell.Current.GoToAsync("WorkerDetailsPage", navigationParameter);
        }

        private async Task OnLogoutAsync()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert("Logout", "Are you sure?", "Yes", "No");
            if (!confirm) return;
            await _auth.LogoutAsync();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}