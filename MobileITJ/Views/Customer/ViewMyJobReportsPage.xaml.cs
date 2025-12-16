using Microsoft.Maui.Controls;
using MobileITJ.ViewModels;

namespace MobileITJ.Views.Customer
{
    public partial class ViewMyJobReportsPage : ContentPage
    {
        private readonly ViewMyJobReportsViewModel _viewModel;

        public ViewMyJobReportsPage(ViewMyJobReportsViewModel vm)
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