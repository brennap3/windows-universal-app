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


// The Blank Page item template is documented at
//  http://go.microsoft.com/fwlink/?LinkId=234238 

namespace UWPNavigation
{

    /// <summary> 
    /// An empty page that can be used on its own or navigated to within a Frame. 
    /// </summary> 

        public sealed partial class BlankPage1 : Page
        {
            string path;
            SQLite.Net.SQLiteConnection conn;

            public BlankPage1()
            {
                this.InitializeComponent();
                path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path,
                   "db.sqlite");
                conn = new SQLite.Net.SQLiteConnection(new
                   SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
                conn.CreateTable<Contact>();
            }

            private void Retrieve_Click(object sender, RoutedEventArgs e)
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
                }

                textBlock2.Text = "ID: " + id + "\nphone: " + phone + "\nemail: " + email;
            }

            private void Add_Click(object sender, RoutedEventArgs e)
            {

                var s = conn.Insert(new Contact()
                {
                    email = textBox.Text,
                    phone = textBox1.Text
                });

            }

            private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }



    }

}

public class Contact
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string email { get; set; }
    public string phone { get; set; }
}
