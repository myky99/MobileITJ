using MobileITJ.ViewModels;

namespace MobileITJ.Views.Customer
{
    public partial class CustomerDashboardPage : ContentPage
    {
        private readonly CustomerDashboardViewModel _viewModel;
        public CustomerDashboardPage(CustomerDashboardViewModel vm)
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