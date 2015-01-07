using AeroGear.OAuth2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Shoot
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IWebAuthenticationContinuable
    {
        private MediaCapture captureMgr { get; set; }
        public bool IsPreviewing { get; set; }
        public static MainPage instance;
        public StorageFile file { get; set; }
        public IDictionary<string, Uri> uploadLocation { get; set; }

        public MainPage()
        {
            Window.Current.SizeChanged += Current_SizeChanged;
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            instance = this;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.

            captureMgr = new MediaCapture();
            await captureMgr.InitializeAsync();
            capturePreview.Source = captureMgr;
            await captureMgr.StartPreviewAsync();
            IsPreviewing = true;
            uploadLocation = new Dictionary<string, Uri>();

            DisplayInformation displayInfo = DisplayInformation.GetForCurrentView();
            displayInfo.OrientationChanged += DisplayInfo_OrientationChanged;
            DisplayInfo_OrientationChanged(displayInfo, null);

            var config = await GoogleConfig.Create(
                "517285908032-11moj33qbn01m7sem6g7gmfco2tp252v.apps.googleusercontent.com",
                new List<string>(new string[] { "https://www.googleapis.com/auth/drive" }),
                "google"
            );
            uploadLocation.Add("google", new Uri("https://www.googleapis.com/upload/drive/v2/files"));
            await AccountManager.AddAccount(config);

            var keyCloak = await KeycloakConfig.Create("shoot-third-party", "https://localhost:8443", "shoot-realm");
            uploadLocation.Add("shoot-third-party", new Uri("https://localhost:8443/shoot/rest/photos"));
            await AccountManager.AddKeyCloak(keyCloak);

            var facebook = FacebookConfig.Create("YYY", "XXX",
                    new List<string>(new string[] { "photo_upload, publish_actions" }), "facebook");
            uploadLocation.Add("facebook", new Uri("https://graph.facebook.com/me/photos"));
            await AccountManager.AddFacebook(facebook);
        }

        async void IWebAuthenticationContinuable.ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args)
        {
            await Upload(await AccountManager.ParseContinuationEvent(args));
        }

        private async Task Upload(string moduleName)
        {
            var module = AccountManager.GetAccountByName(moduleName);
            if (await module.RequestAccessAndContinue())
            {
                await Upload(module);
            }
        }

        private async Task Upload(OAuth2Module module)
        {
            var statusBar = StatusBar.GetForCurrentView();
            statusBar.ProgressIndicator.Text = "Uploading";
            await statusBar.ProgressIndicator.ShowAsync();
            using (var client = new HttpClient())
            using (var content = new MultipartFormDataContent())
            {
                client.DefaultRequestHeaders.Authorization = module.AuthenticationHeaderValue();

                var fileContent = new StreamContent((await file.OpenAsync(FileAccessMode.Read)).AsStreamForRead());
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "\"image\"",
                    FileName = "\"" + file.Name + "\""
                };
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");

                content.Add(fileContent);
                HttpResponseMessage response = await client.PostAsync(uploadLocation[module.config.accountId], content);
                string responseString = await response.Content.ReadAsStringAsync();

                Debug.WriteLine(responseString);
                await statusBar.ProgressIndicator.HideAsync();
                await new MessageDialog("uploaded file " + (response.StatusCode != HttpStatusCode.OK ? "un" : "") + "successful").ShowAsync();
            }
        }

        private async void TakePicture(object sender, RoutedEventArgs e)
        {
            var encodingProperties = ImageEncodingProperties.CreateJpeg();
            file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Photo.jpg", CreationCollisionOption.ReplaceExisting);

            await captureMgr.CapturePhotoToStorageFileAsync(encodingProperties, file);
            var bitmap = new BitmapImage();
            bitmap.SetSource(await file.OpenReadAsync());
            shot.Source = bitmap;
            IsPreviewing = false;

            VisualStateManager.GoToState(this, "Taken", true);
        }

        void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            var CurrentViewState = ApplicationView.GetForCurrentView().Orientation;
            switch (CurrentViewState)
            {
                case ApplicationViewOrientation.Landscape:
                    grid.RowDefinitions.Clear();
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(177) });
                    shareBar.ColumnDefinitions.Clear();
                    for (int i = 0; i < 3; i++)
                        shareBar.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    break;

                case ApplicationViewOrientation.Portrait:
                    grid.ColumnDefinitions.Clear();
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(177) });
                    shareBar.RowDefinitions.Clear();
                    for (int i = 0; i < 3; i++)
                        shareBar.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                    break;
            }
        }

        private void DisplayInfo_OrientationChanged(DisplayInformation sender, object args)
        {
            captureMgr.SetPreviewRotation(VideoRotationLookup(sender.CurrentOrientation, false));
        }

        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await CleanupCaptureResources();
            deferral.Complete();
        }

        public async Task CleanupCaptureResources()
        {
            if (IsPreviewing && captureMgr != null)
            {
                await captureMgr.StopPreviewAsync();
                IsPreviewing = false;
            }

            if (captureMgr != null)
            {
                if (capturePreview != null)
                {
                    capturePreview.Source = null;
                }
                captureMgr.Dispose();
            }
        }

        private VideoRotation VideoRotationLookup(DisplayOrientations displayOrientation, bool counterclockwise)
        {
            switch (displayOrientation)
            {
                case DisplayOrientations.Landscape:
                    return VideoRotation.None;

                case DisplayOrientations.Portrait:
                    return (counterclockwise) ? VideoRotation.Clockwise270Degrees : VideoRotation.Clockwise90Degrees;

                case DisplayOrientations.LandscapeFlipped:
                    return VideoRotation.Clockwise180Degrees;

                case DisplayOrientations.PortraitFlipped:
                    return (counterclockwise) ? VideoRotation.Clockwise90Degrees :
                    VideoRotation.Clockwise270Degrees;

                default:
                    return VideoRotation.None;
            }
        }

        private async void ShareGoogle_Click(object sender, RoutedEventArgs e)
        {
            await Upload("google");
        }

        private async void ShareKeycloak_Click(object sender, RoutedEventArgs e)
        {
            await Upload("shoot-third-party");
        }

        private async void ShareFacebook_Click(object sender, RoutedEventArgs e)
        {
            await Upload("facebook");
        }

        internal void GoBack()
        {
            VisualStateManager.GoToState(this, "Initial", true);
            IsPreviewing = true;
        }
    }
}
