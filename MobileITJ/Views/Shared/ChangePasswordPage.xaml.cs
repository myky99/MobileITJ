using MobileITJ.ViewModels;

namespace MobileITJ.Views.Shared
{
    public partial class ChangePasswordPage : ContentPage
    {
        public ChangePasswordPage(ChangePasswordViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}