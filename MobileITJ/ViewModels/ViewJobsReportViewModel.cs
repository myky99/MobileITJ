using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Models;
using MobileITJ.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;

namespace MobileITJ.ViewModels
{
    public class ViewJobsReportViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;

        // 👇 1. ADDED STATS PROPERTIES (These were missing!)
        private int _totalWorkers;
        public int TotalWorkers { get => _totalWorkers; set => SetProperty(ref _totalWorkers, value); }

        private int _totalJobs;
        public int TotalJobs { get => _totalJobs; set => SetProperty(ref _totalJobs, value); }

        private decimal _totalPayouts;
        public decimal TotalPayouts { get => _totalPayouts; set => SetProperty(ref _totalPayouts, value); }
        // 👆 END STATS

        public ObservableCollection<HrReportDetail> WorkerReports { get; } = new ObservableCollection<HrReportDetail>();

        public Command LoadReportsCommand { get; }
        public Command<HrReportDetail> ToggleExpandCommand { get; }
        public Command LogoutCommand { get; }

        // Navigation Commands
        public Command NavigateCreateWorkerCommand { get; }
        public Command NavigateViewWorkersCommand { get; }
        public Command NavigateJobReportsCommand { get; }
        public Command NavigateCustomersCommand { get; }

        public ViewJobsReportViewModel(IAuthenticationService auth)
        {
            _auth = auth;
            LoadReportsCommand = new Command(async () => await OnLoadReportsAsync());

            // Logic: Flip the IsExpanded switch
            ToggleExpandCommand = new Command<HrReportDetail>((item) =>
            {
                if (item != null)
                {
                    item.IsExpanded = !item.IsExpanded;
                }
            });

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
                // 👇 2. ADDED CALCULATION LOGIC HERE

                // A. Calculate Total Workers
                var allWorkers = await _auth.GetAllWorkersAsync();
                TotalWorkers = allWorkers.Count;

                // B. Calculate Total Jobs (Using the new Service method)
                var allJobs = await _auth.GetAllJobsAsync();
                TotalJobs = allJobs.Count;

                // C. Calculate Total Payouts (Using the new Service method)
                var allTransactions = await _auth.GetAllTransactionsAsync();
                TotalPayouts = allTransactions.Sum(t => t.AmountPaid);

                // 👆 END CALCULATION LOGIC

                // D. Load Reports List (Your existing logic)
                WorkerReports.Clear();
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
            bool confirm = await Application.Current.MainPage.DisplayAlert("Logout", "Are you sure?", "Yes", "No");
            if (!confirm) return;
            await _auth.LogoutAsync();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}