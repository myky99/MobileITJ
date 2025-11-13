using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Models;
using MobileITJ.Services;
using System.Collections.Generic; // 👈 Make sure this is added

namespace MobileITJ.ViewModels
{
    public class ViewWorkersViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;
        public ObservableCollection<WorkerDetail> Workers { get; } = new ObservableCollection<WorkerDetail>();

        public Command LoadWorkersCommand { get; }
        public Command<WorkerDetail> ToggleActivationCommand { get; }
        public Command<WorkerDetail> AddSkillCommand { get; }

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

            // We just save the change. The 'IsActive' property in WorkerDetail
            // is already bound to the Switch, so no need to set it here.
            await _auth.UpdateWorkerProfileAsync(worker);
        }

        private async Task OnAddSkillAsync(WorkerDetail worker)
        {
            if (worker == null) return;

            string newSkill = await Application.Current.MainPage.DisplayPromptAsync("Add Skill", $"Enter new skill for {worker.FullName}:");

            if (!string.IsNullOrWhiteSpace(newSkill))
            {
                // Create a new list based on the old one
                var currentSkills = new List<string>(worker.Skills);
                currentSkills.Add(newSkill.Trim());

                // Assign the new list. This triggers the 'set' in WorkerDetail
                // which updates both 'Skills' and 'SkillsDisplay'
                worker.Skills = currentSkills;

                // Save the change to the JSON file
                await _auth.UpdateWorkerProfileAsync(worker);
            }
        }
    }
}