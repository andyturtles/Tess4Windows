using System.Windows;
using Tess4Windows.UserControls;

namespace Tess4Windows {

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow(bool pwMode, TessControlManager.ShowErrorMessage showErrorMsg, ResourceDictionary resources) {
            InitializeComponent();

            TessControlManager.Instance.Init(this.ucFunction, showErrorMsg, resources);

            if ( pwMode ) {
                TessPasswordControl tc = new TessPasswordControl();
                this.ucFunction.Content = tc;
            }
            else {
                TessControlManager.Instance.TessApi.LoadLogin();
                TessControlManager.Instance.ShowSuitableControl();
            }
        }
    }
}