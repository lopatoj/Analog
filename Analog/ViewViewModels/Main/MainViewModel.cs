using Analog.Models;
using MyFirstProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Analog.ViewViewModels.Main
{
    class MainViewModel : BaseViewModel
    {
        public ICommand OnCameraClicked { get; set; }
        public ICommand OnGalleryClicked { get; set; }

        public MainViewModel()
        {
            Title = Titles.MainTitle;

            OnCameraClicked = new Command(OnCameraClickedAsync);
            OnGalleryClicked = new Command(OnGalleryClickedAsync);
        }

        private async void OnCameraClickedAsync(Object obj)
        {

            await Application.Current.MainPage.Navigation.PushAsync(new LayoutView());

        }

        private async void OnGalleryClickedAsync(Object obj)
        {

            await Application.Current.MainPage.Navigation.PushAsync(new LayoutView());

        }
    }
}
