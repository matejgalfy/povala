using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using povala_app.Services;
using povala_app.Views;
using Povala.Shared.DTOs.Auth;

namespace povala_app.ViewModels.Auth
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly AuthApiService _authApiService;

        [ObservableProperty] public partial string Name { get; set; } = string.Empty;

        [ObservableProperty] public partial string Password { get; set; } = string.Empty;

        [ObservableProperty] public partial string RepeatPassword { get; set; } = string.Empty;

        [ObservableProperty] public partial string Email { get; set; } = string.Empty;

        [ObservableProperty] public partial string ErrorMessage { get; set; } = string.Empty;

        [ObservableProperty] public partial bool HasError { get; set; }

        public RegisterViewModel(AuthApiService authApiService)
        {
            _authApiService = authApiService;
        }

        [RelayCommand]
        async Task RegisterAsync()
        {
            try
            {
                HasError = false;

                if (Password.Length < 8)
                {
                    HasError = true;
                    ErrorMessage = "Password must be at least 8 characters!";
                    return;
                }

                if (Password != RepeatPassword)
                {
                    HasError = true;
                    ErrorMessage = "Password and repeat password must match";
                    return;
                }

                if (string.IsNullOrWhiteSpace(Name))
                {
                    HasError = true;
                    ErrorMessage = "Name can not be empty";
                    return;
                }

                string? error = await _authApiService.RegisterAsync(new RegisterDto()
                {
                    Name = Name,
                    Password = Password,
                    Email = Email
                });

                var page = Application.Current?.Windows[0].Page;
                if (page == null)
                {
                    return;
                }

                if (error != null)
                {
                    HasError = true;
                    ErrorMessage = error;
                }
                else
                    await page.DisplayAlertAsync("Success", "You can now log in", "OK");
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Unexpected error: {ex.Message}";
            }
        }

        [RelayCommand]
        async Task GoToLoginAsync()
        {
            LoginPage? loginPage = Application.Current?.Handler?.MauiContext?.Services.GetService<LoginPage>();
            if (loginPage == null)
                return;

            await Application.Current!.Windows[0].Page!.Navigation.PushAsync(loginPage);
        }
    }
}
