using Microsoft.Maui.Storage;
using povala_app.ViewModels.User;

namespace povala_app.Views;

public partial class ProfilePage : ContentPage
{
	public ProfilePage(ProfileViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ProfileViewModel viewModel)
            await viewModel.LoadStatsAsync();
    }

    private void OnLogoutClicked(object? sender, EventArgs e)
    {
        SecureStorage.Remove("auth_token");
        SecureStorage.Remove("user_role");

        var loginPage = Handler!.MauiContext!.Services.GetRequiredService<LoginPage>();
        Application.Current!.Windows[0].Page = new NavigationPage(loginPage);
    }
}
