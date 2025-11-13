using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Services;
using MobileITJ.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace MobileITJ.ViewModels
{
    public class ViewMyJobReportsViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;
        private readonly IPopupService _popupService;

        // --- 👇 FIX: This is a list of workers, not jobs 👇 ---
        public ObservableCollection<WorkerDetail> ActiveWorkers { get; } = new ObservableCollection<WorkerDetail>();
        public ObservableCollection<WorkerReport> MyFiledWorkerReports { get; } = new ObservableCollection<WorkerReport>();

        public Command LoadDataCommand { get; }
        // --- 👇 FIX: The command expects a WorkerDetail 👇 ---
        public Command<WorkerDetail> FileReportCommand { get; }

        public Command NavigateCreateJobCommand { get; }
        public Command NavigateViewMyJobsCommand { get; }
        public Command NavigateRateWorkerCommand { get; }
        public Command NavigateViewReportsCommand { get; }
        public Command LogoutCommand { get; }

        public ViewMyJobReportsViewModel(IAuthenticationService auth, IPopupService popupService)
        {
            _auth = auth;
            _popupService = popupService;
            LoadDataCommand = new Command(async () => await OnLoadDataAsync());
            // --- 👇 FIX: The command is created with WorkerDetail 👇 ---
            FileReportCommand = new Command<WorkerDetail>(async (worker) => await OnFileReportAsync(worker));

            LogoutCommand = new Command(async () => await OnLogoutAsync());

            NavigateCreateJobCommand = new Command(async () => await Shell.Current.GoToAsync("../CreateJobPage"));
            NavigateViewMyJobsCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewMyJobsPage"));
            NavigateRateWorkerCommand = new Command(async () => await Shell.Current.GoToAsync("../RateWorkerPage"));
            NavigateViewReportsCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewMyJobReportsPage"));
        }

        public async Task OnAppearing()
        {
            await OnLoadDataAsync();
        }

        private async Task OnLoadDataAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                // --- 👇 FIX: Load all active workers 👇 ---
                ActiveWorkers.Clear();
                var allWorkers = await _auth.GetAllWorkersAsync();
                foreach (var worker in allWorkers.Where(w => w.IsActive))
                {
                    ActiveWorkers.Add(worker);
                }
                // --- END OF FIX ---

                MyFiledWorkerReports.Clear();
                var myReports = await _auth.GetMyFiledWorkerReportsAsync();
                foreach (var report in myReports)
                {
                    MyFiledWorkerReports.Add(report);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        // --- 👇 FIX: The method signature now takes a WorkerDetail 👇 ---
        private async Task OnFileReportAsync(WorkerDetail worker)
        {
            if (worker == null) return;

            string reportMessage = await _popupService.DisplayPrompt(
                "File Report",
                $"Please describe the issue with {worker.FullName}.", // Now correctly uses worker name
                "Submit Report", "Cancel", "e.g., Worker was unprofessional...");

            if (string.IsNullOrWhiteSpace(reportMessage))
                return;

            // --- 👇 FIX: This call now passes the correct object type 👇 ---
            var (success, message) = await _auth.FileWorkerReportAsync(worker, reportMessage);

            if (success)
            {
                await _popupService.DisplayAlert("Report Filed", message, "OK");
                await OnLoadDataAsync();
            }
            else
            {
                await _popupService.DisplayAlert("Error", message, "OK");
            }
        }

        private async Task OnLogoutAsync()
        {
            await _auth.LogoutAsync();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}