using System.Threading.Tasks;
using Microsoft.Maui.Controls; // ?? --- THIS LINE IS REQUIRED FOR THE POP-UP ---
using MobileITJ.Services;
using MobileITJ.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace MobileITJ.ViewModels
{
 public class RateWorkerViewModel : BaseViewModel
 {
 private readonly IAuthenticationService _auth;
 public ObservableCollection<Job> Jobs { get; } = new ObservableCollection<Job>();
 public Command LoadJobsCommand { get; }
 public Command<Job> RateJobCommand { get; }

 public Command NavigateCreateJobCommand { get; }
 public Command NavigateViewMyJobsCommand { get; }
 public Command NavigateRateWorkerCommand { get; }
 public Command NavigateViewReportsCommand { get; }

 public RateWorkerViewModel(IAuthenticationService auth)
 {
 _auth = auth;
 LoadJobsCommand = new Command(async () => await OnLoadJobsAsync());
 RateJobCommand = new Command<Job>(async (job) => await OnRateJobAsync(job));

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
 var myJobs = await _auth.GetMyJobsAsync();
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

 private async Task OnRateJobAsync(Job job)
 {
 if (job == null) return;

 if (job.IsRated)
 {
 await Application.Current.MainPage.DisplayAlert("Already Rated", "You have already submitted a rating for this job.", "OK");
 return;
 }

 string review = await Application.Current.MainPage.DisplayPromptAsync(
 "Write a Review",
 $"How was your experience with the worker on '{job.JobDescription}'?",
 "Submit", "Cancel", "e.g., Great work!", -1, Keyboard.Default, "");

 if (review == null)
 return;

 // Use DisplayActionSheet (Task<string>) which is available on Page
 string ratingStr = await Application.Current.MainPage.DisplayActionSheet(
 "Select a Rating", "Cancel", null, "??????????", "????????", "??????", "????", "??");

 if (string.IsNullOrEmpty(ratingStr) || ratingStr == "Cancel")
 return;

 int rating =0;
 if (ratingStr == "??") rating =1;
 else if (ratingStr == "????") rating =2;
 else if (ratingStr == "??????") rating =3;
 else if (ratingStr == "????????") rating =4;
 else if (ratingStr == "??????????") rating =5;

 if (rating ==0)
 return;

 await _auth.RateJobAsync(job, rating, review);

 job.IsRated = true;
 job.Rating = rating;
 job.Review = review;

 await Application.Current.MainPage.DisplayAlert("Success", "Your rating has been submitted. Thank you!", "OK");
 }
 }
}
