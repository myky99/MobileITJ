using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Models;
using MobileITJ.Services;
using System.ComponentModel;

namespace MobileITJ.ViewModels
{
    public class UpdateProfileViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;

        private WorkerDetail? _profile;
        public WorkerDetail? Profile
        {
            get => _profile;
            set => SetProperty(ref _profile, value);
        }

        public Command LoadProfileCommand { get; }
        public Command NavigateToChangePasswordCommand { get; }
        public Command NavigateToWalletCommand { get; } // 👈 --- ADD THIS ---
        public Command LogoutCommand { get; }

        public Command NavigateViewAvailableJobsCommand { get; }
        public Command NavigateViewOngoingJobsCommand { get; }
        public Command NavigateUpdateProfileCommand { get; }
        public Command NavigateViewRatingsCommand { get; }

        public UpdateProfileViewModel(IAuthenticationService auth)
        {
            _auth = auth;
            LoadProfileCommand = new Command(async () => await OnLoadProfileAsync());
            NavigateToChangePasswordCommand = new Command(async () => await Shell.Current.GoToAsync("ChangePasswordPage"));
            NavigateToWalletCommand = new Command(async () => await Shell.Current.GoToAsync("WalletPage")); // 👈 --- ADD THIS ---
            LogoutCommand = new Command(async () => await OnLogoutAsync());

            NavigateViewAvailableJobsCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewAvailableJobsPage"));
            NavigateViewOngoingJobsCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewOngoingJobsPage"));
            NavigateUpdateProfileCommand = new Command(async () => await Shell.Current.GoToAsync("../UpdateWorkerProfilePage"));
            NavigateViewRatingsCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewRatingsPage"));
        }

        public async Task OnAppearing()
        {
            await OnLoadProfileAsync();
        }

        private async Task OnLoadProfileAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                Profile = await _auth.GetMyWorkerProfileAsync();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnLogoutAsync()
        {
            await _auth.LogoutAsync();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}