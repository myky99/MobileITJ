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
        private readonly IPopupService _popupService;

        public ObservableCollection<CustomerJobDetail> Jobs { get; } = new ObservableCollection<CustomerJobDetail>();

        public Command LoadJobsCommand { get; }
        public Command<CustomerJobDetail> ViewApplicationsCommand { get; }
        public Command<CustomerJobDetail> CompleteJobCommand { get; }

        // 👇 NEW COMMAND
        public Command<CustomerJobDetail> MarkIncompleteCommand { get; }

        public Command NavigateCreateJobCommand { get; }
        public Command NavigateViewMyJobsCommand { get; }
        public Command NavigateRateWorkerCommand { get; }
        public Command NavigateViewReportsCommand { get; }

        public ViewMyJobsViewModel(IAuthenticationService auth, IPopupService popupService)
        {
            _auth = auth;
            _popupService = popupService;

            LoadJobsCommand = new Command(async () => await OnLoadJobsAsync());
            ViewApplicationsCommand = new Command<CustomerJobDetail>(async (jobDetail) => await OnViewApplicationsAsync(jobDetail));
            CompleteJobCommand = new Command<CustomerJobDetail>(async (jobDetail) => await OnCompleteJobAsync(jobDetail));

            // 👇 Initialize New Command
            MarkIncompleteCommand = new Command<CustomerJobDetail>(async (jobDetail) => await OnMarkIncompleteAsync(jobDetail));

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
            finally { IsBusy = false; }
        }

        private async Task OnViewApplicationsAsync(CustomerJobDetail jobDetail)
        {
            if (jobDetail == null) return;
            await Shell.Current.GoToAsync($"ViewJobApplicationsPage?jobId={jobDetail.Job.Id}");
        }

        private async Task OnCompleteJobAsync(CustomerJobDetail jobDetail)
        {
            if (jobDetail == null) return;

            bool confirm = await _popupService.DisplayAlert("Confirm", "Mark this job as successfully completed?", "Yes", "No");
            if (!confirm) return;

            var (success, message) = await _auth.CustomerCompleteJobAsync(jobDetail.Job.Id);

            if (success)
            {
                await _popupService.DisplayAlert("Success", message, "OK");
                jobDetail.Status = JobStatus.Completed;
                jobDetail.CanCompleteJob = false;
                // Force reload to update UI buttons immediately
                await OnLoadJobsAsync();
            }
            else
            {
                await _popupService.DisplayAlert("Error", message, "OK");
            }
        }

        // 👇 NEW: Logic for marking Incomplete
        private async Task OnMarkIncompleteAsync(CustomerJobDetail jobDetail)
        {
            if (jobDetail == null) return;

            // 1. Ask for reason
            string reason = await Application.Current.MainPage.DisplayPromptAsync("Mark Incomplete",
                "Why is this job incomplete? (e.g. Worker didn't show up)", "Submit", "Cancel");

            if (string.IsNullOrWhiteSpace(reason)) return; // Cancelled

            // 2. Call Service
            var (success, message) = await _auth.CustomerMarkJobIncompleteAsync(jobDetail.Job.Id, reason);

            if (success)
            {
                await _popupService.DisplayAlert("Reported", "Job marked as Incomplete.", "OK");
                jobDetail.Status = JobStatus.Incomplete;
                jobDetail.CanCompleteJob = false;
                await OnLoadJobsAsync();
            }
            else
            {
                await _popupService.DisplayAlert("Error", message, "OK");
            }
        }
    }
}