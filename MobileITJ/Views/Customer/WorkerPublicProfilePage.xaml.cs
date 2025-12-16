using Microsoft.Maui.Controls;
using MobileITJ.ViewModels;

namespace MobileITJ.Views.Customer
{
    public partial class WorkerPublicProfilePage : ContentPage
    {
        public WorkerPublicProfilePage(WorkerPublicProfileViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}