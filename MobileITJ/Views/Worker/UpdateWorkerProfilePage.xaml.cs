using Microsoft.Maui.Controls;
using MobileITJ.ViewModels; // ?? --- ADD THIS ---

namespace MobileITJ.Views.Worker
{
    public partial class UpdateWorkerProfilePage : ContentPage
    {
        // --- ?? THIS IS THE NEW MVVM PATTERN ?? ---

        private readonly UpdateProfileViewModel _viewModel;

        public UpdateWorkerProfilePage(UpdateProfileViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm; // Set the "brain"
            _viewModel = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Tell the ViewModel to load the data
            await _viewModel.OnAppearing();
        }

        // --- ?? REMOVE ALL THE OLD CODE-BEHIND COMMANDS ?? ---
    }
}