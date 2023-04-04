using Analog.Models;
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
        public Byte[] imgAsBytes;

        private ClockScan _clockScan;

        public MainViewModel()
        {
            Title = Titles.MainTitle;

            _clockScan = new ClockScan();
        }

        public async Task RunInferenceAsync()
        {
            try
            {
                var result = await _clockScan.GetClassificationAsync(imgAsBytes);
                await Application.Current.MainPage.DisplayAlert("Result", result, "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.StackTrace, "OK");
            }
        }
    }
}
