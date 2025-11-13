using MobileITJ.ViewModels;

namespace MobileITJ.Views.Worker
{
    public partial class WorkerDashboardPage : ContentPage
    {
        private readonly WorkerDashboardViewModel _viewModel;
        public WorkerDashboardPage(WorkerDashboardViewModel vm)
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