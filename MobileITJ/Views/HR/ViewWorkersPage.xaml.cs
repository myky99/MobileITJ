using Microsoft.Maui.Controls;
using MobileITJ.ViewModels;

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
            await _viewModel.OnAppearing();
        }
    }
}