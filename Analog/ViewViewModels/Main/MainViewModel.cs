using Analog.Models;
using MyFirstProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Analog.ViewViewModels.Main
{
    class MainViewModel : BaseViewModel
    {
        public ICommand OnCameraClicked { get; set; }
        public ICommand OnGalleryClicked { get; set; }
        public Byte[] imgAsBytes;

        ClockScan _clockScan;

        public MainViewModel()
        {
            Title = Titles.MainTitle;

            OnCameraClicked = new Command(OnCameraClickedAsync);
            OnGalleryClicked = new Command(OnGalleryClickedAsync);

            _clockScan = new ClockScan();
        }

        async Task RunInferenceAsync()
        {
            try
            {
                var image = imgAsBytes;
                var result = await _clockScan.GetClassificationAsync(image);
                await Application.Current.MainPage.DisplayAlert("The time on your clock is:", result, "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnCameraClickedAsync(Object obj)
        {

        }

        private async void OnGalleryClickedAsync(Object obj)
        {

        }
    }
}
