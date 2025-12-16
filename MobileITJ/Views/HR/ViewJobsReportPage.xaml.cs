using Microsoft.Maui.Controls;
using MobileITJ.ViewModels;

namespace MobileITJ.Views.HR
{
    public partial class ViewJobsReportPage : ContentPage
    {
        public ViewJobsReportPage(ViewJobsReportViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is ViewJobsReportViewModel vm)
            {
                await vm.OnAppearing();
            }
        }
    }
}