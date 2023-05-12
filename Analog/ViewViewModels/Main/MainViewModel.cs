using Analog.Models;
using Analog.ViewViewModels.Settings;
using MyFirstProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace Analog.ViewViewModels.Main
{
    class MainViewModel : BaseViewModel
    {
        public byte[] imgAsBytes;

        private ClockScan _clockScan;

        public ICommand OnSettingsClicked { get; set; }

        public MainViewModel()
        {
            Title = Titles.MainTitle;

            _clockScan = new ClockScan();

            OnSettingsClicked = new Command(OnSettingsClickedAsync);
        }

        public async Task RunInferenceAsync(bool issc)
        {
            // Runs the model with isScan set to whether the image is from the camera or from the gallery
            var result = await _clockScan.GetClassificationAsync(imgAsBytes, issc);
            await Application.Current.MainPage.DisplayAlert("Result", result, "OK");
        }

        public async void OnSettingsClickedAsync(object obj) 
        {
            await Application.Current.MainPage.Navigation.PushAsync(new SettingsView());
        }
    }
}
