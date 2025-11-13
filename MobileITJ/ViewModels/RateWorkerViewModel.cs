using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Services;
using MobileITJ.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace MobileITJ.ViewModels
{
    public class RateWorkerViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;
        private readonly IPopupService _popupService; // 👈 --- ADD THIS ---

        public ObservableCollection<Job> CompletedJobs { get; } = new ObservableCollection<Job>();
        public Command LoadJobsCommand { get; }
        public Command<Job> SelectJobCommand { get; }

        public Command NavigateCreateJobCommand { get; }
        public Command NavigateViewMyJobsCommand { get; }
        public Command NavigateRateWorkerCommand { get; }
        public Command NavigateViewReportsCommand { get; }

        public RateWorkerViewModel(IAuthenticationService auth, IPopupService popupService) // 👈 --- INJECT SERVICE ---
        {
            _auth = auth;
            _popupService = popupService; // 👈 --- ADD THIS ---

            LoadJobsCommand = new Command(async () => await OnLoadJobsAsync());
            SelectJobCommand = new Command<Job>(async (job) => await OnSelectJobAsync(job));

            NavigateCreateJobCommand = new Command(async () => await Shell.Current.GoToAsync("../CreateJobPage"));
            NavigateViewMyJobsCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewMyJobsPage"));
            NavigateRateWorkerCommand = new Command(async () => await Shell.Current.GoToAsync("../RateWorkerPage"));
            NavigateViewReportsCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewMyJobReportsPage"));
        }

        public async Task OnAppearing()
        {
            await OnLoadJobsAsync();
        }

        private async Task OnLoadJobsAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                CompletedJobs.Clear();
                var myJobs = await _auth.GetMyJobsAsync();
                var completedJobs = myJobs.Where(j => j.Status == JobStatus.Completed);
                foreach (var job in completedJobs)
                {
                    CompletedJobs.Add(job);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnSelectJobAsync(Job job)
        {
            if (job == null) return;
            await Shell.Current.GoToAsync($"RateJobWorkersPage?jobId={job.Id}");
        }
    }
}