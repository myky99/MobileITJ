using Microsoft.Maui.Controls;
using MobileITJ.ViewModels;

namespace MobileITJ.Views.HR
{
    public partial class ViewCustomersPage : ContentPage
    {
        // --- ?? THIS IS THE NEW MVVM PATTERN ?? ---

        private readonly ViewCustomersViewModel _viewModel;

        public ViewCustomersPage(ViewCustomersViewModel vm)
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