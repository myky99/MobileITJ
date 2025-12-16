using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Models;
using MobileITJ.Services;
using System;
using System.Linq;

namespace MobileITJ.ViewModels
{
    public class ViewOngoingJobsViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;
        private readonly IPopupService _popupService;

        // 👇 Split into two collections
        public ObservableCollection<MyJobDetail> ActiveJobs { get; } = new ObservableCollection<MyJobDetail>();
        public ObservableCollection<MyJobDetail> JobHistory { get; } = new ObservableCollection<MyJobDetail>();

        public Command LoadJobsCommand { get; }
        public Command<MyJobDetail> ToggleClockCommand { get; }

        public Command NavigateViewAvailableJobsCommand { get; }
        public Command NavigateViewOngoingJobsCommand { get; }
        public Command NavigateUpdateProfileCommand { get; }
        public Command NavigateViewRatingsCommand { get; }

        public ViewOngoingJobsViewModel(IAuthenticationService auth, IPopupService popupService)
        {
            _auth = auth;
            _popupService = popupService;
            LoadJobsCommand = new Command(async () => await OnLoadJobsAsync());
            ToggleClockCommand = new Command<MyJobDetail>(async (job) => await OnToggleClockAsync(job));

            NavigateViewAvailableJobsCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewAvailableJobsPage"));
            NavigateViewOngoingJobsCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewOngoingJobsPage"));
            NavigateUpdateProfileCommand = new Command(async () => await Shell.Current.GoToAsync("../UpdateWorkerProfilePage"));
            NavigateViewRatingsCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewRatingsPage"));
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
                ActiveJobs.Clear();
                JobHistory.Clear();

                var allMyJobs = await _auth.GetMyWorkerJobsAsync();

                foreach (var job in allMyJobs)
                {
                    // Sort based on status
                    if (job.Job.Status == JobStatus.Completed || job.Job.Status == JobStatus.Incomplete)
                    {
                        JobHistory.Add(job);
                    }
                    else
                    {
                        ActiveJobs.Add(job);
                    }
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnToggleClockAsync(MyJobDetail jobDetail)
        {
            if (jobDetail == null) return;

            if (jobDetail.IsClockedIn)
            {
                // Clock Out
                var (success, message, totalTime) = await _auth.ClockOutAsync(jobDetail.ApplicationId);
                if (success)
                {
                    jobDetail.IsClockedIn = false;
                    if (totalTime.HasValue)
                        jobDetail.TotalTimeSpent = totalTime.Value;
                }
                else
                {
                    await _popupService.DisplayAlert("Error", message, "OK");
                }
            }
            else
            {
                // Clock In
                var (success, message) = await _auth.ClockInAsync(jobDetail.ApplicationId);
                if (success)
                {
                    jobDetail.IsClockedIn = true;
                }
                else
                {
                    await _popupService.DisplayAlert("Error", message, "OK");
                }
            }
        }
    }
}