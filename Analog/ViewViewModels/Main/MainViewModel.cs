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
        public MainViewModel()
        {
            Title = Titles.MainTitle;
        }

        private void OnCameraClickedAsync(Object obj)
        {

        }

        private void OnGalleryClickedAsync(Object obj)
        {

        }
    }
}
