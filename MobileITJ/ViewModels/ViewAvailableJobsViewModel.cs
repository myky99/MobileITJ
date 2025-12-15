using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Models;
using MobileITJ.Services;
using System.Linq;
using System.Collections.Generic;
using System;

namespace MobileITJ.ViewModels
{
    public class ViewAvailableJobsViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;
        private List<Job> _allJobs = new List<Job>();

        public ObservableCollection<Job> AvailableJobs { get; } = new ObservableCollection<Job>();

        // --- 🔎 Search & Filter Properties ---
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { SetProperty(ref _searchText, value); FilterJobs(); }
        }

        public ObservableCollection<string> SkillCategories { get; } = new ObservableCollection<string>();

        private string _selectedCategory;
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                SetProperty(ref _selectedCategory, value);

                // 👇 NEW: Toggle Custom Entry if "Something Else" is picked
                IsCustomSkillFilterVisible = value == "Something Else";

                // Clear custom text if switching away
                if (!IsCustomSkillFilterVisible) CustomSkillFilter = "";

                FilterJobs();
            }
        }

        // 👇 NEW: Custom Skill Filter Logic
        private string _customSkillFilter;
        public string CustomSkillFilter
        {
            get => _customSkillFilter;
            set { SetProperty(ref _customSkillFilter, value); FilterJobs(); }
        }

        private bool _isCustomSkillFilterVisible;
        public bool IsCustomSkillFilterVisible
        {
            get => _isCustomSkillFilterVisible;
            set => SetProperty(ref _isCustomSkillFilterVisible, value);
        }
        // 👆 END NEW LOGIC

        public Command LoadJobsCommand { get; }
        public Command<Job> ApplyJobCommand { get; }
        public Command ClearFiltersCommand { get; }

        public Command NavigateViewAvailableJobsCommand { get; }
        public Command NavigateViewOngoingJobsCommand { get; }
        public Command NavigateUpdateProfileCommand { get; }
        public Command NavigateViewRatingsCommand { get; }

        public ViewAvailableJobsViewModel(IAuthenticationService auth)
        {
            _auth = auth;
            LoadJobsCommand = new Command(async () => await OnLoadJobsAsync());
            ApplyJobCommand = new Command<Job>(async (job) => await OnApplyJobAsync(job));
            ClearFiltersCommand = new Command(OnClearFilters);

            NavigateViewAvailableJobsCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewAvailableJobsPage"));
            NavigateViewOngoingJobsCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewOngoingJobsPage"));
            NavigateUpdateProfileCommand = new Command(async () => await Shell.Current.GoToAsync("../UpdateWorkerProfilePage"));
            NavigateViewRatingsCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewRatingsPage"));
        }

        public async Task OnAppearing()
        {
            await LoadCategoriesAsync();
            await OnLoadJobsAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            var cats = await _auth.GetSkillCategoriesAsync();
            if (cats != null)
            {
                SkillCategories.Clear();
                foreach (var c in cats) SkillCategories.Add(c);
            }
        }

        private async Task OnLoadJobsAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                _allJobs = await _auth.GetAvailableJobsAsync();
                FilterJobs();
            }
            finally { IsBusy = false; }
        }

        private void FilterJobs()
        {
            var filtered = _allJobs.AsEnumerable();

            // 1. Filter by Search Text (Title or Location)
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string query = SearchText.ToLower();
                filtered = filtered.Where(j =>
                    (j.Title != null && j.Title.ToLower().Contains(query)) ||
                    (j.Location != null && j.Location.ToLower().Contains(query)) ||
                    (j.JobDescription != null && j.JobDescription.ToLower().Contains(query))
                );
            }

            // 2. Filter by Category
            if (!string.IsNullOrEmpty(SelectedCategory))
            {
                if (SelectedCategory == "Something Else")
                {
                    // 👇 If "Something Else", filter by the Custom Text Box
                    if (!string.IsNullOrWhiteSpace(CustomSkillFilter))
                    {
                        string customQuery = CustomSkillFilter.ToLower();
                        filtered = filtered.Where(j => j.SkillsNeeded != null &&
                            j.SkillsNeeded.Any(s => s.ToLower().Contains(customQuery)));
                    }
                }
                else
                {
                    // Standard Filter
                    filtered = filtered.Where(j => j.SkillsNeeded != null && j.SkillsNeeded.Contains(SelectedCategory));
                }
            }

            AvailableJobs.Clear();
            foreach (var job in filtered) AvailableJobs.Add(job);
        }

        private void OnClearFilters()
        {
            SearchText = string.Empty;
            SelectedCategory = null;
            CustomSkillFilter = string.Empty;
            IsCustomSkillFilterVisible = false;
        }

        private async Task OnApplyJobAsync(Job job)
        {
            if (job == null) return;

            string rateStr = await Application.Current.MainPage.DisplayPromptAsync(
                "Negotiate Rate",
                $"The listed rate is {job.RatePerHour:C}/hr. Enter your rate to apply.",
                "Apply", "Cancel", $"{job.RatePerHour}", -1, Keyboard.Numeric, "");

            if (string.IsNullOrWhiteSpace(rateStr)) return;
            if (!decimal.TryParse(rateStr, out decimal negotiatedRate)) return;

            var (success, message) = await _auth.ApplyForJobAsync(job.Id, negotiatedRate);
            await Application.Current.MainPage.DisplayAlert(success ? "Applied!" : "Error", message, "OK");
        }
    }
}