using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TessApi;
using TessApi.JsonData;

namespace Tess4Windows.UserControls {

    /// <summary>
    /// Interaktionslogik für TessChooseControl.xaml
    /// </summary>
    public partial class TessChooseControl : UserControl {

        private ChooseUiController cuic;

        internal TessChooseControl() {
            InitializeComponent();

            cuic                = new ChooseUiController();
            this.DataContext    = cuic;

            Task.Run(async () => {
                TessApiResult res = await TessControlManager.Instance.TessApi.ListProducts(false);

                if ( !res.Success ) TessControlManager.Instance.ShowError(res.Message);
                else                cuic.NotifyPropertyChanged();
            });
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e) {
            TessControlManager.Instance.ShowSuitableControl();
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e) {
            if ( dgProds.SelectedItem == null ) {
                TessControlManager.Instance.ShowError("Please choose one!");
                return;
            }

            Product p       = (Product)dgProds.SelectedItem;

            TessControlManager.Instance.CarChoosen(p.id);
            TessControlManager.Instance.ShowSuitableControl();
        }
    }
}