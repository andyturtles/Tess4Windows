using System.Windows;

namespace Tess4Windows {

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();

            TessControlManager.Instance.Init(this.ucFunction, App.myApp.ShowError);
            TessControlManager.Instance.ShowSuitableControl();
        }
    }
}