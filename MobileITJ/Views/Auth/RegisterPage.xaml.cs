using MobileITJ.ViewModels;

namespace MobileITJ.Views.Auth
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage(RegisterViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}