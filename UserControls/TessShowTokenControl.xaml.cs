using System.Windows;
using System.Windows.Controls;
using aBasics;
using TessApi;
using TessApi.JsonData;

namespace Tess4Windows.UserControls {

    /// <summary>
    /// Interaktionslogik für TessShowTokenControl.xaml
    /// </summary>
    public partial class TessShowTokenControl : UserControl {

        internal TessShowTokenControl() {
            InitializeComponent();

            if ( TessControlManager.Instance.TessApi.LoginResponse != null ) {
                tbx_access_token.Text  = TessControlManager.Instance.TessApi.LoginResponse.access_token;
                tbx_refresh_token.Text = TessControlManager.Instance.TessApi.LoginResponse.refresh_token;

                string json = SerializeTool.SerializeJson(TessControlManager.Instance.TessApi.LoginResponse);
                tbx_loginJson.Text = json;
            }
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e) {
            TessControlManager.Instance.ShowSettings();
        }

        private void Btn_saveJson_OnClick(object sender, RoutedEventArgs e) {
            TessControlManager.Instance.TessApi.LoginResponse = SerializeTool.DeSerializeJson<LoginResponse>(tbx_loginJson.Text);
            TessTools.SaveResponse(TessControlManager.Instance.TessApi.LoginResponse, null, true);
            TessControlManager.Instance.ShowSettings();
        }

        private async void Btn_doTokenRefresh_OnClick(object sender, RoutedEventArgs e) {
            
            TessApiLoginResult res = await TessControlManager.Instance.TessApi.RefreshToken();
            //if ( res.Success ) {
            //}
            //else TessControlManager.Instance.ShowError("Login failed: " + res.Message);

        }

    }

}
