using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Models;
using MobileITJ.Services;
using System.Linq;
using System.Collections.Generic;

namespace MobileITJ.ViewModels
{
    [QueryProperty(nameof(WorkerId), "workerId")]
    [QueryProperty(nameof(WorkerName), "workerName")]
    public class WorkerPublicProfileViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;
        private int _workerId;
        private string _workerName = "";
        private double _averageRating;

        public ObservableCollection<JobApplicationDetail> Reviews { get; } = new ObservableCollection<JobApplicationDetail>();

        public int WorkerId
        {
            get => _workerId;
            set
            {
                _workerId = value;
                OnPropertyChanged();
                LoadHistoryCommand.Execute(null);
            }
        }

        public string WorkerName
        {
            get => _workerName;
            set => SetProperty(ref _workerName, value);
        }

        public double AverageRating
        {
            get => _averageRating;
            set => SetProperty(ref _averageRating, value);
        }

        public Command LoadHistoryCommand { get; }
        public Command BackCommand { get; }

        public WorkerPublicProfileViewModel(IAuthenticationService auth)
        {
            _auth = auth;
            LoadHistoryCommand = new Command(async () => await OnLoadHistoryAsync());
            BackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        }

        private async Task OnLoadHistoryAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                Reviews.Clear();

                // Get all history for this worker
                var history = await _auth.GetWorkerJobHistoryAsync(WorkerId);

                // Filter only rated jobs
                var ratedJobs = history.Where(h => h.IsRated).ToList();

                // Calculate Average
                if (ratedJobs.Any())
                {
                    AverageRating = ratedJobs.Average(r => r.Rating);
                }
                else
                {
                    AverageRating = 0;
                }

                // Add to list (Newest first)
                foreach (var review in ratedJobs.OrderByDescending(r => r.ApplicationId))
                {
                    Reviews.Add(review);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}