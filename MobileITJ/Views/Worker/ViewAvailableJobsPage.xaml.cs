using Microsoft.Maui.Controls;
using MobileITJ.ViewModels;

namespace MobileITJ.Views.Worker
{
    public partial class ViewAvailableJobsPage : ContentPage
    {
        private readonly ViewAvailableJobsViewModel _viewModel;

        public ViewAvailableJobsPage(ViewAvailableJobsViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            _viewModel = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.OnAppearing();
        }
    }
}