using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Models;
using MobileITJ.Services;
using System;

namespace MobileITJ.ViewModels
{
    public class ViewMyJobsViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;
        private readonly IPopupService _popupService; // 👈 --- ADD THIS ---

        public ObservableCollection<CustomerJobDetail> Jobs { get; } = new ObservableCollection<CustomerJobDetail>();

        public Command LoadJobsCommand { get; }
        public Command<CustomerJobDetail> ViewApplicationsCommand { get; }
        public Command<CustomerJobDetail> CompleteJobCommand { get; } // 👈 --- ADD THIS ---

        public Command NavigateCreateJobCommand { get; }
        public Command NavigateViewMyJobsCommand { get; }
        public Command NavigateRateWorkerCommand { get; }
        public Command NavigateViewReportsCommand { get; }

        public ViewMyJobsViewModel(IAuthenticationService auth, IPopupService popupService) // 👈 --- INJECT SERVICE ---
        {
            _auth = auth;
            _popupService = popupService; // 👈 --- ADD THIS ---

            LoadJobsCommand = new Command(async () => await OnLoadJobsAsync());
            ViewApplicationsCommand = new Command<CustomerJobDetail>(async (jobDetail) => await OnViewApplicationsAsync(jobDetail));
            CompleteJobCommand = new Command<CustomerJobDetail>(async (jobDetail) => await OnCompleteJobAsync(jobDetail)); // 👈 --- ADD THIS ---

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
                Jobs.Clear();
                var myJobs = await _auth.GetMyCustomerJobsAsync();
                foreach (var job in myJobs)
                {
                    Jobs.Add(job);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnViewApplicationsAsync(CustomerJobDetail jobDetail)
        {
            if (jobDetail == null) return;
            await Shell.Current.GoToAsync($"ViewJobApplicationsPage?jobId={jobDetail.Job.Id}");
        }

        // --- 👇 ADD THIS NEW METHOD 👇 ---
        private async Task OnCompleteJobAsync(CustomerJobDetail jobDetail)
        {
            if (jobDetail == null) return;

            var (success, message) = await _auth.CustomerCompleteJobAsync(jobDetail.Job.Id);

            if (success)
            {
                await _popupService.DisplayAlert("Success", message, "OK");
                // Refresh the job status
                jobDetail.Status = JobStatus.Completed;
                jobDetail.CanCompleteJob = false;
            }
            else
            {
                await _popupService.DisplayAlert("Error", message, "OK");
            }
        }
        // --- END OF NEW ---
    }
}