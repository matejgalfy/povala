namespace povala_app.Views;

public class BasePage : ContentPage
{
    public BasePage()
    {
        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Logout",
            Command = new Command(async () =>
            {
                SecureStorage.RemoveAll();
                Application.Current!.Windows[0].Page = new NavigationPage(
                    Application.Current.Handler!.MauiContext!.Services
                        .GetRequiredService<LoginPage>()
                );
            })
        });
    }
}