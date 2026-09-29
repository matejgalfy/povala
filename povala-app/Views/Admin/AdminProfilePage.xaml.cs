using Microsoft.Maui.Storage;

namespace povala_app.Views;

public partial class AdminProfilePage : ContentPage
{
    public AdminProfilePage()
    {
        InitializeComponent();
    }

    private void OnLogoutClicked(object? sender, EventArgs e)
    {
        SecureStorage.Remove("auth_token");
        SecureStorage.Remove("user_role");

        var loginPage = Handler!.MauiContext!.Services.GetRequiredService<LoginPage>();
        Application.Current!.Windows[0].Page = new NavigationPage(loginPage);
    }
}
