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

        private void CaptureImage(object Sender, EventArgs e)
        {
            cameraView.Shutter();
        }

        private void MediaCaptured(object Sender, MediaCapturedEventArgs e)
        {
            VM.imgAsBytes = e.ImageData;
            _ = VM.RunInferenceAsync();
        }

        private void Camera_Button_Clicked(object sender, EventArgs e)
        {
            cameraView.Shutter();
            
        }

        private void cameraView_MediaCaptured(object sender, Xamarin.CommunityToolkit.UI.Views.MediaCapturedEventArgs e)
        {
            Console.WriteLine("done");
            ImageSource newimg = e.Image;
            
        }

        private async void On_Gallery_Button_Clicked(object sender, EventArgs e)
        {
            ImageSource image = null;
            (sender as Button).IsEnabled = false;

            Stream stream = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
            if (stream != null)
            {
                image = ImageSource.FromStream(() => stream);
            }

            (sender as Button).IsEnabled = true;

        }
    }
}