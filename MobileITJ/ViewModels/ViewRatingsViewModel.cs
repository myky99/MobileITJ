using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Models;
using MobileITJ.Services;

namespace MobileITJ.ViewModels
{
    public class ViewRatingsViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;
        // --- 👇 UPDATED LIST TYPE 👇 ---
        public ObservableCollection<RatingDetail> RatedJobs { get; } = new ObservableCollection<RatingDetail>();

        public Command LoadRatingsCommand { get; }

        public Command NavigateViewAvailableJobsCommand { get; }
        public Command NavigateViewOngoingJobsCommand { get; }
        public Command NavigateUpdateProfileCommand { get; }
        public Command NavigateViewRatingsCommand { get; }

        public ViewRatingsViewModel(IAuthenticationService auth)
        {
            _auth = auth;
            LoadRatingsCommand = new Command(async () => await OnLoadRatingsAsync());

            NavigateViewAvailableJobsCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewAvailableJobsPage"));
            NavigateViewOngoingJobsCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewOngoingJobsPage"));
            NavigateUpdateProfileCommand = new Command(async () => await Shell.Current.GoToAsync("../UpdateWorkerProfilePage"));
            NavigateViewRatingsCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewRatingsPage"));
        }

        public async Task OnAppearing()
        {
            await OnLoadRatingsAsync();
        }

        private async Task OnLoadRatingsAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                RatedJobs.Clear();
                // --- 👇 UPDATED METHOD CALL 👇 ---
                var myRatings = await _auth.GetMyRatingsAsync();
                foreach (var rating in myRatings)
                {
                    RatedJobs.Add(rating);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}