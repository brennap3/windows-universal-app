﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UWPNavigation;
using Windows.Devices.Geolocation;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using EASendMailRT;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace windows_universal_app
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage6 : Page
    {
        private Accelerometer myAccelerometer;
        Windows.UI.Popups.MessageDialog dlg;
        string path;
        SQLite.Net.SQLiteConnection conn;
        bool msgsent = false;

        public BlankPage6()
        {
            this.InitializeComponent();
            path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path,
                      "db.sqlite");
            conn = new SQLite.Net.SQLiteConnection(new
                   SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

            myAccelerometer = Accelerometer.GetDefault();
            startReading();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            //

        }
        public async void startReading()
        {
            if (myAccelerometer != null)
            {

                uint minReportInterval = myAccelerometer.MinimumReportInterval;
                uint reportInterval = minReportInterval > 16 ? minReportInterval : 16;
                myAccelerometer.ReportInterval = reportInterval;
                myAccelerometer.ReadingChanged += MyAccelerometer_ReadingChanged;
            }
        }



        private async void MyAccelerometer_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {

            AccelerometerReading reading = args.Reading;
            var xsq = reading.AccelerationX * reading.AccelerationX;
            var ysq = reading.AccelerationY * reading.AccelerationY;
            var zsq = reading.AccelerationZ * reading.AccelerationZ;
            var readprod = xsq + ysq + zsq;
            string Resultaccel = "Panic";

            if (readprod > 1.5 & msgsent==false) //use this to only send once, dont know if you wanna do this multiple messages could be handy
            {
                try
                {
                    msgsent = true;
                    // dlg = new Windows.UI.Popups.MessageDialog(Resultaccel);
                    // await dlg.ShowAsync();
                    myAccelerometer = null;
                    
                    LatLng latlng = await mycoordinatesGPS();
                    string message = latlng.lat + latlng.lng + "_Test_Panic_Distress_Call";
                    await sendSMS(message);
                    await Send_Email(message);

                    CloseApp(); //close the app leave no trace you had this set
                }
                catch (Exception ex) { CloseApp(); } //if an exception occurs you still want to close app as to leave no trace what you did
            }
        }

        private async Task sendSMS(
       String messageBody)
        {
            string username = "brennap3"; //should be in a resource filelogin credentials to Bulk VMS
            //string username = "724433"; //should be in a resource filelogin credentials to Bulk VMS
            string password = "0v10Bronco";
            /*
            * Your phone number, including country code, i.e. +44123123123 in this case:
            */
            string msisdn = RetrievePhone();
            //"http://bulksms.vsms.net/eapi/submission/send_sms/2/2.0?username=brennap3&password=0v10Bronco&message=foo&msisdn=353858550333"
            string url = "https://bulksms.vsms.net/eapi/submission/send_sms/2/2.0?";
            string foo = "latlinh";
            string url2 = "http://bulksms.vsms.net/eapi/submission/send_sms/2/2.0?username=brennap3&password=0v10Bronco&message=" + messageBody + "&msisdn=353858550333";
            HttpWebRequest request = WebRequest.Create(url2) as HttpWebRequest;
            //optional
            WebResponse response = await request.GetResponseAsync();
            var stream = response.GetResponseStream();
        }

        private String RetrievePhone()
        {
            var query = conn.Table<Contact>();
            string id = "";
            string phone = "";
            string email = "";

            foreach (var message in query)
            {
                id = id + " " + message.Id;
                phone = phone + " " + message.phone;
                email = email + " " + message.email;
                break;
            }

            return phone;
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
                lat = "Latitude" + lati,
                lng = "Longitude" + longi
            };

            return emp;
        }

        public void CloseApp()
        {
            Application.Current.Exit();
        }

        private async Task Send_Email(string messageBody)
        {
            String Result = "";
            try
            {
                SmtpMail oMail = new SmtpMail("TryIt");
                SmtpClient oSmtp = new SmtpClient();

                // Set sender email address, please change it to yours
                oMail.From = new MailAddress("peter.brennan74@gmail.com");

                // Add recipient email address, please change it to yours
                oMail.To.Add(new MailAddress(RetrieveEmail()));

                // Set email subject and email body text
                oMail.Subject = "This is an emergency contact email";
                oMail.TextBody = messageBody;

                // Your SMTP server address
                SmtpServer oServer = new SmtpServer("smtp.gmail.com");

                // User and password for SMTP authentication            
                // these should be in a reource file or else add them via SQLite
                oServer.User = "peter.brennan74@gmail.com";
                oServer.Password = "0v10Bronco";

                // If your SMTP server requires TLS connection on 25 port, please add this line
                // oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;

                // If your SMTP server requires SSL connection on 465 port, please add this line
                oServer.Port = 465;
                oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;

                await oSmtp.SendMailAsync(oServer, oMail);
                Result = "Email was sent successfully!";
            }
            catch (Exception ep)
            {
                Result = String.Format("Failed to send email with the following error: {0}", ep.Message);
            }
        }

        private String RetrieveEmail()
        {
            var query = conn.Table<Contact>();
            string id = "";
            string phone = "";
            string email = "";

            foreach (var message in query)
            {
                id = id + " " + message.Id;
                phone = phone + " " + message.phone;
                email = email + " " + message.email;
                break;
            }

            return email;
        }


        }
    }

