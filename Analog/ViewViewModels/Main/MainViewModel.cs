using Analog.Models;
using Android.Graphics;
using MyFirstProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace Analog.ViewViewModels.Main
{
    class MainViewModel : BaseViewModel
    {
        public ICommand OnCameraClicked { get; set; }
        public ICommand OnGalleryClicked { get; set; }
        public Byte[] imgAsBytes;

        public MainViewModel()
        {
            Title = Titles.MainTitle;

            OnCameraClicked = new Command(OnCameraClickedAsync);
            OnGalleryClicked = new Command(OnGalleryClickedAsync);
        }

        private async void OnCameraClickedAsync(Object obj)
        {

        }

        private async void OnGalleryClickedAsync(Object obj)
        {

        }
    }
}
