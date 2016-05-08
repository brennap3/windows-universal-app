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
using System;
using System.Collections.Generic;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Media.Editing;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Dropbox.Api;
using Dropbox.Api.Files;
using System.Threading.Tasks;



// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPNavigation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage4 : Page
    {
        public BlankPage4()
        {
            this.InitializeComponent();
        }

        private Windows.Storage.StorageFile storeFile;
        private IRandomAccessStream stream;
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            CameraCaptureUI capture = new CameraCaptureUI();
            capture.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            capture.PhotoSettings.CroppedAspectRatio = new Size(3, 5);
            capture.PhotoSettings.MaxResolution = CameraCaptureUIMaxPhotoResolution.HighestAvailable;
            storeFile = await capture.CaptureFileAsync(CameraCaptureUIMode.Photo);
            if (storeFile != null)
            {
                BitmapImage bimage = new BitmapImage();
                stream = await storeFile.OpenAsync(FileAccessMode.Read); ;
                await bimage.SetSourceAsync(stream);
                captureImage.Source = bimage;



                DateTime dt = DateTime.Now;
                string dtstr = dt.ToString("ddMyyyy");

                string filename = "imagerecordedat" + dtstr;


                try
                {
                    //FileSavePicker save = new FileSavePicker();
                    //save.FileTypeChoices.Add("Image", new List<string>() { ".jpeg" });
                    //save.DefaultFileExtension = ".jpeg";
                    //save.SuggestedFileName = "Image" + filename;
                    //save.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                    //save.SuggestedSaveFile = storeFile;
                    //var s = await save.PickSaveFileAsync();

                    using (var reader = new DataReader(stream.GetInputStreamAt(0)))
                    {
                        await reader.LoadAsync((uint)stream.Size);
                        byte[] buffer = new byte[(int)stream.Size];
                        reader.ReadBytes(buffer);
                        await FileIO.WriteBytesAsync(storeFile, buffer);
                        await Upload(storeFile, buffer);
                    }
                }
                catch (Exception ex) {
                    string BadResult = "somethings gone wrong" + ex;
                    Windows.UI.Popups.MessageDialog dlg = new
                    Windows.UI.Popups.MessageDialog(BadResult);

                    await dlg.ShowAsync();
                }
                this.Frame.Navigate(typeof(BlankPage4));
                }
                
        }

        private async Task Upload(StorageFile storefile, byte[] buffer)
        {
            var dbx = new DropboxClient("nBarWn-vczQAAAAAAAAzlmloAQM3L8SSSCtf4grXUPkiXmIV9wd6IWml-nsAJPsZ");
            //
            var full = await dbx.Users.GetCurrentAccountAsync();
            Stream stream = new MemoryStream(buffer);
            String filename = storefile.Name;
            //String folder = storefile.Path;
            String folder = "/ca1";

            //
            var updated = await dbx.Files.UploadAsync(folder + "/" + filename, WriteMode.Overwrite.Instance, body: stream);

        }

        private async void button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BlankPage2));
        }

    }
}
