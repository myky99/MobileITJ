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

        // --- 👇 NEW LIST TYPE 👇 ---
        public ObservableCollection<HrReportDetail> WorkerReports { get; } = new ObservableCollection<HrReportDetail>();
        public Command LoadReportsCommand { get; }
        // --- END OF NEW ---

        public Command NavigateCreateWorkerCommand { get; }
        public Command NavigateViewWorkersCommand { get; }
        public Command NavigateJobReportsCommand { get; }
        public Command NavigateCustomersCommand { get; }

        public ViewJobsReportViewModel(IAuthenticationService auth)
        {
            _auth = auth;
            LoadReportsCommand = new Command(async () => await OnLoadReportsAsync());

            NavigateCreateWorkerCommand = new Command(async () => await Shell.Current.GoToAsync("../CreateWorkerPage"));
            NavigateViewWorkersCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewWorkersPage"));
            NavigateJobReportsCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewJobsReportPage"));
            NavigateCustomersCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewCustomersPage"));
        }

        // --- 👇 ADD OnAppearing AND NEW LOGIC 👇 ---
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

                // 1. Get all workers
                var allWorkers = await _auth.GetAllWorkersAsync();

                // 2. Get all reports
                var allReports = await _auth.GetAllWorkerReportsAsync();

                // 3. Group reports by WorkerId
                var reportsByWorker = allReports
                    .GroupBy(r => r.WorkerUserId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // 4. Create the final list
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
        // --- END OF NEW ---
    }
}