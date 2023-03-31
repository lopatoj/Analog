using Analog.Models;
using MyFirstProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace Analog.ViewViewModels.Main
{
    class MainViewModel : BaseViewModel
    {
        public ICommand OnCameraClicked { get; set; }
        public ICommand OnGalleryClicked { get; set; }
        public ICommand ShutterCommand { get; set; }

        public MainViewModel()
        {
            Title = Titles.MainTitle;

            OnCameraClicked = new Command(OnCameraClickedAsync);
            OnGalleryClicked = new Command(OnGalleryClickedAsync);
            ShutterCommand = new Command(OnCameraClickedAsync);
        }

        private async void OnCameraClickedAsync(Object obj)
        {
            
        }

        private async void OnGalleryClickedAsync(Object obj)
        {

        }
    }
}
