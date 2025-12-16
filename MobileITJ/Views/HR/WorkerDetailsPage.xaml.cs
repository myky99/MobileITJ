using MobileITJ.ViewModels;
using Microsoft.Maui.Controls;

namespace MobileITJ.Views.HR
{
    public partial class WorkerDetailsPage : ContentPage
    {
        public WorkerDetailsPage(WorkerDetailsViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        // 👇 ADDED THIS: Ensures data loads when page appears
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is WorkerDetailsViewModel vm)
            {
                await vm.LoadWorkerHistoryAsync();
            }
        }
    }
}