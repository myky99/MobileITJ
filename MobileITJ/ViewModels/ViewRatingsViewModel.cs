using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Models;
using MobileITJ.Services;
using System.Linq;

namespace MobileITJ.ViewModels
{
    public class ViewRatingsViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;

        public ObservableCollection<RatingDetail> RatedJobs { get; } = new ObservableCollection<RatingDetail>();

        // 👇 NEW: Selected Item Logic for the Click Event
        private RatingDetail _selectedRating;
        public RatingDetail SelectedRating
        {
            get => _selectedRating;
            set
            {
                SetProperty(ref _selectedRating, value);
                if (value != null)
                {
                    // Trigger the command immediately when an item is selected
                    ViewRatingDetailsCommand.Execute(value);
                    SelectedRating = null; // Reset selection so it can be clicked again
                }
            }
        }

        public Command LoadRatingsCommand { get; }
        public Command<RatingDetail> ViewRatingDetailsCommand { get; } // 👈 NEW Command

        // Navigation Commands
        public Command NavigateViewAvailableJobsCommand { get; }
        public Command NavigateViewOngoingJobsCommand { get; }
        public Command NavigateUpdateProfileCommand { get; }
        public Command NavigateViewRatingsCommand { get; }

        public ViewRatingsViewModel(IAuthenticationService auth)
        {
            _auth = auth;
            LoadRatingsCommand = new Command(async () => await OnLoadRatingsAsync());

            // 👇 NEW: The Logic for the Pop-up
            ViewRatingDetailsCommand = new Command<RatingDetail>(async (rating) => await OnViewRatingDetailsAsync(rating));

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

        // 👇 NEW: Show the Popup with Details
        private async Task OnViewRatingDetailsAsync(RatingDetail rating)
        {
            if (rating == null) return;

            await Application.Current.MainPage.DisplayAlert(
                "Rating Details",
                $"Job: {rating.JobDescription}\n\n" +
                $"Rated By: {rating.CustomerName}\n\n" + // Shows the Client Name
                $"Rating: {rating.Rating} Stars\n" +
                $"Review: \"{rating.Review}\"",
                "Close");
        }
    }
}