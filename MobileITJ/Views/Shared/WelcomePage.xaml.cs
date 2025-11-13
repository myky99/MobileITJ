using MobileITJ.ViewModels;

namespace MobileITJ.Views.Shared;

public partial class WelcomePage : ContentPage
{
    public WelcomePage(WelcomeViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}