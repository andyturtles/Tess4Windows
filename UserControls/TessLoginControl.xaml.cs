using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using aBasics.WpfBasics;
using TessApi;

namespace Tess4Windows.UserControls {

    /// <summary>
    /// Interaktionslogik für TessLoginControl.xaml
    /// </summary>
    public partial class TessLoginControl : UserControl {

        internal TessLoginControl() {
            InitializeComponent();

            TessLoginUiController cUiCtl = new TessLoginUiController();
            DataContext = cUiCtl;

            tbx_info.Text = @"ATTENTION - New Login Procedure 
Step 1: Click 'Get Login URL'
Step 2: Copy the generated URL from the first textbox (URL to Use) to your preferred browser or use the Button 'Open in Browser'
Step 3: Complete the login procedure in the browser. At the End you will get an 'Page Not Found' Message. THAT IS CORRECT!
Step 4: Copy the complete URL from your Browser to the second textbox (Result URL), the URL must contain some part like '&code=..many characters..'
Step 5: Click the 'Proceed Button'
";
        }
        
        private class TessLoginUiController : UIController {

            public ImageSource img { get { return TessControlManager.Instance.TessBackground; } }

            public override void NotifyPropertyChanged() {
                NotifyPropertyChanged(null); // Alles
            }

        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e) {
            TessControlManager.Instance.ShowSuitableControl();
        }

        private void btn_getLoginUrl_Click(object sender, RoutedEventArgs e) {
            string url = TessControlManager.Instance.TessApi.GetLoginUrl();
            tbx_urlOut.Text           = url;
            btn_openBrowser.IsEnabled = true;
        }

        private async void btn_processReturnUrl_Click(object sender, RoutedEventArgs e) {
            TessApiLoginResult res = await TessControlManager.Instance.TessApi.ContinueLogin(tbx_urlIn.Text);
            if ( res.Success ) {
                TessControlManager.Instance.Settings.CarId = null; // Muss reset erhalten, kann sich wohl ändern
                TessControlManager.Instance.ShowSuitableControl();
            }
            else TessControlManager.Instance.ShowError("Login failed: " + res.Message);
        }

        private void btn_openBrowser_Click(object sender, RoutedEventArgs e) {
            System.Diagnostics.Process.Start(tbx_urlOut.Text);
        }

    }

}
