using Microsoft.Maui.Controls;
using MobileITJ.ViewModels;

namespace MobileITJ.Views.Customer
{
    public partial class RateWorkerPage : ContentPage
    {
        // --- 👇 THIS IS THE UPDATED MVVM PATTERN 👇 ---

        private readonly RateWorkerViewModel _viewModel;

        public RateWorkerPage(RateWorkerViewModel vm)
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

        // --- ⛔️ REMOVE ALL THE OLD CODE-BEHIND COMMANDS ⛔️ ---
    }
}