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
using Dropbox.Api;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Dropbox.Api.Files;
using Windows.Devices.Sensors;
using Windows.Foundation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPNavigation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage3 : Page
    {
        private Windows.Storage.StorageFile storeFile;  
        private IRandomAccessStream stream;
        

        public BlankPage3()
        {
            this.InitializeComponent();
        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            CameraCaptureUI video = new CameraCaptureUI();
            video.VideoSettings.Format = CameraCaptureUIVideoFormat.Mp4;
            video.VideoSettings.MaxResolution = CameraCaptureUIMaxVideoResolution.HighDefinition;
            storeFile = await video.CaptureFileAsync(CameraCaptureUIMode.Video);
            if (storeFile != null)
            {
                stream = await storeFile.OpenAsync(FileAccessMode.Read);
                MediaClip media = await MediaClip.CreateFromFileAsync(storeFile);
                MediaComposition mComposition = new MediaComposition();
                mComposition.Clips.Add(media);
                MediaStreamSource source = mComposition.GeneratePreviewMediaStreamSource((int)demoVideo.ActualWidth, (int)demoVideo.ActualHeight);
                demoVideo.SetMediaStreamSource(source);


                DateTime dt = DateTime.Now;
                string dtstr = dt.ToString("ddMyyyy");

                string filename = "videorecordedat" + dtstr;


                //FileSavePicker save = new FileSavePicker();
                //save.FileTypeChoices.Add("Video", new List<string>() { ".mp4", ".wmv" });
                //save.DefaultFileExtension = ".mp4";
                //save.SuggestedFileName = "video"+filename;
                //save.SuggestedStartLocation = PickerLocationId.VideosLibrary;
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



                this.Frame.Navigate(typeof(BlankPage3));
            }
        }

        private async Task Upload(StorageFile storefile, byte[] buffer )
        {
            var dbx = new DropboxClient("nBarWn-vczQAAAAAAAAzlmloAQM3L8SSSCtf4grXUPkiXmIV9wd6IWml-nsAJPsZ");
            //
            var full = await dbx.Users.GetCurrentAccountAsync();
            Stream stream = new MemoryStream(buffer);
            String filename = storefile.Name;
            //String folder = storefile.Path;
            String folder = "/ca1";

            //
            var updated = await dbx.Files.UploadAsync(folder + "/" + filename,WriteMode.Overwrite.Instance,body: stream);
            
        }

    private async void button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BlankPage2));
        }


    }
}
