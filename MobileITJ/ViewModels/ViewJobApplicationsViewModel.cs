using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Models;
using MobileITJ.Services;
using System.Linq;

namespace MobileITJ.ViewModels
{
    [QueryProperty(nameof(JobId), "jobId")]
    public class ViewJobApplicationsViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;
        private readonly IPopupService _popupService;
        private int _jobId;

        public ObservableCollection<JobApplicationDetail> Applications { get; } = new ObservableCollection<JobApplicationDetail>();
        public Command LoadApplicationsCommand { get; }
        public Command<JobApplicationDetail> AcceptApplicationCommand { get; }
        public Command<JobApplicationDetail> RejectApplicationCommand { get; }

        // 👇 NEW: Command to view worker profile
        public Command<JobApplicationDetail> ViewWorkerProfileCommand { get; }

        public Command CompleteJobCommand { get; }
        private bool _canCompleteJob;
        public bool CanCompleteJob { get => _canCompleteJob; set => SetProperty(ref _canCompleteJob, value); }
        private bool _isJobCompleted;
        public bool IsJobCompleted { get => _isJobCompleted; set => SetProperty(ref _isJobCompleted, value); }

        public int JobId
        {
            get => _jobId;
            set
            {
                _jobId = value;
                OnPropertyChanged();
                LoadApplicationsCommand.Execute(null);
            }
        }

        public ViewJobApplicationsViewModel(IAuthenticationService auth, IPopupService popupService)
        {
            _auth = auth;
            _popupService = popupService;
            LoadApplicationsCommand = new Command(async () => await OnLoadApplicationsAsync());
            AcceptApplicationCommand = new Command<JobApplicationDetail>(async (app) => await OnAcceptApplicationAsync(app));
            RejectApplicationCommand = new Command<JobApplicationDetail>(async (app) => await OnRejectApplicationAsync(app));

            // 👇 NEW: Initialize Profile Command
            // We will navigate to 'WorkerPublicProfilePage' and pass the WorkerUserId
            ViewWorkerProfileCommand = new Command<JobApplicationDetail>(async (app) =>
                await Shell.Current.GoToAsync($"WorkerPublicProfilePage?workerId={app.WorkerUserId}&workerName={app.WorkerName}"));

            CompleteJobCommand = new Command(async () => await OnCompleteJobAsync(), () => CanCompleteJob);
        }

        private async Task OnLoadApplicationsAsync()
        {
            if (IsBusy) return;
            if (JobId == 0) return;
            IsBusy = true;

            try
            {
                Applications.Clear();
                var applications = await _auth.GetApplicationsForJobAsync(JobId);
                foreach (var app in applications)
                {
                    Applications.Add(app);
                }

                var firstApp = Applications.FirstOrDefault();
                if (firstApp != null)
                {
                    IsJobCompleted = (firstApp.JobStatus == JobStatus.Completed);
                }
                else
                {
                    IsJobCompleted = false;
                }

                bool anyClockedIn = Applications.Any(app => app.IsClockedIn);

                CanCompleteJob = !IsJobCompleted && !anyClockedIn && Applications.Any(app => app.Status == ApplicationStatus.Accepted);
                CompleteJobCommand.ChangeCanExecute();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnCompleteJobAsync()
        {
            var (success, message) = await _auth.CustomerCompleteJobAsync(JobId);

            if (success)
            {
                await _popupService.DisplayAlert("Job Completed!", message, "OK");
                await OnLoadApplicationsAsync();
            }
            else
            {
                await _popupService.DisplayAlert("Error", message, "OK");
            }
        }

        private async Task OnAcceptApplicationAsync(JobApplicationDetail application)
        {
            if (application == null) return;

            bool confirm = await _popupService.DisplayAlert(
                "Accept Worker?",
                $"Do you want to accept {application.WorkerName} for this job at Php {application.NegotiatedRate:N2}/hr?",
                "Accept", "Cancel");

            if (!confirm) return;

            var (success, message) = await _auth.AcceptApplicationAsync(application);

            if (success)
            {
                await _popupService.DisplayAlert("Worker Accepted!", message, "OK");
                await OnLoadApplicationsAsync();
            }
            else
            {
                await _popupService.DisplayAlert("Error", message, "OK");
            }
        }

        private async Task OnRejectApplicationAsync(JobApplicationDetail application)
        {
            if (application == null) return;

            bool confirm = await _popupService.DisplayAlert(
                "Reject Application",
                $"Are you sure you want to reject {application.WorkerName}?",
                "Yes, Reject", "Cancel");

            if (!confirm) return;

            var (success, message) = await _auth.RejectApplicationAsync(application.ApplicationId);

            if (success)
            {
                await OnLoadApplicationsAsync();
            }
            else
            {
                await _popupService.DisplayAlert("Error", message, "OK");
            }
        }
    }
}