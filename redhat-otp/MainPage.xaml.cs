using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AeroGear.OTP;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace redhat_otp
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private const string Pass = "X4GNFRC6RLTBVX7YSZFZIZ7L6MB7MRIE5A53227P3H2EZW3YM3MA";
        private const string CounterKey = "counter";

        public MainPage()
        {
            InitializeComponent();
            GenerateOtp();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            GenerateOtp();
        }

        private void GenerateOtp()
        {
            var settings = ApplicationData.Current.LocalSettings.Values;
            var counter = 0L;
            if (settings.ContainsKey(CounterKey))
            {
                counter = (long) settings[CounterKey];
            }
            Otp.Text = new Hotp(Pass, counter).now();
            settings[CounterKey] = ++counter;
        }
    }
}