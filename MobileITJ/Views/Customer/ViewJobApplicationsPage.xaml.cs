using MobileITJ.ViewModels;

namespace MobileITJ.Views.Customer;

public partial class ViewJobApplicationsPage : ContentPage
{
    public ViewJobApplicationsPage(ViewJobApplicationsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}