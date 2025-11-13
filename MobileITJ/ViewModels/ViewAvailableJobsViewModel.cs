using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Models;
using MobileITJ.Services;
using System.Linq;
using System; // 👈 --- ADD THIS ---

namespace MobileITJ.ViewModels
{
    public class ViewAvailableJobsViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;
        public ObservableCollection<Job> AvailableJobs { get; } = new ObservableCollection<Job>();

        public Command LoadJobsCommand { get; }
        public Command<Job> ApplyJobCommand { get; }

        // --- Navigation Commands for Tabs ---
        public Command NavigateViewAvailableJobsCommand { get; }
        public Command NavigateViewOngoingJobsCommand { get; }
        public Command NavigateUpdateProfileCommand { get; }
        public Command NavigateViewRatingsCommand { get; }

        public ViewAvailableJobsViewModel(IAuthenticationService auth)
        {
            _auth = auth;
            LoadJobsCommand = new Command(async () => await OnLoadJobsAsync());
            ApplyJobCommand = new Command<Job>(async (job) => await OnApplyJobAsync(job));

            // --- Tab Navigation ---
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
                AvailableJobs.Clear();
                var openJobs = await _auth.GetAvailableJobsAsync();
                foreach (var job in openJobs)
                {
                    AvailableJobs.Add(job);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        // --- 👇 THIS METHOD IS NOW FULLY IMPLEMENTED 👇 ---
        private async Task OnApplyJobAsync(Job job)
        {
            if (job == null) return;

            // 1. Ask for negotiated rate
            string rateStr = await Application.Current.MainPage.DisplayPromptAsync(
                "Negotiate Rate",
                $"The listed rate is {job.RatePerHour:C}/hr. Enter your rate to apply.",
                "Apply", "Cancel",
                $"{job.RatePerHour}", // Prefill with the job's rate
                -1, Keyboard.Numeric, "");

            if (string.IsNullOrWhiteSpace(rateStr))
                return; // User cancelled

            if (!decimal.TryParse(rateStr, out decimal negotiatedRate))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Invalid rate. Please enter a valid number.", "OK");
                return;
            }

            // 2. Call the service to apply
            var (success, message) = await _auth.ApplyForJobAsync(job.Id, negotiatedRate);

            // 3. Show result
            await Application.Current.MainPage.DisplayAlert(
                success ? "Applied!" : "Error",
                message,
                "OK");
        }
        // --- END OF UPDATE ---
    }
}