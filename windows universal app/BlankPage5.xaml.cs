using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UWPNavigation;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace windows_universal_app
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage5 : Page
    {
        
        public BlankPage5()
        {
            this.InitializeComponent();
            //https://www.google.ie/maps/@53.3765557,-6.1840479
            //http://maps.google.com/maps?q=loc:36.26577,-92.54324

            this.InitializeComponent();

            //Uri foo = new Uri("https://www.google.ie/maps/@"+ lat() + "," + lng());

            // webView1.Navigate(foo);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //Dont use this anymore
            //var instance = new RecordingViewModel();
            //Instead now:
            var lati = await lat() ;
            var longi = await lng();
            Uri foo = new Uri("http://maps.google.com/maps?q=loc:" + lati + "," + longi);

            webView1.Navigate(foo);
        }

        private async Task<string> lat()
        {
            LatLng latlng = await mycoordinatesGPS();
            return latlng.lat;
        }
        private async Task<string> lng()
        {
            LatLng latlng = await mycoordinatesGPS();
            return latlng.lng;
        }



        private async Task<LatLng> mycoordinatesGPS()
        {
            Geoposition position;
            var geolocator = new Geolocator();
            position = await geolocator.GetGeopositionAsync();
            var lati = position.Coordinate.Latitude;
            var longi = position.Coordinate.Longitude;

            var emp = new LatLng()
            {
                lat = "" + lati,
                lng = "" + longi
            };

            return emp;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BlankPage2));
        }

        public class LatLng
        {
            public string lat { get; set; }
            public string lng { get; set; }


        }
    }


}
