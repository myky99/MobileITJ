namespace MobileITJ.Views.Shared;

public partial class DeactivatedAccountPage : ContentPage
{
    public DeactivatedAccountPage()
    {
        InitializeComponent();
    }

    private async void OnBackToLoginClicked(object sender, System.EventArgs e)
    {
        // Navigate back to the Login page (absolute route)
        await Shell.Current.GoToAsync("//LoginPage");
    }
}