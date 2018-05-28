using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StudentTask.Uwp.App.Views
{
    public sealed partial class Splash
    {
        public Splash(SplashScreen splashScreen)
        {
            InitializeComponent();
            Window.Current.SizeChanged += (s, e) => Resize(splashScreen);
            Resize(splashScreen);
            Opacity = 0;
        }

        public Splash()
        {
        }

        private void Resize(SplashScreen splashScreen)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (splashScreen.ImageLocation.Top == 0)
            {
                SplashImage.Visibility = Visibility.Collapsed;
                return;
            }

            RootCanvas.Background = null;
            SplashImage.Visibility = Visibility.Visible;
            SplashImage.Height = splashScreen.ImageLocation.Height;
            SplashImage.Width = splashScreen.ImageLocation.Width;
            SplashImage.SetValue(Canvas.TopProperty, splashScreen.ImageLocation.Top);
            SplashImage.SetValue(Canvas.LeftProperty, splashScreen.ImageLocation.Left);
            ProgressTransform.TranslateY = SplashImage.Height / 2;
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            Opacity = 1;
        }
    }
}