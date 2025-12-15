using MobileITJ.Services;

namespace MobileITJ
{
    public partial class App : Application
    {
        private readonly IAuthenticationService _authService;

        public App(IAuthenticationService authService)
        {
            InitializeComponent();
            _authService = authService;

            MainPage = new AppShell(_authService);
        }

        protected override async void OnStart()
        {
            base.OnStart();
            await _authService.InitializeAsync();
        }
    }
}