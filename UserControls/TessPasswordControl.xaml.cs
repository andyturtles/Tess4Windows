using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using aBasics;
using aBasics.WpfBasics;
using TessApi;

namespace Tess4Windows.UserControls {

    /// <summary>
    /// Interaktionslogik für TessLoginControl.xaml
    /// </summary>
    public partial class TessPasswordControl : UserControl {

        private class TessPasswordUiController : UIController {
            public ImageSource img { get { return TessControlManager.Instance.TessBackground; } }

            public override void NotifyPropertyChanged() {
                NotifyPropertyChanged(null); // Alles
            }
        }

        internal TessPasswordControl() {
            InitializeComponent();

            TessPasswordUiController cUiCtl = new TessPasswordUiController();
            this.DataContext                = cUiCtl;
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
            TessControlManager.Instance.ExitWithThread();
        }
    }
}