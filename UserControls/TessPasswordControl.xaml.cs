using System;
using System.Windows;
using System.Windows.Controls;
using aBasics;
using TessApi;

namespace Tess4Windows.UserControls {

    /// <summary>
    /// Interaktionslogik für TessLoginControl.xaml
    /// </summary>
    public partial class TessPasswordControl : UserControl {

        internal TessPasswordControl() {
            InitializeComponent();
        }

        private void btn_proceed_Click(object sender, RoutedEventArgs e) {
            if ( !String.IsNullOrEmpty(tbx_pass.Password) ) {
                TessTools.SetEncryptEntropy(tbx_pass.Password);
                try {
                    TessControlManager.Instance.TessApi.LoadLogin();
                    TessControlManager.Instance.ShowSuitableControl();
                }
                catch ( Exception ex) {
                    Log.Error("TessPasswordControl", ex);
                    TessControlManager.Instance.ShowError("Error Decrypting API Token: " + ex.Message);
                }
            }
        }

        private void btn_quit_Click(object sender, RoutedEventArgs e) {
            App.myApp.ExitWithThread();
        }
    }
}