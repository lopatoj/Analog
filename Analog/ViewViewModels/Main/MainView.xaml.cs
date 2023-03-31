using System;
using System.Collections.Generic;
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
    }
}