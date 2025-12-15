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
    // 👇 Store the service here
    private readonly IAuthenticationService _authService;

    // 👇 Update Constructor to accept the service
    public AppShell(IAuthenticationService authService)
    {
        InitializeComponent();
        _authService = authService; // Save it for later

        // Register Routes
        Routing.RegisterRoute("ChangePasswordPage", typeof(ChangePasswordPage));
        Routing.RegisterRoute("WelcomePage", typeof(WelcomePage));
        Routing.RegisterRoute("DeactivatedAccountPage", typeof(DeactivatedAccountPage));

        Routing.RegisterRoute("CreateWorkerPage", typeof(CreateWorkerPage));
        Routing.RegisterRoute("ViewWorkersPage", typeof(ViewWorkersPage));
        Routing.RegisterRoute("WorkerDetailsPage", typeof(WorkerDetailsPage));
        Routing.RegisterRoute("ViewJobsReportPage", typeof(ViewJobsReportPage));
        Routing.RegisterRoute("ReportDetailsPage", typeof(ReportDetailsPage));
        Routing.RegisterRoute("ViewCustomersPage", typeof(ViewCustomersPage));
        Routing.RegisterRoute("CustomerDetailsPage", typeof(CustomerDetailsPage));

        Routing.RegisterRoute("CreateJobPage", typeof(CreateJobPage));
        Routing.RegisterRoute("ViewMyJobsPage", typeof(ViewMyJobsPage));
        Routing.RegisterRoute("RateWorkerPage", typeof(RateWorkerPage));
        Routing.RegisterRoute("ViewMyJobReportsPage", typeof(ViewMyJobReportsPage));
        Routing.RegisterRoute("ViewJobApplicationsPage", typeof(ViewJobApplicationsPage));
        Routing.RegisterRoute("RateJobWorkersPage", typeof(RateJobWorkersPage));

        Routing.RegisterRoute("ViewAvailableJobsPage", typeof(ViewAvailableJobsPage));
        Routing.RegisterRoute("ViewOngoingJobsPage", typeof(ViewOngoingJobsPage));
        Routing.RegisterRoute("UpdateWorkerProfilePage", typeof(UpdateWorkerProfilePage));
        Routing.RegisterRoute("ViewRatingsPage", typeof(ViewRatingsPage));
        Routing.RegisterRoute("WalletPage", typeof(WalletPage));
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
        if (!confirm) return;

        // 👇 Use the stored service (No more null errors)
        if (_authService != null)
        {
            await _authService.LogoutAsync();
        }

        // Force navigation back to Login
        await GoToAsync("//LoginPage");
    }
}