using MobileITJ.ViewModels; // 👈 ADD THIS

namespace MobileITJ.Views.HR;

public partial class CreateWorkerPage : ContentPage
{
    // --- 👇 REPLACE THE CONSTRUCTOR 👇 ---
    public CreateWorkerPage(CreateWorkerViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm; // 👈 Set the "brain"
    }
}