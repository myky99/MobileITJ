using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Models;
using MobileITJ.Services;
using System.Linq;

namespace MobileITJ.ViewModels
{
    [QueryProperty(nameof(JobId), "jobId")]
    public class RateJobWorkersViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;
        private readonly IPopupService _popupService;
        private int _jobId;

        public ObservableCollection<JobApplicationDetail> WorkersToRate { get; } = new ObservableCollection<JobApplicationDetail>();
        public Command LoadWorkersCommand { get; }
        public Command<JobApplicationDetail> RateWorkerCommand { get; }
        public Command<JobApplicationDetail> PayWorkerCommand { get; } // 👈 --- ADD THIS ---

        public int JobId
        {
            get => _jobId;
            set
            {
                _jobId = value;
                OnPropertyChanged();
                LoadWorkersCommand.Execute(null);
            }
        }

        public RateJobWorkersViewModel(IAuthenticationService auth, IPopupService popupService)
        {
            _auth = auth;
            _popupService = popupService;
            LoadWorkersCommand = new Command(async () => await OnLoadWorkersAsync());
            RateWorkerCommand = new Command<JobApplicationDetail>(async (app) => await OnRateWorkerAsync(app));
            PayWorkerCommand = new Command<JobApplicationDetail>(async (app) => await OnPayWorkerAsync(app)); // 👈 --- ADD THIS ---
        }

        private async Task OnLoadWorkersAsync()
        {
            if (IsBusy) return;
            if (JobId == 0) return;
            IsBusy = true;

            try
            {
                WorkersToRate.Clear();
                var applications = await _auth.GetApplicationsForJobAsync(JobId);
                // Only show workers who were actually accepted for the job
                foreach (var app in applications.Where(a => a.Status == ApplicationStatus.Accepted))
                {
                    WorkersToRate.Add(app);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        // --- 👇 ADD THIS NEW METHOD 👇 ---
        private async Task OnPayWorkerAsync(JobApplicationDetail application)
        {
            if (application == null || application.IsPaid) return;

            bool confirm = await _popupService.DisplayAlert(
                "Confirm Payment",
                $"Do you want to pay {application.WorkerName} the amount of {application.TotalPay:C} for {application.TotalTimeSpent.TotalHours:N2} hours?",
                "Yes, Pay Now", "Cancel");

            if (!confirm) return;

            var (success, message) = await _auth.PayWorkerAsync(application.ApplicationId);

            if (success)
            {
                await _popupService.DisplayAlert("Payment Success", message, "OK");
                application.IsPaid = true; // Update the UI
            }
            else
            {
                await _popupService.DisplayAlert("Payment Error", message, "OK");
            }
        }
        // --- END OF NEW ---

        private async Task OnRateWorkerAsync(JobApplicationDetail application)
        {
            if (application == null) return;

            // --- 👇 NEW LOGIC: Can only rate *after* paying ---
            if (!application.IsPaid)
            {
                await _popupService.DisplayAlert("Payment Required", "You must pay the worker before you can submit a rating.", "OK");
                return;
            }
            // --- END OF NEW ---

            if (application.IsRated)
            {
                await _popupService.DisplayAlert("Already Rated", "You have already rated this worker for this job.", "OK");
                return;
            }

            string review = await _popupService.DisplayPrompt(
                "Write a Review",
                $"How was your experience with {application.WorkerName}?",
                "Submit", "Cancel", "e.g., Great work!");

            if (review == null) // User cancelled
                return;

            string ratingStr = await _popupService.DisplayActionSheet(
                "Select a Rating", "Cancel", null, "⭐️⭐️⭐️⭐️⭐️", "⭐️⭐️⭐️⭐️", "⭐️⭐️⭐️", "⭐️⭐️", "⭐️");

            if (string.IsNullOrEmpty(ratingStr) || ratingStr == "Cancel")
                return;

            int rating = 0;
            if (ratingStr == "⭐️") rating = 1;
            else if (ratingStr == "⭐️⭐️") rating = 2;
            else if (ratingStr == "⭐️⭐️⭐️") rating = 3;
            else if (ratingStr == "⭐️⭐️⭐️⭐️") rating = 4;
            else if (ratingStr == "⭐️⭐️⭐️⭐️⭐️") rating = 5;

            if (rating == 0)
                return;

            await _auth.RateWorkerOnJobAsync(application.ApplicationId, rating, review);

            application.IsRated = true;
            application.Rating = rating;
            application.Review = review;

            await _popupService.DisplayAlert("Success", "Your rating has been submitted. Thank you!", "OK");
        }
    }
}