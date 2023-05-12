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
            // Takes a picture using the cameraView
            cameraView.Shutter();
        }

        private async void MediaCaptured(object sender, MediaCapturedEventArgs e)
        {
            // Saves the camera's image and runs the model
            VM.imgAsBytes = e.ImageData;
            await VM.RunInferenceAsync(true);
        }

        private async void On_Gallery_Button_Clicked(object sender, EventArgs e)
        {
            // Disables gallery button to prevent false input
            (sender as Button).IsEnabled = false;

            var assembly = GetType().Assembly;

            // Pulls image data stream from the gallery
            Stream stream = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
            using var mStream = new MemoryStream();
            stream.CopyTo(mStream);

            // If stream is not null, run the model
            if (stream != null)
            {
                VM.imgAsBytes = mStream.ToArray();
                await VM.RunInferenceAsync(false);
            }

            // Reenables the gallery button
            (sender as Button).IsEnabled = true;
        }
    }
}