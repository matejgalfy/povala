using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using povala_app.Services;
using povala_app.Views;
using Povala.Shared.DTOs.Auth;

namespace povala_app.ViewModels.Auth
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthApiService _authApiService;

        [ObservableProperty] public partial string Email { get; set; } = string.Empty;

        [ObservableProperty] public partial string Password { get; set; } = string.Empty;

        [ObservableProperty] public partial string ErrorMessage { get; set; } = string.Empty;

        [ObservableProperty]
        public partial bool HasError { get; set; }

        public LoginViewModel(AuthApiService authApiService)
        {
            _authApiService = authApiService;
        }


        [RelayCommand]
        async Task LoginAsync()
        {
            HasError = false;

            if (Password.Length < 8)
            {
                HasError = true;
                ErrorMessage = "Password must be at least 8 characters!";
                return;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                HasError = true;
                ErrorMessage = "Email can not be empty";
                return;
            }

            (string? error, string? role) = await _authApiService.LoginAsync(new LoginDto
            {
                Email = Email,
                Password = Password
            });

            if (error != null)
            {
                HasError = true;
                ErrorMessage = error;
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Application.Current!.Windows[0].Page = role switch
                    {
                        "Admin" => new AdminShell(),
                        _ => new UserShell()
                    };
                });
            }
        }

        [RelayCommand]
        async Task GoToRegisterAsync()
        {
            RegisterPage? registerPage = Application.Current?.Handler?.MauiContext?.Services.GetService<RegisterPage>();
            if (registerPage == null)
                return;

            await Application.Current!.Windows[0].Page!.Navigation.PushAsync(registerPage);
        }
    }
}
