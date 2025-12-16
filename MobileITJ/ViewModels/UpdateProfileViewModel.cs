using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Models;
using MobileITJ.Services;
using System.Collections.Generic;
using System.Linq;

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

        // --- SKILL ADDITION PROPERTIES ---
        public ObservableCollection<string> AvailableSkills { get; } = new ObservableCollection<string>();

        private string _selectedSkill;
        public string SelectedSkill
        {
            get => _selectedSkill;
            set
            {
                SetProperty(ref _selectedSkill, value);
                IsCustomSkillVisible = value == "Something Else";
            }
        }

        private string _customSkillEntry;
        public string CustomSkillEntry
        {
            get => _customSkillEntry;
            set => SetProperty(ref _customSkillEntry, value);
        }

        private bool _isCustomSkillVisible;
        public bool IsCustomSkillVisible
        {
            get => _isCustomSkillVisible;
            set => SetProperty(ref _isCustomSkillVisible, value);
        }

        private bool _isAddingSkill;
        public bool IsAddingSkill
        {
            get => _isAddingSkill;
            set => SetProperty(ref _isAddingSkill, value);
        }

        // --- COMMANDS ---
        public Command LoadProfileCommand { get; }
        public Command NavigateToChangePasswordCommand { get; }
        public Command NavigateToWalletCommand { get; }
        public Command LogoutCommand { get; }

        public Command ToggleAddSkillCommand { get; }
        public Command SaveSkillCommand { get; }

        public Command NavigateViewAvailableJobsCommand { get; }
        public Command NavigateViewOngoingJobsCommand { get; }
        public Command NavigateUpdateProfileCommand { get; }
        public Command NavigateViewRatingsCommand { get; }

        public UpdateProfileViewModel(IAuthenticationService auth)
        {
            _auth = auth;
            LoadProfileCommand = new Command(async () => await OnLoadProfileAsync());
            NavigateToChangePasswordCommand = new Command(async () => await Shell.Current.GoToAsync("ChangePasswordPage"));
            NavigateToWalletCommand = new Command(async () => await Shell.Current.GoToAsync("WalletPage"));
            LogoutCommand = new Command(async () => await OnLogoutAsync());

            ToggleAddSkillCommand = new Command(() =>
            {
                IsAddingSkill = !IsAddingSkill;
                if (IsAddingSkill) LoadCategoriesAsync(); // Load skills when opening
            });

            SaveSkillCommand = new Command(async () => await OnSaveSkillAsync());

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
            finally { IsBusy = false; }
        }

        private async Task LoadCategoriesAsync()
        {
            var cats = await _auth.GetSkillCategoriesAsync();
            if (cats != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    AvailableSkills.Clear();
                    foreach (var c in cats) AvailableSkills.Add(c);
                });
            }
        }

        private async Task OnSaveSkillAsync()
        {
            if (Profile == null) return;
            if (string.IsNullOrEmpty(SelectedSkill))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Select a skill first.", "OK");
                return;
            }

            string skillToAdd = SelectedSkill;
            if (skillToAdd == "Something Else")
            {
                if (string.IsNullOrWhiteSpace(CustomSkillEntry))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Type your custom skill.", "OK");
                    return;
                }
                skillToAdd = CustomSkillEntry.Trim();

                // Save custom skill globally
                await _auth.AddSkillCategoryAsync(skillToAdd);
                await LoadCategoriesAsync();
            }

            // Check if already exists
            if (Profile.Skills.Contains(skillToAdd))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "You already have this skill.", "OK");
                return;
            }

            // Add to list and save
            var newSkills = new List<string>(Profile.Skills) { skillToAdd };
            if (newSkills.Contains("Unspecified")) newSkills.Remove("Unspecified");

            Profile.Skills = newSkills;
            await _auth.UpdateWorkerProfileAsync(Profile);

            // Refresh UI
            OnPropertyChanged(nameof(Profile));

            // Reset and close
            SelectedSkill = null;
            CustomSkillEntry = string.Empty;
            IsAddingSkill = false;

            await Application.Current.MainPage.DisplayAlert("Success", "Skill Added!", "OK");
        }

        private async Task OnLogoutAsync()
        {
            await _auth.LogoutAsync();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}