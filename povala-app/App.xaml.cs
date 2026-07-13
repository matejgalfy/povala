using povala_app.Views;

namespace povala_app
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Current?.UserAppTheme = AppTheme.Dark;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            bool isUserLoggedIn = false;

            Page initialPage;
            if (isUserLoggedIn)
            {
                initialPage = new UserShell();
            }
            else
            {
                var serviceProvider = activationState?.Context.Services;
                var registerPage = serviceProvider?.GetRequiredService<RegisterPage>();

                initialPage = new NavigationPage(registerPage);
            }

            return new Window(initialPage);
        }
    }
}