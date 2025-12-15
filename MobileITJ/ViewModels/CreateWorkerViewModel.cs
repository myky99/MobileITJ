using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Services;
using System.Collections.Generic;
using System.Linq;

namespace MobileITJ.ViewModels
{
    public class CreateWorkerViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        private decimal? _ratePerHour;
        public decimal? RatePerHour
        {
            get => _ratePerHour;
            set => SetProperty(ref _ratePerHour, value);
        }

        // Dropdown List Data
        public ObservableCollection<string> AvailableSkills { get; } = new ObservableCollection<string>();

        private string _selectedSkill;
        public string SelectedSkill
        {
            get => _selectedSkill;
            set
            {
                SetProperty(ref _selectedSkill, value);
                // Show custom box ONLY if "Something Else" is picked
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

        public Command LoadSkillsCommand { get; }
        public Command SubmitWorkerCommand { get; }
        public Command NavigateBackCommand { get; }

        public CreateWorkerViewModel(IAuthenticationService auth)
        {
            _auth = auth;
            LoadSkillsCommand = new Command(async () => await LoadCategoriesAsync());
            SubmitWorkerCommand = new Command(async () => await OnCreateWorkerAsync());
            NavigateBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));

            // Load skills immediately
            Task.Run(async () => await LoadCategoriesAsync());
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

        private async Task OnCreateWorkerAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            // 1. Determine Final Skill
            string finalSkill = SelectedSkill;

            if (string.IsNullOrEmpty(finalSkill))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please select a skill category.", "OK");
                IsBusy = false;
                return;
            }

            if (finalSkill == "Something Else")
            {
                if (string.IsNullOrWhiteSpace(CustomSkillEntry))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Please specify the custom skill.", "OK");
                    IsBusy = false;
                    return;
                }
                finalSkill = CustomSkillEntry.Trim();
            }

            // 2. Validate Text Fields
            if (string.IsNullOrWhiteSpace(FirstName) ||
                string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(Email))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all fields.", "OK");
                IsBusy = false;
                return;
            }

            // 3. Validate Rate
            if (RatePerHour == null || RatePerHour <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please enter a valid hourly rate.", "OK");
                IsBusy = false;
                return;
            }

            // 4. Create Worker
            var skillsList = new List<string> { finalSkill };

            var (success, message, workerId, tempPass) = await _auth.CreateWorkerAsync(
                FirstName,
                LastName,
                Email,
                skillsList,
                RatePerHour.Value);

            if (success)
            {
                // 👇 THIS WAS MISSING: Save the new skill to the dropdown list!
                if (SelectedSkill == "Something Else")
                {
                    await _auth.AddSkillCategoryAsync(finalSkill);
                    // Refresh the list immediately
                    await LoadCategoriesAsync();
                }
                // 👆 END FIX

                await Application.Current.MainPage.DisplayAlert("Success",
                    $"Worker Created!\nID: {workerId}\nTemp Password: {tempPass}", "OK");

                // Clear fields
                FirstName = string.Empty;
                LastName = string.Empty;
                Email = string.Empty;
                RatePerHour = null;
                SelectedSkill = null;
                CustomSkillEntry = string.Empty;
                IsCustomSkillVisible = false; // Hide the box

                OnPropertyChanged(nameof(FirstName));
                OnPropertyChanged(nameof(LastName));
                OnPropertyChanged(nameof(Email));
                OnPropertyChanged(nameof(RatePerHour));

                // Go back
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
            }

            IsBusy = false;
        }
    }
}