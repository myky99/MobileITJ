using MobileITJ.ViewModels;

namespace MobileITJ.Views.Customer;

public partial class RateJobWorkersPage : ContentPage
{
    public RateJobWorkersPage(RateJobWorkersViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}