using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Models;
using MobileITJ.Services;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MobileITJ.ViewModels
{
    [QueryProperty(nameof(Worker), "Worker")]
    public class WorkerDetailsViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;
        private WorkerDetail _worker;

        // Picker Data
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

        public WorkerDetail Worker
        {
            get => _worker;
            set
            {
                SetProperty(ref _worker, value);
                // Trigger load when worker is set
                if (value != null)
                {
                    Task.Run(async () => await LoadWorkerHistoryAsync());
                }
            }
        }

        public ObservableCollection<JobApplicationDetail> ActiveJobs { get; } = new ObservableCollection<JobApplicationDetail>();
        public ObservableCollection<JobApplicationDetail> JobHistory { get; } = new ObservableCollection<JobApplicationDetail>();

        // Commands
        public Command ToggleActivationCommand { get; }
        public Command EditRateCommand { get; }
        public Command SaveSkillCommand { get; }

        public Command AddNewCategoryCommand { get; }
        public Command DeleteCategoryCommand { get; }

        public WorkerDetailsViewModel(IAuthenticationService auth)
        {
            _auth = auth;
            ToggleActivationCommand = new Command(async () => await OnToggleActivationAsync());
            EditRateCommand = new Command(async () => await OnEditRateAsync());
            SaveSkillCommand = new Command(async () => await OnSaveSkillAsync());

            AddNewCategoryCommand = new Command(async () => await OnAddNewCategoryAsync());
            DeleteCategoryCommand = new Command(async () => await OnDeleteCategoryAsync());

            Task.Run(async () => await LoadCategoriesAsync());
        }

        // 👇 UPDATED: Runs on Main Thread to ensure UI updates
        public async Task LoadWorkerHistoryAsync()
        {
            if (Worker == null) return;
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                var allHistory = await _auth.GetWorkerJobHistoryAsync(Worker.UserId);

                // IMPORTANT: UI updates must be on Main Thread
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ActiveJobs.Clear();
                    JobHistory.Clear();

                    foreach (var job in allHistory)
                    {
                        if (job.JobStatus == JobStatus.Completed ||
                            job.JobStatus == JobStatus.Incomplete ||
                            job.Status == ApplicationStatus.Rejected)
                        {
                            JobHistory.Add(job);
                        }
                        else
                        {
                            ActiveJobs.Add(job);
                        }
                    }
                });
            }
            finally
            {
                IsBusy = false;
            }
        }
        // 👆 END UPDATE

        private async Task LoadCategoriesAsync()
        {
            var cats = await _auth.GetSkillCategoriesAsync();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                AvailableSkills.Clear();
                foreach (var c in cats) AvailableSkills.Add(c);
            });
        }

        private async Task OnSaveSkillAsync()
        {
            if (Worker == null) return;
            string skillToAdd = SelectedSkill;

            if (string.IsNullOrEmpty(skillToAdd))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please select a skill category.", "OK");
                return;
            }

            if (skillToAdd == "Something Else")
            {
                if (string.IsNullOrWhiteSpace(CustomSkillEntry))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Please type the specific skill.", "OK");
                    return;
                }
                skillToAdd = CustomSkillEntry.Trim();
                await _auth.AddSkillCategoryAsync(skillToAdd);
                await LoadCategoriesAsync();
            }

            var currentSkills = new List<string>(Worker.Skills);
            if (currentSkills.Contains("Unspecified")) currentSkills.Remove("Unspecified");

            if (currentSkills.Contains(skillToAdd))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Worker already has this skill.", "OK");
                return;
            }

            currentSkills.Add(skillToAdd);
            Worker.Skills = currentSkills;

            await _auth.UpdateWorkerProfileAsync(Worker);
            await Application.Current.MainPage.DisplayAlert("Success", $"Added skill: {skillToAdd}", "OK");

            SelectedSkill = null;
            CustomSkillEntry = string.Empty;
        }

        private async Task OnAddNewCategoryAsync()
        {
            string newCat = await Application.Current.MainPage.DisplayPromptAsync("New Category", "Enter name of new job category:");
            if (!string.IsNullOrWhiteSpace(newCat))
            {
                await _auth.AddSkillCategoryAsync(newCat.Trim());
                await LoadCategoriesAsync();
            }
        }

        private async Task OnDeleteCategoryAsync()
        {
            string action = await Application.Current.MainPage.DisplayActionSheet("Delete Category", "Cancel", null, AvailableSkills.Where(s => s != "Something Else").ToArray());
            if (action != "Cancel" && action != null)
            {
                bool confirm = await Application.Current.MainPage.DisplayAlert("Confirm", $"Delete '{action}' from dropdown?", "Yes", "No");
                if (confirm)
                {
                    await _auth.RemoveSkillCategoryAsync(action);
                    await LoadCategoriesAsync();
                }
            }
        }

        private async Task OnToggleActivationAsync()
        {
            if (Worker == null) return;
            string action = Worker.IsActive ? "deactivate" : "activate";
            bool confirm = await Application.Current.MainPage.DisplayAlert("Confirm", $"Are you sure you want to {action} {Worker.FullName}?", "Yes", "No");
            if (!confirm) return;
            Worker.IsActive = !Worker.IsActive;
            await _auth.UpdateWorkerProfileAsync(Worker);
            await Application.Current.MainPage.DisplayAlert("Success", "Status updated.", "OK");
        }

        private async Task OnEditRateAsync()
        {
            if (Worker == null) return;
            string result = await Application.Current.MainPage.DisplayPromptAsync("Update Rate", $"Enter hourly rate for {Worker.FullName}:", initialValue: Worker.RatePerHour.ToString(), keyboard: Keyboard.Numeric);
            if (decimal.TryParse(result, out decimal newRate))
            {
                Worker.RatePerHour = newRate;
                await _auth.UpdateWorkerProfileAsync(Worker);
            }
        }
    }
}