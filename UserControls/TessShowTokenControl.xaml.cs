using System.Windows;
using System.Windows.Controls;

namespace Tess4Windows.UserControls {

    /// <summary>
    /// Interaktionslogik für TessShowTokenControl.xaml
    /// </summary>
    public partial class TessShowTokenControl : UserControl {

        internal TessShowTokenControl() {
            InitializeComponent();

            tbx_access_token.Text = TessControlManager.Instance.TessApi.LoginResponse.access_token;
            tbx_refresh_token.Text =TessControlManager.Instance.TessApi.LoginResponse.refresh_token;
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e) {
            TessControlManager.Instance.ShowSuitableControl();
        }
    }
}