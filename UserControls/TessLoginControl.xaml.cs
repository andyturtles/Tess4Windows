using System.Windows;
using System.Windows.Controls;
using TessApi;

namespace Tess4Windows.UserControls {

    /// <summary>
    /// Interaktionslogik für TessLoginControl.xaml
    /// </summary>
    public partial class TessLoginControl : UserControl {

        internal TessLoginControl() {
            InitializeComponent();
        }

        private async void btn_login_Click(object sender, RoutedEventArgs e) {
            TessApiLoginResult res = await TessControlManager.Instance.TessApi.Login(tbx_user.Text, tbx_pass.Password);
            if ( res.Success ) {
                if ( res.MfaNeeded ) {
                    lbl_mfa.Visibility = Visibility.Visible;
                    tbx_mfaCode.Visibility = Visibility.Visible;
                    btn_mfaLogin.Visibility = Visibility.Visible;

                    btn_login.IsEnabled = false;
                    tbx_user.IsEnabled = false;
                    tbx_pass.IsEnabled = false;
                }
                else {
                    if ( TessControlManager.Instance.Settings != null ) TessControlManager.Instance.Settings.CarId = null; // Muss reseted werden, kann sich wohl ändern
                    TessControlManager.Instance.ShowSuitableControl();
                }
            }
            else {
                TessControlManager.Instance.ShowError("Login failed: " + res.Message);
            }
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e) {
            TessControlManager.Instance.ShowSuitableControl();
        }

        private async void btn_mfaLogin_Click(object sender, RoutedEventArgs e) {
            string code = tbx_mfaCode.Text;

            TessApiLoginResult res = await TessControlManager.Instance.TessApi.ContinueMfaLogin(tbx_mfaCode.Text);
            if ( res.Success ) {
                if ( TessControlManager.Instance.Settings != null ) TessControlManager.Instance.Settings.CarId = null; // Muss reseted werden, kann sich wohl ändern
                TessControlManager.Instance.ShowSuitableControl();
            }
            else {
                TessControlManager.Instance.ShowError("Login failed: " + res.Message);
            }
        }
    }
}