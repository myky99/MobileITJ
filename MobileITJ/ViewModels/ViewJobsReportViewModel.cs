using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Models;
using MobileITJ.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace MobileITJ.ViewModels
{
    public class ViewJobsReportViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;

        public ObservableCollection<HrReportDetail> WorkerReports { get; } = new ObservableCollection<HrReportDetail>();
        public Command LoadReportsCommand { get; }
        public Command LogoutCommand { get; }

        public Command NavigateCreateWorkerCommand { get; }
        public Command NavigateViewWorkersCommand { get; }
        public Command NavigateJobReportsCommand { get; }
        public Command NavigateCustomersCommand { get; }

        public ViewJobsReportViewModel(IAuthenticationService auth)
        {
            _auth = auth;
            LoadReportsCommand = new Command(async () => await OnLoadReportsAsync());
            LogoutCommand = new Command(async () => await OnLogoutAsync());

            NavigateCreateWorkerCommand = new Command(async () => await Shell.Current.GoToAsync("../CreateWorkerPage"));
            NavigateViewWorkersCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewWorkersPage"));
            NavigateJobReportsCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewJobsReportPage"));
            NavigateCustomersCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewCustomersPage"));
        }

        public async Task OnAppearing()
        {
            await OnLoadReportsAsync();
        }

        private async Task OnLoadReportsAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                WorkerReports.Clear();

                var allWorkers = await _auth.GetAllWorkersAsync();
                var allReports = await _auth.GetAllWorkerReportsAsync();

                var reportsByWorker = allReports
                    .GroupBy(r => r.WorkerUserId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                foreach (var worker in allWorkers)
                {
                    var detail = new HrReportDetail { Worker = worker };

                    if (reportsByWorker.TryGetValue(worker.UserId, out var reports))
                    {
                        detail.Reports = reports;
                    }

                    WorkerReports.Add(detail);
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