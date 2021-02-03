using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TessApi;
using TessApi.JsonData;

namespace Tess4Windows.UserControls {

    /// <summary>
    /// Interaktionslogik für TessSettingsControl.xaml
    /// </summary>
    public partial class TessSettingsControl : UserControl {

        private SettingsUiController cuic;

        internal TessSettingsControl() {
            InitializeComponent();
            this.dgProds.DataContextChanged += DgProds_DataContextChanged;
            this.dgProds.SelectionChanged += DgProds_SelectionChanged;

            if ( TessControlManager.Instance.Settings == null ) TessControlManager.Instance.Settings = new Tess4WinSettings();
            cuic                = new SettingsUiController(TessControlManager.Instance.Settings);
            this.DataContext    = cuic;

            Task.Run(async () => {
                TessApiResult res = await TessControlManager.Instance.TessApi.ListProducts(false);

                if ( !res.Success ) TessControlManager.Instance.ShowError(res.Message);
                else                cuic.NotifyPropertyChanged();
            });

            if ( ( TessControlManager.Instance.TessApi.MyCar == null ) || TessControlManager.Instance.TessApi.IsSleeping ) {
                btn_getCurrentPosForHL.IsEnabled = false;
                btn_getCurrentPosForWin.IsEnabled = false;
            }
        }

        private void DgProds_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            SetSelectedItem();
        }

        private void DgProds_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => SetSelectedItem()));
        }

        private void SetSelectedItem() {
            if ( dgProds.Items.Count == 1 ) dgProds.SelectedItem = dgProds.Items[0];
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

            TessControlManager.Instance.ShowSuitableControl();
        }

        private async Task RefreshCarData() {
            if ( TessControlManager.Instance.TessApi.MyCarData == null ) {
                TessApiResult res = await TessControlManager.Instance.TessApi.GetCarInfo(false);
                if ( !res.Success ) {
                    TessControlManager.Instance.ShowError(res.Message);
                    return;
                }
            }
        }

        private async void btn_getCurrentPosForHL_Click(object sender, RoutedEventArgs e) {
            await RefreshCarData();

            TessControlManager.Instance.Settings.latHomelink = TessControlManager.Instance.TessApi.MyCarData?.drive_state?.latitude;
            TessControlManager.Instance.Settings.lonHomelink = TessControlManager.Instance.TessApi.MyCarData?.drive_state?.longitude;
            cuic.NotifyPropertyChanged();
        }

        private async void btn_getCurrentPosForWin_Click(object sender, RoutedEventArgs e) {
            await RefreshCarData();

            TessControlManager.Instance.Settings.latWindows = TessControlManager.Instance.TessApi.MyCarData?.drive_state?.latitude;
            TessControlManager.Instance.Settings.lonWindows = TessControlManager.Instance.TessApi.MyCarData?.drive_state?.longitude;
            cuic.NotifyPropertyChanged();
        }

        private void btn_showToken_Click(object sender, RoutedEventArgs e) {
            TessShowTokenControl tst = new TessShowTokenControl();
            TessControlManager.Instance.ShowControl(tst);
        }
    }
}