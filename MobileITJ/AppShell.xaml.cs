using Microsoft.Maui.Controls;
using MobileITJ.Services;
using MobileITJ.Views.Auth;
using MobileITJ.Views.Shared;
using MobileITJ.Views.HR;
using MobileITJ.Views.Worker;
using MobileITJ.Views.Customer;

namespace MobileITJ;

public partial class AppShell : Shell
{
    private readonly IAuthenticationService _authService;

    public AppShell(IAuthenticationService authService)
    {
        InitializeComponent();
        _authService = authService;


        // Auth & Shared
        Routing.RegisterRoute("ChangePasswordPage", typeof(ChangePasswordPage));
        Routing.RegisterRoute("WelcomePage", typeof(WelcomePage));
        Routing.RegisterRoute("DeactivatedAccountPage", typeof(DeactivatedAccountPage));
        Routing.RegisterRoute("WalletPage", typeof(WalletPage));

        // HR Sub-pages (Details)
        Routing.RegisterRoute("WorkerDetailsPage", typeof(WorkerDetailsPage));
        Routing.RegisterRoute("ReportDetailsPage", typeof(ReportDetailsPage));
        Routing.RegisterRoute("CustomerDetailsPage", typeof(CustomerDetailsPage));

        // Customer Sub-pages
        Routing.RegisterRoute("ViewJobApplicationsPage", typeof(ViewJobApplicationsPage));
        Routing.RegisterRoute("RateJobWorkersPage", typeof(RateJobWorkersPage));
        Routing.RegisterRoute("WorkerPublicProfilePage", typeof(Views.Customer.WorkerPublicProfilePage));

    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
        if (!confirm) return;

        if (_authService != null)
        {
            await _authService.LogoutAsync();
        }

        await GoToAsync("//LoginPage");
    }
}