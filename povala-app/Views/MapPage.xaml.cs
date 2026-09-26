using povala_app.ViewModels;
using Povala.Shared.DTOs.Boulder;

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
            }
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
                _currentScale += (e.Scale - 1) * _startScale;
                _currentScale = Math.Max(1, _currentScale);
                _currentScale = Math.Min(_currentScale, 5);

                ZoomContainer.Scale = _currentScale;
            }
            else if (e.Status == GestureStatus.Completed)
            {
                if (_currentScale == 1)
                {
                    ZoomContainer.TranslationX = 0;
                    ZoomContainer.TranslationY = 0;
                    _xOffset = 0;
                    _yOffset = 0;
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
                        break;

                    case GestureStatus.Completed:
                        _xOffset = ZoomContainer.TranslationX;
                        _yOffset = ZoomContainer.TranslationY;
                        break;
                }
            }
        }

        private async void OnBoulderTapped(object? sender, TappedEventArgs e)
        {
            if (e.Parameter is not BoulderDto boulder) return;
            if (BindingContext is not MapViewModel viewModel) return;

            var role = await SecureStorage.GetAsync("user_role");

            if (role == "Admin")
            {
                var action = await Application.Current!.Windows[0].Page!.DisplayActionSheetAsync(
                    $"Manage boulder {boulder.Id} ({boulder.Color}, {boulder.Difficulty})", 
                    "Cancel", 
                    "Delete boulder");

                if (action == "Delete boulder")
                {
                    await viewModel.DeleteBoulderAsync(boulder.Id);
                }
            }
            else // Role: User
            {
                var existingAscent = viewModel.MyAscents.FirstOrDefault(a => a.BoulderId == boulder.Id);
                var existingProject = viewModel.MyProjects.FirstOrDefault(p => p.BoulderId == boulder.Id);

                string ascentOption = existingAscent != null ? "Remove ascent" : "Mark as ascended";
                string projectOption = existingProject != null ? "Remove project" : "Mark as project";

                var action = await Application.Current!.Windows[0].Page!.DisplayActionSheetAsync(
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
    }
}
