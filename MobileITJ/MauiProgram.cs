using Microsoft.Extensions.Logging;
using MobileITJ.Services;
using MobileITJ.ViewModels;
using MobileITJ.Views.Auth;
using MobileITJ.Views.Customer;
using MobileITJ.Views.HR;
using MobileITJ.Views.Shared;
using MobileITJ.Views.Worker;
using MobileITJ.Converters;

namespace MobileITJ
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // ---------------------------------------------------------
            // 1. Converters & Services
            // ---------------------------------------------------------
            builder.Services.AddSingleton<SkillsListToStringConverter>();
            builder.Services.AddSingleton<IsNotNullConverter>();
            builder.Services.AddSingleton<BoolToIsActiveConverter>();
            builder.Services.AddSingleton<TimeSpanToStringConverter>();
            builder.Services.AddSingleton<InvertedBoolConverter>();

            // Services
            // Ensure PopupService is implemented using standard DisplayAlert if you don't have the Toolkit
            builder.Services.AddSingleton<IPopupService, PopupService>();
            builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();

            // ---------------------------------------------------------
            // 2. ViewModels
            // ---------------------------------------------------------

            // Auth
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<ChangePasswordViewModel>();
            builder.Services.AddTransient<WelcomeViewModel>();

            // Dashboards
            builder.Services.AddTransient<HRDashboardViewModel>();
            builder.Services.AddTransient<CustomerDashboardViewModel>();
            builder.Services.AddTransient<WorkerDashboardViewModel>();

            // HR (Includes the new Stats ViewModel)
            builder.Services.AddTransient<CreateWorkerViewModel>();
            builder.Services.AddTransient<ViewWorkersViewModel>();
            builder.Services.AddTransient<ViewJobsReportViewModel>(); // HR Stats Logic
            builder.Services.AddTransient<ViewCustomersViewModel>();
            builder.Services.AddTransient<WorkerDetailsViewModel>();
            builder.Services.AddTransient<ReportDetailsViewModel>();
            builder.Services.AddTransient<CustomerDetailsViewModel>();

            // Customer
            builder.Services.AddTransient<CreateJobViewModel>();
            builder.Services.AddTransient<ViewMyJobsViewModel>();
            builder.Services.AddTransient<RateWorkerViewModel>();
            builder.Services.AddTransient<ViewMyJobReportsViewModel>();
            builder.Services.AddTransient<ViewJobApplicationsViewModel>();
            builder.Services.AddTransient<RateJobWorkersViewModel>();

            // Worker
            builder.Services.AddTransient<ViewAvailableJobsViewModel>();
            builder.Services.AddTransient<ViewOngoingJobsViewModel>();
            builder.Services.AddTransient<UpdateProfileViewModel>();
            builder.Services.AddTransient<ViewRatingsViewModel>();
            builder.Services.AddTransient<WalletViewModel>();
            builder.Services.AddTransient<Views.Customer.WorkerPublicProfilePage>();
            builder.Services.AddTransient<ViewModels.WorkerPublicProfileViewModel>();

            // ---------------------------------------------------------
            // 3. Pages
            // ---------------------------------------------------------

            // Auth
            builder.Services.AddTransient<SplashPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<ChangePasswordPage>();
            builder.Services.AddTransient<WelcomePage>();
            builder.Services.AddTransient<DeactivatedAccountPage>();

            // Dashboards
            builder.Services.AddTransient<HRDashboardPage>();
            builder.Services.AddTransient<CustomerDashboardPage>();
            builder.Services.AddTransient<WorkerDashboardPage>();

            // HR (Includes the new Stats Page)
            builder.Services.AddTransient<CreateWorkerPage>();
            builder.Services.AddTransient<ViewWorkersPage>();
            builder.Services.AddTransient<ViewJobsReportPage>(); // HR Stats UI
            builder.Services.AddTransient<ViewCustomersPage>();
            builder.Services.AddTransient<WorkerDetailsPage>();
            builder.Services.AddTransient<ReportDetailsPage>();
            builder.Services.AddTransient<CustomerDetailsPage>();

            // Customer
            builder.Services.AddTransient<CreateJobPage>();
            builder.Services.AddTransient<ViewMyJobsPage>();
            builder.Services.AddTransient<RateWorkerPage>();
            builder.Services.AddTransient<ViewMyJobReportsPage>();
            builder.Services.AddTransient<ViewJobApplicationsPage>();
            builder.Services.AddTransient<RateJobWorkersPage>();

            // Worker
            builder.Services.AddTransient<ViewAvailableJobsPage>();
            builder.Services.AddTransient<ViewOngoingJobsPage>();
            builder.Services.AddTransient<UpdateWorkerProfilePage>();
            builder.Services.AddTransient<ViewRatingsPage>();
            builder.Services.AddTransient<WalletPage>();

            return builder.Build();
        }
    }
}