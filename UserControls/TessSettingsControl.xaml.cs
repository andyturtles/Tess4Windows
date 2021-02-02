using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TessApi;
using TessApi.JsonData;

namespace Tess4Windows.UserControls {

    /// <summary>
    /// Interaktionslogik für TessChooseControl.xaml
    /// </summary>
    public partial class TessSettingsControl : UserControl {

        private SettingsUiController cuic;

        internal TessSettingsControl() {
            InitializeComponent();

            if ( TessControlManager.Instance.Settings == null ) TessControlManager.Instance.Settings = new Tess4WinSettings();
            cuic                = new SettingsUiController(TessControlManager.Instance.Settings);
            this.DataContext    = cuic;

            Task.Run(async () => {
                TessApiResult res = await TessControlManager.Instance.TessApi.ListProducts(false);

                if ( !res.Success ) TessControlManager.Instance.ShowError(res.Message);
                else                cuic.NotifyPropertyChanged();
            });

            if ( TessControlManager.Instance.TessApi.IsSleeping ) btn_getCurrentPosForHL.IsEnabled = false;
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
            cuic.SaveSettings(p.id);

            //TessControlManager.Instance.CarChoosen(p.id);
            TessControlManager.Instance.ShowSuitableControl();
        }

        private async void btn_getCurrentPosForHL_Click(object sender, RoutedEventArgs e) {
            if ( TessControlManager.Instance.TessApi.MyCarData == null ) {
                TessApiResult res = await TessControlManager.Instance.TessApi.GetCarInfo(false);
                if ( !res.Success ) {
                    TessControlManager.Instance.ShowError(res.Message);
                    return;
                }
            }

            TessControlManager.Instance.Settings.latHomelink = TessControlManager.Instance.TessApi.MyCarData?.drive_state?.latitude;
            TessControlManager.Instance.Settings.lonHomelink = TessControlManager.Instance.TessApi.MyCarData?.drive_state?.longitude;
            cuic.NotifyPropertyChanged();
        }
    }
}