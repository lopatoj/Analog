using Analog.ViewViewModels.Camera;
using Analog.ViewViewModels.Main;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Analog
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new CameraView());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
