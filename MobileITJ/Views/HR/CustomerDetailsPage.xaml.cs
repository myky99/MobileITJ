using MobileITJ.ViewModels;
namespace MobileITJ.Views.HR
{
    public partial class CustomerDetailsPage : ContentPage
    {
        public CustomerDetailsPage(CustomerDetailsViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}