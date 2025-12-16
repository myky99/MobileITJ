using MobileITJ.ViewModels;

namespace MobileITJ.Views.HR
{
    public partial class ReportDetailsPage : ContentPage
    {
        public ReportDetailsPage(ReportDetailsViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}