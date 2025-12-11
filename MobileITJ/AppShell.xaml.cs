using Microsoft.Maui.Controls;
using MobileITJ.Views.Auth;
using MobileITJ.Views.Shared;
using MobileITJ.Views.HR;
using MobileITJ.Views.Worker;
using MobileITJ.Views.Customer;

namespace MobileITJ;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("ChangePasswordPage", typeof(ChangePasswordPage));
        Routing.RegisterRoute("WelcomePage", typeof(WelcomePage));
        Routing.RegisterRoute("DeactivatedAccountPage", typeof(DeactivatedAccountPage));

        Routing.RegisterRoute("CreateWorkerPage", typeof(CreateWorkerPage));
        Routing.RegisterRoute("ViewWorkersPage", typeof(ViewWorkersPage));
        Routing.RegisterRoute("ViewJobsReportPage", typeof(ViewJobsReportPage));
        Routing.RegisterRoute("ViewCustomersPage", typeof(ViewCustomersPage));

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
}