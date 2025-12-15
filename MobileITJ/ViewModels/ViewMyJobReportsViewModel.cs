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

        public ObservableCollection<WorkerDetail> ActiveWorkers { get; } = new ObservableCollection<WorkerDetail>();
        public ObservableCollection<WorkerReport> MyFiledWorkerReports { get; } = new ObservableCollection<WorkerReport>();

        public Command LoadDataCommand { get; }
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
                ActiveWorkers.Clear();
                var allWorkers = await _auth.GetAllWorkersAsync();
                foreach (var worker in allWorkers.Where(w => w.IsActive))
                {
                    ActiveWorkers.Add(worker);
                }

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

        private async Task OnFileReportAsync(WorkerDetail worker)
        {
            if (worker == null) return;

            // 1. Get the list of jobs where this worker was hired by the current customer
            var myJobsWithWorkers = await _auth.GetMyJobsWithWorkerAsync();

            // 2. Find the job associated with this specific worker
            // (We assume the latest job if multiple exist)
            var jobLink = myJobsWithWorkers.FirstOrDefault(j => j.WorkerName == worker.FullName);

            // If no job is found, we can't file a specific report (or we create a dummy job reference if allowed)
            if (jobLink == null || jobLink.Job == null)
            {
                await _popupService.DisplayAlert("Error", "You can only report workers you have hired.", "OK");
                return;
            }

            string reportMessage = await _popupService.DisplayPrompt(
                "File Report",
                $"Please describe the issue with {worker.FullName} regarding the job '{jobLink.Job.JobDescription}'.",
                "Submit Report", "Cancel", "e.g., Worker was unprofessional...");

            if (string.IsNullOrWhiteSpace(reportMessage))
                return;

            // 3. FIX: Now we pass THREE arguments: Worker, Job, and Message
            var (success, message) = await _auth.FileWorkerReportAsync(worker, jobLink.Job, reportMessage);

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