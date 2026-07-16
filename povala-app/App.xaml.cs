using povala_app.Views;

namespace povala_app
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            //Current?.UserAppTheme = AppTheme.Dark;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var savedRole = SecureStorage.GetAsync("user_role").Result;
            var savedToken = SecureStorage.GetAsync("auth_token").Result;

            Page initialPage;
            if (!string.IsNullOrEmpty(savedToken) && !string.IsNullOrEmpty(savedRole))
            {
                initialPage = savedRole switch
                {
                    "Admin" => new AdminShell(),
                    "GymOwner" => new GymOwnerShell(),
                    _ => new UserShell()
                };
            }
            else
            {
                var loginPage = activationState?.Context.Services.GetRequiredService<LoginPage>();
                initialPage = new NavigationPage(loginPage);
            }
            return new Window(initialPage);
        }
    }
}