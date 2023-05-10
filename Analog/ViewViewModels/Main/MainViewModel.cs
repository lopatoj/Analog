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
        public ICommand OnScan { get; set; }
        public ICommand OnGallery { get; set; }

        public MainViewModel()
        {
            Title = Titles.MainTitle;

            _clockScan = new ClockScan();

            OnSettingsClicked = new Command(OnSettingsClickedAsync);
            OnScan = new Command(OnScanClicked);
            OnGallery = new Command(OnGalleryClicked);
        }

        public async Task RunInferenceAsync()
        {
            try
            {
                string result = await _clockScan.GetClassificationAsync(imgAsBytes);
                await Application.Current.MainPage.DisplayAlert("Result", result, "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        public async void OnSettingsClickedAsync(object obj) 
        {
            await Application.Current.MainPage.Navigation.PushAsync(new SettingsView());
        }

        public async void OnScanClicked()
        {
            await Application.Current.MainPage.DisplayAlert("Result", "Time on the clock is 1:51", "OK");
        }

        public async void OnGalleryClicked()
        {
            await Application.Current.MainPage.DisplayAlert("Result", "Time on the clock is 10:08", "OK");
        }
    }
}
