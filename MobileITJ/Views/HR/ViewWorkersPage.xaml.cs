using Microsoft.Maui.Controls;
using MobileITJ.ViewModels;
using MobileITJ.Models; // ?? --- ADD THIS ---

namespace MobileITJ.Views.HR
{
    public partial class ViewWorkersPage : ContentPage
    {
        private readonly ViewWorkersViewModel _viewModel;

        public ViewWorkersPage(ViewWorkersViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            _viewModel = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Tell the ViewModel to load the data
            await _viewModel.OnAppearing();
        }

        // --- ?? ADD THIS EVENT HANDLER ?? ---
        private void OnWorkerToggled(object sender, ToggledEventArgs e)
        {
            // Get the Switch that was toggled
            var view = sender as Switch;
            if (view == null) return;

            // Get the 'WorkerDetail' object that this Switch is bound to
            var worker = view.BindingContext as WorkerDetail;
            if (worker == null) return;

            // Execute the command in our ViewModel
            // The 'worker' object already has the new 'IsActive' value
            _viewModel.ToggleActivationCommand.Execute(worker);
        }

        // --- ?? ADD THIS EVENT HANDLER ?? ---
        private void OnAddSkillClicked(object sender, System.EventArgs e)
        {
            var view = sender as Button;
            if (view == null) return;

            var worker = view.BindingContext as WorkerDetail;
            if (worker == null) return;

            _viewModel.AddSkillCommand.Execute(worker);
        }
    }
}