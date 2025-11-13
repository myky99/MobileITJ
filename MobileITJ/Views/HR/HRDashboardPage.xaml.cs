using MobileITJ.ViewModels;

namespace MobileITJ.Views.HR
{
    public partial class HRDashboardPage : ContentPage
    {
        private readonly HRDashboardViewModel _viewModel;

        public HRDashboardPage(HRDashboardViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            _viewModel = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.OnAppearing(); // Call the viewmodel's OnAppearing
        }
    }
}