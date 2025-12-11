using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Models;
using MobileITJ.Services;
using System.Collections.Generic;

namespace MobileITJ.ViewModels
{
    public class ViewWorkersViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;
        public ObservableCollection<WorkerDetail> Workers { get; } = new ObservableCollection<WorkerDetail>();

        public Command LoadWorkersCommand { get; }
        public Command<WorkerDetail> ToggleActivationCommand { get; }
        public Command<WorkerDetail> AddSkillCommand { get; }
        public Command LogoutCommand { get; }

        // --- Navigation Commands for Tabs ---
        public Command NavigateCreateWorkerCommand { get; }
        public Command NavigateViewWorkersCommand { get; }
        public Command NavigateJobReportsCommand { get; }
        public Command NavigateCustomersCommand { get; }

        public ViewWorkersViewModel(IAuthenticationService auth)
        {
            _auth = auth;
            LoadWorkersCommand = new Command(async () => await OnLoadWorkersAsync());
            AddSkillCommand = new Command<WorkerDetail>(async (worker) => await OnAddSkillAsync(worker));
            ToggleActivationCommand = new Command<WorkerDetail>(async (worker) => await OnToggleActivationAsync(worker));
            LogoutCommand = new Command(async () => await OnLogoutAsync());

            // --- Tab Navigation ---
            NavigateCreateWorkerCommand = new Command(async () => await Shell.Current.GoToAsync("../CreateWorkerPage"));
            NavigateViewWorkersCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewWorkersPage"));
            NavigateJobReportsCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewJobsReportPage"));
            NavigateCustomersCommand = new Command(async () => await Shell.Current.GoToAsync("../ViewCustomersPage"));
        }

        public async Task OnAppearing()
        {
            // This is a special task to load data when the page appears
            await OnLoadWorkersAsync();
        }

        private async Task OnLoadWorkersAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                Workers.Clear();
                var workers = await _auth.GetAllWorkersAsync();
                foreach (var worker in workers)
                {
                    Workers.Add(worker);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnToggleActivationAsync(WorkerDetail worker)
        {
            if (worker == null) return;

            string status = worker.IsActive ? "activate" : "deactivate";
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirm Action",
                $"Are you sure you want to {status} {worker.FullName}?",
                "Yes",
                "No");

            if (!confirm)
            {
                // Revert the toggle
                worker.IsActive = !worker.IsActive;
                return;
            }

            await _auth.UpdateWorkerProfileAsync(worker);
            
            string message = worker.IsActive ? "activated" : "deactivated";
            await Application.Current.MainPage.DisplayAlert("Success", $"{worker.FullName} has been {message}.", "OK");
        }

        private async Task OnAddSkillAsync(WorkerDetail worker)
        {
            if (worker == null) return;

            string newSkill = await Application.Current.MainPage.DisplayPromptAsync(
                "Add Skill", 
                $"Enter new skill for {worker.FullName}:",
                "Add",
                "Cancel",
                "e.g., Plumbing");

            if (!string.IsNullOrWhiteSpace(newSkill))
            {
                var currentSkills = new List<string>(worker.Skills);
                
                // Check for duplicates
                if (currentSkills.Contains(newSkill.Trim()))
                {
                    await Application.Current.MainPage.DisplayAlert("Duplicate Skill", "This skill already exists for this worker.", "OK");
                    return;
                }

                currentSkills.Add(newSkill.Trim());
                worker.Skills = currentSkills;

                await _auth.UpdateWorkerProfileAsync(worker);
                
                await Application.Current.MainPage.DisplayAlert("Success", $"Skill '{newSkill}' added successfully!", "OK");
            }
        }

        private async Task OnLogoutAsync()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Logout", 
                "Are you sure you want to logout?", 
                "Yes", 
                "No");

            if (!confirm) return;

            await _auth.LogoutAsync();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}