using Microsoft.Maui.Controls.Shapes;
using povala_app.ViewModels;
using Povala.Shared.DTOs.Boulder;
using Povala.Shared.Enums;

namespace povala_app.Views
{
    public partial class MapPage : ContentPage
    {
        private double _currentScale = 1;
        private double _startScale = 1;
        private double _xOffset = 0;
        private double _yOffset = 0;

        public MapPage(MapViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is MapViewModel viewModel)
            {
                await viewModel.LoadDataAsync();
                RenderBoulders();
            }
        }

        private void RenderBoulders()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (BindingContext is not MapViewModel viewModel) return;

            // Remove old boulder dots (keep the Image which is the first child)
            while (ZoomContainer.Children.Count > 1)
            {
                ZoomContainer.Children.RemoveAt(ZoomContainer.Children.Count - 1);
            }

            foreach (var boulder in viewModel.AvailableBoulders)
            {
                var dot = new Ellipse
                {
                    WidthRequest = 20,
                    HeightRequest = 20,
                    Fill = GetBrushForColor(boulder.Color),
                    Stroke = new SolidColorBrush(Colors.White),
                    StrokeThickness = 1
                };

                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += OnBoulderTapped;
                tapGesture.CommandParameter = boulder;
                dot.GestureRecognizers.Add(tapGesture);

                AbsoluteLayout.SetLayoutBounds(dot, new Rect(boulder.CoordX - 10, boulder.CoordY - 10, 20, 20));
                AbsoluteLayout.SetLayoutFlags(dot, Microsoft.Maui.Layouts.AbsoluteLayoutFlags.None);

                ZoomContainer.Children.Add(dot);
            }
            });
        }

        private static SolidColorBrush GetBrushForColor(BoulderColor color)
        {
            return color switch
            {
                BoulderColor.White => new SolidColorBrush(Colors.White),
                BoulderColor.Yellow => new SolidColorBrush(Colors.Yellow),
                BoulderColor.Green => new SolidColorBrush(Colors.LimeGreen),
                BoulderColor.Blue => new SolidColorBrush(Colors.DodgerBlue),
                BoulderColor.Red => new SolidColorBrush(Colors.Red),
                BoulderColor.Black => new SolidColorBrush(Colors.Black),
                BoulderColor.Purple => new SolidColorBrush(Colors.Purple),
                _ => new SolidColorBrush(Colors.Gray)
            };
        }

        private void OnPinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
        {
            if (e.Status == GestureStatus.Started)
            {
                _startScale = ZoomContainer.Scale;
                ZoomContainer.AnchorX = e.ScaleOrigin.X;
                ZoomContainer.AnchorY = e.ScaleOrigin.Y;
            }
            else if (e.Status == GestureStatus.Running)
            {
                // Fix for buggy zoom-out
                _currentScale += (e.Scale - 1) * _startScale;
                _currentScale = Math.Max(1, _currentScale);
                _currentScale = Math.Min(_currentScale, 5);

                ZoomContainer.Scale = _currentScale;
            }
            else if (e.Status == GestureStatus.Completed)
            {
                _xOffset = ZoomContainer.TranslationX;
                _yOffset = ZoomContainer.TranslationY;
                
                if (_currentScale == 1)
                {
                    ZoomContainer.TranslationX = 0;
                    ZoomContainer.TranslationY = 0;
                    _xOffset = 0;
                    _yOffset = 0;
                }
                else 
                {
                    ClampTranslation();
                }
            }
        }

        private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
        {
            if (ZoomContainer.Scale > 1)
            {
                switch (e.StatusType)
                {
                    case GestureStatus.Running:
                        ZoomContainer.TranslationX = _xOffset + e.TotalX;
                        ZoomContainer.TranslationY = _yOffset + e.TotalY;
                        ClampTranslation();
                        break;

                    case GestureStatus.Completed:
                        _xOffset = ZoomContainer.TranslationX;
                        _yOffset = ZoomContainer.TranslationY;
                        break;
                }
            }
        }

        private void ClampTranslation()
        {
            // Calculate the maximum allowed translation based on the current scale
            double maxTranslationX = ZoomContainer.Width * (ZoomContainer.Scale - 1) / 2;
            double maxTranslationY = ZoomContainer.Height * (ZoomContainer.Scale - 1) / 2;

            ZoomContainer.TranslationX = Math.Clamp(ZoomContainer.TranslationX, -maxTranslationX, maxTranslationX);
            ZoomContainer.TranslationY = Math.Clamp(ZoomContainer.TranslationY, -maxTranslationY, maxTranslationY);
        }

        private async void OnBoulderTapped(object? sender, TappedEventArgs e)
        {
            if (e.Parameter is not BoulderDto boulder) return;
            if (BindingContext is not MapViewModel viewModel) return;

            var role = await SecureStorage.GetAsync("user_role");

            if (role == "Admin")
            {
                var action = await this.DisplayActionSheetAsync(
                    $"Manage boulder {boulder.Id} ({boulder.Color}, {boulder.Difficulty})", 
                    "Cancel", 
                    "Delete boulder");

                if (action == "Delete boulder")
                {
                    await viewModel.DeleteBoulderAsync(boulder.Id);
                    RenderBoulders();
                }
            }
            else
            {
                var existingAscent = viewModel.MyAscents.FirstOrDefault(a => a.BoulderId == boulder.Id);
                var existingProject = viewModel.MyProjects.FirstOrDefault(p => p.BoulderId == boulder.Id);

                string ascentOption = existingAscent != null ? "Remove ascent" : "Mark as ascended";
                string projectOption = existingProject != null ? "Remove project" : "Mark as project";

                var action = await this.DisplayActionSheetAsync(
                    $"Boulder {boulder.Id} ({boulder.Color}, {boulder.Difficulty})", 
                    "Cancel", 
                    null, 
                    ascentOption, 
                    projectOption);

                if (action == "Mark as ascended")
                    await viewModel.MarkAscentAsync(boulder.Id);
                else if (action == "Remove ascent" && existingAscent != null)
                    await viewModel.RemoveAscentAsync(existingAscent.Id);
                else if (action == "Mark as project")
                    await viewModel.MarkProjectAsync(boulder.Id);
                else if (action == "Remove project" && existingProject != null)
                    await viewModel.RemoveProjectAsync(existingProject.Id);
            }
        }

        private async void OnMapTapped(object? sender, TappedEventArgs e)
        {
            var role = await SecureStorage.GetAsync("user_role");
            if (role != "Admin") return;

            if (BindingContext is not MapViewModel viewModel) return;

            var position = e.GetPosition(ZoomContainer);
            if (position == null) return;

            double x = position.Value.X;
            double y = position.Value.Y;

            var colorNames = Enum.GetNames<BoulderColor>();
            var selectedColor = await this.DisplayActionSheetAsync(
                $"Add boulder at ({x:F0}, {y:F0}) — Select color:", 
                "Cancel", 
                null, 
                colorNames);

            if (string.IsNullOrEmpty(selectedColor) || selectedColor == "Cancel") return;

            var difficultyNames = Enum.GetNames<BoulderDifficulty>();
            var selectedDifficulty = await this.DisplayActionSheetAsync(
                "Select difficulty:", 
                "Cancel", 
                null, 
                difficultyNames);

            if (string.IsNullOrEmpty(selectedDifficulty) || selectedDifficulty == "Cancel") return;

            var newBoulder = new CreateBoulderDto
            {
                CoordX = x,
                CoordY = y,
                Color = Enum.Parse<BoulderColor>(selectedColor),
                Difficulty = Enum.Parse<BoulderDifficulty>(selectedDifficulty)
            };

            await viewModel.CreateBoulderAsync(newBoulder);
            RenderBoulders();
        }

        private async void OnLogoutClicked(object? sender, EventArgs e)
        {
            SecureStorage.Remove("auth_token");
            SecureStorage.Remove("user_role");

            var loginPage = Handler!.MauiContext!.Services.GetRequiredService<LoginPage>();
            Application.Current!.Windows[0].Page = new NavigationPage(loginPage);
        }
    }
}
