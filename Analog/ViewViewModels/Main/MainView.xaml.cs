using Analog.Patterns.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Analog.ViewViewModels.Main
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainView : ContentPage
    {
        MainViewModel VM;

        public MainView()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
            VM = (MainViewModel)BindingContext;
        }

        private void CaptureImage(object sender, EventArgs e)
        {
            cameraView.Shutter();
        }

        private void MediaCaptured(object sender, MediaCapturedEventArgs e)
        {
            //(sender as Button).IsEnabled = false;

            VM.imgAsBytes = e.ImageData;
            _ = VM.RunInferenceAsync();

            //(sender as Button).IsEnabled = true;
        }

        private async void On_Gallery_Button_Clicked(object sender, EventArgs e)
        {
            (sender as Button).IsEnabled = false;

            var assembly = GetType().Assembly;

            Stream stream = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
            if (stream != null)
            {
                VM.imgAsBytes = null; // needs updating
                _ = VM.RunInferenceAsync();
            }

            (sender as Button).IsEnabled = true;
        }
    }
}