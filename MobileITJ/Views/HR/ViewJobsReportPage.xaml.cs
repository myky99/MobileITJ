using Microsoft.Maui.Controls;
using MobileITJ.ViewModels;

namespace MobileITJ.Views.HR
{
    public partial class ViewJobsReportPage : ContentPage
    {
        // --- ?? THIS IS THE NEW MVVM PATTERN ?? ---

        private readonly ViewJobsReportViewModel _viewModel;

        public ViewJobsReportPage(ViewJobsReportViewModel vm)
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
    }
}