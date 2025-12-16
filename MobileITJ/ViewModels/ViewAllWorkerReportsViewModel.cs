using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Models;
using MobileITJ.Services;
using System.Linq;

namespace MobileITJ.ViewModels
{
    public class ViewAllWorkerReportsViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;

        // 👇 STATISTICAL PROPERTIES
        private int _totalWorkers;
        public int TotalWorkers { get => _totalWorkers; set => SetProperty(ref _totalWorkers, value); }

        private int _totalJobs;
        public int TotalJobs { get => _totalJobs; set => SetProperty(ref _totalJobs, value); }

        private decimal _totalPayouts;
        public decimal TotalPayouts { get => _totalPayouts; set => SetProperty(ref _totalPayouts, value); }

        // List of all reports (complaints)
        public ObservableCollection<WorkerReport> AllReports { get; } = new ObservableCollection<WorkerReport>();

        public Command LoadStatsCommand { get; }
        public Command NavigateBackCommand { get; }

        public ViewAllWorkerReportsViewModel(IAuthenticationService auth)
        {
            _auth = auth;
            LoadStatsCommand = new Command(async () => await OnLoadStatsAsync());
            NavigateBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        }

        public async Task OnAppearing()
        {
            await OnLoadStatsAsync();
        }

        private async Task OnLoadStatsAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                // 1. Get Workers Count
                var workers = await _auth.GetAllWorkersAsync();
                TotalWorkers = workers.Count;

                // 2. Get Jobs Count (using the new method)
                var jobs = await _auth.GetAllJobsAsync();
                TotalJobs = jobs.Count;

                // 3. Get Total Payouts (using the new method)
                var allTransactions = await _auth.GetAllTransactionsAsync();
                TotalPayouts = allTransactions.Sum(t => t.AmountPaid);

                // 4. Load Reports
                AllReports.Clear();
                var reports = await _auth.GetAllWorkerReportsAsync();
                foreach (var r in reports) AllReports.Add(r);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}