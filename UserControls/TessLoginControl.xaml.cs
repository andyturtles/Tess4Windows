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
            TessApiResult res = await TessControlManager.Instance.TessApi.Login(tbx_user.Text, tbx_pass.Password);

            if ( res.Success ) {
                TessControlManager.Instance.ShowSuitableControl();
            }
            else {
                TessControlManager.Instance.ShowError("Login failed: " + res.Message);
            }
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e) {
            TessControlManager.Instance.ShowSuitableControl();
        }
    }
}