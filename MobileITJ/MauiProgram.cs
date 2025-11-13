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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSans-Semibold.ttf");
                });

            // Converters
            builder.Services.AddSingleton<SkillsListToStringConverter>();
            builder.Services.AddSingleton<IsNotNullConverter>();
            builder.Services.AddSingleton<BoolToIsActiveConverter>();
            builder.Services.AddSingleton<TimeSpanToStringConverter>();

            // --- 👇 REGISTER THE NEW SERVICE 👇 ---
            builder.Services.AddSingleton<IPopupService, PopupService>();

            // Service
            builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();

            // Auth ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<ChangePasswordViewModel>();
            builder.Services.AddTransient<WelcomeViewModel>();

            // Dashboard ViewModels
            builder.Services.AddTransient<HRDashboardViewModel>();
            builder.Services.AddTransient<CustomerDashboardViewModel>();
            builder.Services.AddTransient<WorkerDashboardViewModel>();

            // HR ViewModels
            builder.Services.AddTransient<CreateWorkerViewModel>();
            builder.Services.AddTransient<ViewWorkersViewModel>();
            builder.Services.AddTransient<ViewJobsReportViewModel>();
            builder.Services.AddTransient<ViewCustomersViewModel>();

            // Customer ViewModels
            builder.Services.AddTransient<CreateJobViewModel>();
            builder.Services.AddTransient<ViewMyJobsViewModel>();
            builder.Services.AddTransient<RateWorkerViewModel>();
            builder.Services.AddTransient<ViewMyJobReportsViewModel>();
            builder.Services.AddTransient<ViewJobApplicationsViewModel>();
            builder.Services.AddTransient<RateJobWorkersViewModel>();

            // Worker ViewModels
            builder.Services.AddTransient<ViewAvailableJobsViewModel>();
            builder.Services.AddTransient<ViewOngoingJobsViewModel>();
            builder.Services.AddTransient<UpdateProfileViewModel>();
            builder.Services.AddTransient<ViewRatingsViewModel>();
            builder.Services.AddTransient<WalletViewModel>();

            // Auth Pages
            builder.Services.AddTransient<SplashPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<ChangePasswordPage>();
            builder.Services.AddTransient<WelcomePage>();
            builder.Services.AddTransient<DeactivatedAccountPage>();

            // Dashboard Pages
            builder.Services.AddTransient<HRDashboardPage>();
            builder.Services.AddTransient<CustomerDashboardPage>();
            builder.Services.AddTransient<WorkerDashboardPage>();

            // HR Pages
            builder.Services.AddTransient<CreateWorkerPage>();
            builder.Services.AddTransient<ViewWorkersPage>();
            builder.Services.AddTransient<ViewJobsReportPage>();
            builder.Services.AddTransient<ViewCustomersPage>();

            // Customer Pages
            builder.Services.AddTransient<CreateJobPage>();
            builder.Services.AddTransient<ViewMyJobsPage>();
            builder.Services.AddTransient<RateWorkerPage>();
            builder.Services.AddTransient<ViewMyJobReportsPage>();
            builder.Services.AddTransient<ViewJobApplicationsPage>();
            builder.Services.AddTransient<RateJobWorkersPage>();

            // Worker Pages
            builder.Services.AddTransient<ViewAvailableJobsPage>();
            builder.Services.AddTransient<ViewOngoingJobsPage>();
            builder.Services.AddTransient<UpdateWorkerProfilePage>();
            builder.Services.AddTransient<ViewRatingsPage>();
            builder.Services.AddTransient<WalletPage>();

            return builder.Build();
        }
    }
}