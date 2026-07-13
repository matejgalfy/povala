using povala_app.ViewModels.Auth;

namespace povala_app.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterViewModel registerViewModel)
	{
		InitializeComponent();
        BindingContext = registerViewModel;
    }
}