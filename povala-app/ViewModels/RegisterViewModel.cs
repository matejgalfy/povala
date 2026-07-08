using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;
using Povala.Shared.DTOs.Auth;

namespace povala_app.ViewModels
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

            bool success = await _authApiService.RegisterAsync(new RegisterDto()
            {
                Name = Name,
                Password = Password,
                Email = Email
            });

            if (!success)
                await Shell.Current.DisplayAlertAsync("Error", "Please try again", "OK");
            else
                await Shell.Current.DisplayAlertAsync("Succes", "You can now log in", "OK");
        }
    }
}
