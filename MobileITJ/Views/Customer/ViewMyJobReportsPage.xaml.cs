using Microsoft.Maui.Controls;
using MobileITJ.ViewModels;

namespace MobileITJ.Views.Customer
{
    public partial class ViewMyJobReportsPage : ContentPage
    {
        private readonly ViewMyJobReportsViewModel _viewModel; // 👈 Add

        public ViewMyJobReportsPage(ViewMyJobReportsViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            _viewModel = vm; // 👈 Add
        }

        // --- 👇 ADD THIS METHOD 👇 ---
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Tell the ViewModel to load the data
            await _viewModel.OnAppearing();
        }
    }
}