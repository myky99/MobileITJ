using Microsoft.Extensions.Logging;
using MobileITJ.Services;
using MobileITJ.Views;
using MobileITJ.Views.Auth;
using MobileITJ.Views.HR;
using MobileITJ.Views.Worker;
using MobileITJ.Views.Customer;

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

 // Register application services
 builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();

 // Register pages with DI so Shell routing and constructor injection work
 builder.Services.AddTransient<MainPage>();
 builder.Services.AddTransient<SplashPage>();
 builder.Services.AddTransient<LoginPage>();
 builder.Services.AddTransient<RegisterPage>();
 builder.Services.AddTransient<ChangePasswordPage>();
 builder.Services.AddTransient<HRDashboardPage>();
 builder.Services.AddTransient<WorkerDashboardPage>();
 builder.Services.AddTransient<CustomerDashboardPage>();

 return builder.Build();
 }
 }
}
