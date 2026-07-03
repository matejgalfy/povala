using Microsoft.Extensions.DependencyInjection;

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
            return new Window(new AppShell());
        }
    }
}