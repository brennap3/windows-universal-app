using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Devices.Geolocation;
using System.Threading.Tasks;
using System.Linq;
using Windows.System;
using System.Net;
using System.Text;
using System.IO;
using System.Collections;
using System.Net.Http;
using System.Collections.Generic;
using GmailSend;
using System.Threading.Tasks;
using EASendMailRT;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Tweetinvi;
using Tweetinvi.Credentials;
using Tweetinvi.Factories;
using Tweetinvi.Controllers.User;
using Tweetinvi.Core.Authentication;





// The Blank Page item template is documented at  
//http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPNavigation {

   /// <summary> 
      /// An empty page that can be used on its own or navigated to within a Frame. 
   /// </summary>
	
   public sealed partial class BlankPage2 : Page {

        string path;
        SQLite.Net.SQLiteConnection conn;
        public BlankPage2(){ 
         this.InitializeComponent();
            path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path,
                      "db.sqlite");
            conn = new SQLite.Net.SQLiteConnection(new
                   SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

        }

        private void Button_Click(object sender, RoutedEventArgs e) {
         this.Frame.Navigate(typeof(MainPage)); 
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
                lat = "Latitude" + longi,
                lng = "Longitude" + lati
            };

            return emp;
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {

            LatLng latlng = await mycoordinatesGPS();
            string message = latlng.lat+latlng.lng + "_last_known_coordinates";
            //txtblock.Text = message;
            //ComposeEmail("brennap3@yahoo.ie",message);
            //ComposeSMS("0858550333",message);
            await sendSMS(message);
            await Send_Email(message);
            await SendTweet(message);
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BlankPage3));
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BlankPage4));
        }

        private async void ComposeEmail(String recipient,
        String messageBody)
        {
            await Launcher.LaunchUriAsync(
            new Uri(
            "mailto:"+recipient+"?subject=SomeSubject&body="+messageBody
            ));

        }
        private async void ComposeSMS(String recipient,
        String messageBody)
        {
            Windows.ApplicationModel.Chat.ChatMessage msg = new Windows.ApplicationModel.Chat.ChatMessage();
            msg.Body = ""+messageBody;
            msg.Recipients.Add(recipient);
            await Windows.ApplicationModel.Chat.ChatMessageManager.ShowComposeSmsMessageAsync(msg);
        }

        private async Task sendSMS(
       String messageBody)
        {
            string username = "brennap3";
            //string username = "724433";
            string password = "0v10Bronco";
            /*
            * Your phone number, including country code, i.e. +44123123123 in this case:
            */
            string msisdn =RetrievePhone();
            //"http://bulksms.vsms.net/eapi/submission/send_sms/2/2.0?username=brennap3&password=0v10Bronco&message=foo&msisdn=353858550333"
            string url = "https://bulksms.vsms.net/eapi/submission/send_sms/2/2.0?";
            string foo = "latlinh";
            string url2 = "http://bulksms.vsms.net/eapi/submission/send_sms/2/2.0?username=brennap3&password=0v10Bronco&message=" + messageBody + "&msisdn=353858550333";
            HttpWebRequest request = WebRequest.Create(url2) as HttpWebRequest;
            //optional
            WebResponse response = await request.GetResponseAsync();
            var stream = response.GetResponseStream();
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

            // Display Result by Diaglog box
            Windows.UI.Popups.MessageDialog dlg = new
                Windows.UI.Popups.MessageDialog(Result);

            await dlg.ShowAsync();
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

        private async Task SendTweet(string messagebody ) {
            
           //var credentials = Auth.SetUserCredentials("233494730-QftupjRFsi8KlfdXafgC8jNWDGu4dlzAyIYMgHqq", "xHmTEPXSQP1MJAGhqyDixHPNHvCvNFbvbzR1WbEsmOYzM", "scp3rgQU7R6K5QMLasMVJwdU8", "UqVgQFiT2Y2KC3JLD2AsfdO8GBOKErJn5KiJCNSurnp3yyF9Kp");
            //var v =  Tweet.PublishTweet("Hello World");

            //Auth.ApplicationCredentials = new TwitterCredentials("233494730-QftupjRFsi8KlfdXafgC8jNWDGu4dlzAyIYMgHqq", "xHmTEPXSQP1MJAGhqyDixHPNHvCvNFbvbzR1WbEsmOYzM", "scp3rgQU7R6K5QMLasMVJwdU8", "UqVgQFiT2Y2KC3JLD2AsfdO8GBOKErJn5KiJCNSurnp3yyF9Kp");
            var creds = new TwitterCredentials("scp3rgQU7R6K5QMLasMVJwdU8",
                "UqVgQFiT2Y2KC3JLD2AsfdO8GBOKErJn5KiJCNSurnp3yyF9Kp",
                "233494730-QftupjRFsi8KlfdXafgC8jNWDGu4dlzAyIYMgHqq",
                "xHmTEPXSQP1MJAGhqyDixHPNHvCvNFbvbzR1WbEsmOYzM"
            );
            

            var tweet = Auth.ExecuteOperationWithCredentials(creds, () =>
                     {
                         var r= Tweet.PublishTweet(messagebody);
                         return r;
                     });

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




    public class LatLng
    {
        public string lat { get; set; }
        public string lng { get; set; }


    }

} 

