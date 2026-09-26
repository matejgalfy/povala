namespace povala_app.Views
{
    public partial class MapPage : ContentPage
    {
        private double _currentScale = 1;
        private double _startScale = 1;
        private double _xOffset = 0;
        private double _yOffset = 0;

        public MapPage()
        {
            InitializeComponent();
        }

        private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
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

        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
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

        private async void OnBoulderTapped(object sender, TappedEventArgs e)
        {
            await DisplayAlertAsync("Prelez", "Klikol si na boulder na interaktívnej mape!", "OK");
        }
    }
}
