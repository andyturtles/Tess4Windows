using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using aBasics;
using aBasics.WpfBasics;
using TessApi;

namespace Tess4Windows.UserControls {

    /// <summary>
    /// UI Controller für das Tesla Window
    ///
    /// TODOs:
    /// Auto - Refresh: Sleep!!
    ///
    /// Charge/heat Later
    /// Streaming API?
    ///
    /// </summary>
    /// <seealso cref="aBasics.WpfBasics.UIController" />
    internal partial class TessUiController : UIController {

        private const bool LOAD_FROM_DISK = false; // Debug / testing without bothering Tesla

        private MyTess myTess { get { return TessControlManager.Instance.TessApi; } }
        private Tess4WinSettings settings { get { return TessControlManager.Instance.Settings; } }

        public ImageSource img { get { return TessControlManager.Instance.TessBackground; } }

        public TessUiController() {
            if ( myTess.IsLoggedIn ) RefreshData(true);

            InitRefresh();
        }

        private async void RefreshData(bool setChargeLimitInput) {
            AnimateRefresh();

            await Task.Run(async () => {
                TessApiResult res = await myTess.ListProducts(LOAD_FROM_DISK);
                if ( !res.Success ) {
                    HandleError(res);
                    return;
                }

                if ( !myTess.IsSleeping ) res = await myTess.GetCarInfo(LOAD_FROM_DISK);
                else {
                    Log.Info("Tess is sleeping, going to load last Info");
                    res = await myTess.GetCarInfo(true);
                }

                if ( !res.Success )  HandleError(res);
                else if ( setChargeLimitInput ) ChargeLimit = myTess.MyCarData?.charge_state?.charge_limit_soc ?? 0;
            });

            NotifyPropertyChanged();
        }

        private bool AreUSure(string msg) {
            return ( MessageBox.Show(msg, "Sure?", MessageBoxButton.OKCancel) == MessageBoxResult.OK );
        }

        public Brush ForegroundLables { get {
                if ( myTess.IsSleeping ) return Brushes.SteelBlue;
                if ( myTess.MyCarData == null ) return Brushes.DimGray;
                return Brushes.WhiteSmoke;
            }
        }

        #region Login / Wakeup

        public string LoginTill { get { return "Login till: " + myTess.LoginResponse?.TokenExpiryDate.ToShortDateString(); } }
        public string CarNameAndState { get { return myTess.MyCar?.display_name + " is " + myTess.MyCar?.state; } }

        public string CarVersion { get { return ( myTess.MyCarData?.vehicle_state == null ) ? "Version: XXXX.xx.XX.x" : "Version: " + myTess.MyCarData?.vehicle_state?.car_version_short; } }

        // Wenn schlafend, anzeigen von wann die letzten Daten sind
        public string LastDataDate { get { return myTess.DiskDataDate?.ToString(); } }

        public Visibility LastDataDateVisible { get { return myTess.IsSleeping ? Visibility.Visible : Visibility.Hidden; } }

        public ICommand WakeUpCmd {
            get { return new RelayCommand(p => WakeUp(), p => myTess.IsSleeping); }
        }

        public ICommand RefreshCmd {
            get { return new RelayCommand(p => RefreshData(false)); }
        }

        private async void WakeUp() {
            if ( !AreUSure("Sure?") ) return;
            if ( LOAD_FROM_DISK ) return;

            AutoRefreshTimerEnabled = true;
            NotifyPropertyChanged();

            await myTess.WakeUp();
        }

        public ICommand RenewLoginCmd {
            get { return new RelayCommand(p => { TessControlManager.Instance.ShowLogin(); }); }
        }

        public ICommand ShowSettingsCmd {
            get { return new RelayCommand(p => { TessControlManager.Instance.ShowSettings(); }); }
        }

        #endregion Login / Wakeup

        public string Drive {
            get {
                string drv = "[" + ( myTess.MyCarData?.drive_state?.shift_state ?? "\u24C5" ) + "] ";
                if ( myTess.MyCarData?.drive_state?.speed.HasValue ?? false ) drv += UiConversion.GetValueStringMiToKm(myTess.MyCarData?.drive_state?.speed);
                else drv += "0";
                drv += " km/h ";
                drv += " - ";
                if ( myTess.MyCarData?.drive_state?.power.HasValue ?? false ) drv += UiConversion.GetValueString(myTess.MyCarData?.drive_state?.power);
                else drv += "0";
                drv += " kW";
                return drv;
            }
        }

        public Visibility HomelinkVisible { get { return TessControlManager.Instance.Settings.HasHomelinkCoordinates ? Visibility.Visible : Visibility.Hidden; } }

        public ICommand TriggerHomelinkCmd {
            get { return new RelayCommand(async p => { HandleResultAndRefresh(await myTess.TriggerHomelink(settings.latHomelink.Value, settings.lonHomelink.Value)); }, p => ( !myTess.IsSleeping )); }
        }

        public ICommand WhereIsTessCmd {
            get { return new RelayCommand(p => WhereIsTess(), P => ( myTess.MyCarData?.drive_state != null )); }
        }

        private void WhereIsTess() {
            //string url                  = $"https://maps.google.com/?q={TessTools.GetDoubleAsString(myTess.MyCarData.drive_state.latitude.Value)},{TessTools.GetDoubleAsString(myTess.MyCarData.drive_state.longitude.Value)}";
            //BrowserControl bc           = new BrowserControl(null, url);
            //App.myApp.myMainWindow.ucFunction.Content = bc;
#warning TODO!
        }

        #region Temp, A/C ...

        public string InsideTemp { get { return "Innen: " + UiConversion.GetValueString(myTess.MyCarData?.climate_state?.inside_temp) + " °C"; } }
        public string OutsideTemp { get { return "Aussen: " + UiConversion.GetValueString(myTess.MyCarData?.climate_state?.outside_temp) + " °C"; } }

        public string TempMeas {
            get {
                return "In / Out: " + UiConversion.GetValueString(myTess.MyCarData?.climate_state?.inside_temp) + "°C / " + UiConversion.GetValueString(myTess.MyCarData?.climate_state?.outside_temp) + " °C";
            }
        }

        public string TempSetting { get { return "Temp. (L/R): " + UiConversion.GetValueString(myTess.MyCarData?.climate_state?.driver_temp_setting, false) + " °C / " +
                                                                   UiConversion.GetValueString(myTess.MyCarData?.climate_state?.passenger_temp_setting, false) + " °C" ; } }

        public string FanACStatus { get { return "Fan:  " + UiConversion.GetValueString(myTess.MyCarData?.climate_state?.fan_status) +
                                                    "  -  A/C: " + UiConversion.GetValueString(myTess.MyCarData?.climate_state?.is_climate_on); } }

        public string SeatHeater { get { return "Seat heater (L/R): " + UiConversion.GetValueString(myTess.MyCarData?.climate_state?.seat_heater_left) + " / " + UiConversion.GetValueString(myTess.MyCarData?.climate_state?.seat_heater_right); } }

        public string IsAutoPreconditioning {
            get {
                string pre = "Conditioning: " + ( ( myTess.MyCarData?.climate_state?.is_auto_conditioning_on ?? false ) ? "on" : "off" );
                if ( myTess.MyCarData?.climate_state?.is_preconditioning ?? false ) pre += " [pre] ";
                if ( myTess.MyCarData?.climate_state?.smart_preconditioning ?? false ) pre += " (smart)";
                return pre;
            }
        }

        public string ClimateKeeperMode { get { return "On leave: " + myTess.MyCarData?.climate_state?.climate_keeper_mode; } }

        public ICommand StartConditioningCmd {
            get {
                return new RelayCommand(async p => {
                    int tempToSet = 20;
                    double? outside = this.myTess.MyCarData?.climate_state?.outside_temp;
                    if ( outside.HasValue && outside.Value < 10 ) tempToSet = 21;
                    //else if ( outside.HasValue && outside.Value > 25 ) tempToSet = 19; // to be considered...

                    TessApiResult res       = await myTess.SetTemps(tempToSet, tempToSet);
                    if ( !res.Success ) HandleError(res);
                    else {
                        res                 = await myTess.StartAutoconditioning();
                        HandleResultAndRefresh(res);
                    }
                },
                p => ( !myTess.IsSleeping && !( myTess.MyCarData?.climate_state?.is_preconditioning ?? true ) ));
            }
        }

        public Visibility DefrostOffVisible {
            get {
                if ( DefrostIsOn )  return Visibility.Visible;
                else                return Visibility.Hidden;
            }
        }

        private bool DefrostIsOn {
            get {
                bool on = false;
                if ( ( myTess.MyCarData?.climate_state != null ) &&
                     ( myTess.MyCarData?.climate_state?.driver_temp_setting == myTess.MyCarData?.climate_state?.max_avail_temp ) ) on = true;
                return on;
            }
        }

        public ICommand ToggleDefrostCmd {
            get {
                return new RelayCommand(async p => {
                    TessApiResult res       = await myTess.SetPreconditioningMax(!DefrostIsOn);
                    HandleResultAndRefresh(res);
                },
                p => ( !myTess.IsSleeping ));
            }
        }

        public ICommand StopConditioningCmd {
            get { return new RelayCommand(async p => { HandleResultAndRefresh(await myTess.StopAutoconditioning()); }, p => ( !myTess.IsSleeping && ( myTess.MyCarData?.climate_state?.is_preconditioning.Value ?? false ) )); }
        }

        public ICommand StartSeatHeatCmd {
            get {
                return new RelayCommand<string>(async p => {
                    TessApiResult res;
                    if ( p == "R" )     res = await myTess.SetSeatHeater(MyTess.SeatHeaterNumber.Passenger, MyTess.SeatHeaterLevel.High);
                    else                res = await myTess.SetSeatHeater(MyTess.SeatHeaterNumber.Driver, MyTess.SeatHeaterLevel.High);
                    HandleResultAndRefresh(res);
                }, p => ( !myTess.IsSleeping && ( myTess.MyCarData?.climate_state?.is_preconditioning ?? false ) ));
            }
        }

        public ICommand StopSeatHeatCmd {
            get {
                return new RelayCommand(async p => {
                    TessApiResult res = await myTess.SetSeatHeater(MyTess.SeatHeaterNumber.Driver, MyTess.SeatHeaterLevel.Off);
                    if ( !res.Success ) HandleError(res);
                    else HandleResultAndRefresh(await myTess.SetSeatHeater(MyTess.SeatHeaterNumber.Passenger, MyTess.SeatHeaterLevel.Off));
                }, p => ( !myTess.IsSleeping && ( myTess.MyCarData?.climate_state?.is_preconditioning ?? false ) ));
            }
        }

        public ICommand WindowVentCmd {
            get { return new RelayCommand(async p => { HandleResultAndRefresh(await myTess.VentWindows(settings.latWindows.Value, settings.lonWindows.Value)); }, p => ( !myTess.IsSleeping && settings.latWindows.HasValue && settings.lonWindows.HasValue )); }
        }

        public ICommand WindowCloseCmd {
            get { return new RelayCommand(async p => { HandleResultAndRefresh(await myTess.CloseWindows(settings.latWindows.Value, settings.lonWindows.Value)); }, p => ( !myTess.IsSleeping && settings.latWindows.HasValue && settings.lonWindows.HasValue )); }
        }

        #endregion Temp, A/C ...

        public ICommand HonkCmd {
            get { return new RelayCommand(async p => { if ( !AreUSure("Ganz sicher hupen?") ) return; HandleResultAndRefresh(await myTess.HonkHorn()); }, p => ( !myTess.IsSleeping )); }
        }

        public ICommand FlashCmd {
            get { return new RelayCommand(async p => { HandleResultAndRefresh(await myTess.FlashLights()); }, p => ( !myTess.IsSleeping )); }
        }

        #region Doors / Trunk ...

        public string LockedAndSentry {
            get {
                string locked = "";
                if ( myTess.MyCarData?.vehicle_state?.locked.HasValue ?? false ) locked += ( myTess.MyCarData.vehicle_state.locked.Value ? "Locked" : "Unlocked" );
                if ( ( myTess.MyCarData?.vehicle_state?.sentry_mode.HasValue ?? false ) && ( myTess.MyCarData.vehicle_state.sentry_mode.Value ) ) locked += " [Sentry ON]";
                return locked;
            }
        }

        public string Odo { get { return " \u27FC " + UiConversion.GetValueStringMiToKm(myTess.MyCarData?.vehicle_state?.odometer) + " km"; } }

        public ICommand UnlockDoorsCmd {
            get { return new RelayCommand(async p => { if ( !AreUSure("Wirklich entriegeln?") ) return; HandleResultAndRefresh(await myTess.DoorsUnlock()); }, p => ( !myTess.IsSleeping && ( myTess.MyCarData?.vehicle_state?.locked ?? false ) )); }
        }

        public ICommand LockDoorsCmd {
            get { return new RelayCommand(async p => { if ( !AreUSure("Wirklich verriegeln?") ) return; HandleResultAndRefresh(await myTess.DoorsLock()); }, p => ( !myTess.IsSleeping && !( myTess.MyCarData?.vehicle_state?.locked ?? false ))); }
        }

        public ICommand OpenFrontTrunkCmd {
            get { return new RelayCommand(async p => { if ( !AreUSure("Ganz sicher den Frunk aufmachen?") ) return; HandleResultAndRefresh(await myTess.OpenFrontTrunk()); }, p => ( !myTess.IsSleeping )); }
        }

        public ICommand OpenRearTrunkCmd {
            get { return new RelayCommand(async p => { if ( !AreUSure("Ganz sicher den Kofferraum aufmachen?") ) return; HandleResultAndRefresh(await myTess.OpenRearTrunk()); }, p => ( !myTess.IsSleeping )); }
        }

        public ICommand SetSentryOn {
            get { return new RelayCommand(async p => { if ( !AreUSure("Wächtermodus aktivieren?") ) return; HandleResultAndRefresh(await myTess.SetSentryMode(true)); }, p => ( !myTess.IsSleeping && !( myTess.MyCarData?.vehicle_state?.sentry_mode ?? false ) )); }
        }

        public ICommand SetSentryOff {
            get { return new RelayCommand(async p => { if ( !AreUSure("Wächtermodus de-aktivieren?") ) return; HandleResultAndRefresh(await myTess.SetSentryMode(false)); }, p => ( !myTess.IsSleeping && ( myTess.MyCarData?.vehicle_state?.sentry_mode ?? true ) )); }
        }

        #endregion Doors / Trunk ...

        #region Battery / Charging ...

        private int chargeLimit;

        public int ChargeLimit {
            get { return chargeLimit; }
            set {
                chargeLimit = value;
                NotifyPropertyChanged(nameof(ChargeLimit));
            }
        }

        public ICommand StartChargeCmd {
            get { return new RelayCommand(async p => { HandleResultAndRefresh(await myTess.StartCharge()); }, p => myTess.CanDoCharging); }
        }

        public ICommand StopChargeCmd {
            get { return new RelayCommand(async p => { HandleResultAndRefresh(await myTess.StopCharge()); }, p => ( myTess.IsCharging && !myTess.IsSleeping )); }
        }

        public ICommand SetChargeLimit {
            get { return new RelayCommand(async p => { HandleResultAndRefresh(await myTess.SetChargeLimit(chargeLimit)); }, p => !myTess.IsSleeping); }
        }

        public ICommand ClosePortCmd {
            get { return new RelayCommand(async p => { HandleResultAndRefresh(await myTess.CloseChargePort()); }, p => ( !myTess.IsCharging && !myTess.IsSleeping )); }
        }

        public ICommand OpenPortCmd {
            get { return new RelayCommand(async p => { HandleResultAndRefresh(await myTess.OpenChargePort()); }, p => ( !myTess.IsCharging && !myTess.IsSleeping )); }
        }

        public int BatteryLevel { get { return UiConversion.GetValueInt(myTess.MyCarData?.charge_state?.battery_level); } }

        public string UsableBatteryLevel {
            get {
                return ( myTess.MyCarData?.charge_state?.usable_battery_level != myTess.MyCarData?.charge_state?.battery_level ) ?
                    "\u2745" + UiConversion.GetValueString(myTess.MyCarData?.charge_state?.usable_battery_level) + "%" : "";
            }
        }

        public string BatteryRange { get { return "\u21E2" + UiConversion.GetValueStringMiToKm(myTess.MyCarData?.charge_state?.battery_range) + " km"; } }
        public string EstBatteryRange { get { return "\u219D" + UiConversion.GetValueStringMiToKm(myTess.MyCarData?.charge_state?.est_battery_range) + " km"; } }

        public string ChargingState { get { return "State: " + myTess.MyCarData?.charge_state?.charging_state; } }

        public string ChargeLimitSoc { get { return "SOC Limit : " + UiConversion.GetValueString(myTess.MyCarData?.charge_state?.charge_limit_soc) + " %" +
                                                    " in: " + UiConversion.GetValueString(myTess.MyCarData?.charge_state?.time_to_full_charge, false) + "h"; } }

        public string ChargerActualCurrent { get { return UiConversion.GetValueString(myTess.MyCarData?.charge_state?.charger_actual_current) + " A ~ " +
                                                                          UiConversion.GetValueStringMiToKm(myTess.MyCarData?.charge_state?.charge_rate) + " km/h"; } }

        public string ChargeCurrentRequest { get { return "Request: " + UiConversion.GetValueString(myTess.MyCarData?.charge_state?.charge_current_request) +
                                                           " / " +  UiConversion.GetValueString(myTess.MyCarData?.charge_state?.charge_current_request_max)  + " A"; } }

        public string ChargerVoltage {
            get {
                string val = UiConversion.GetValueString(myTess.MyCarData?.charge_state?.charger_voltage) + " V - " +
                                                                 UiConversion.GetValueString(myTess.MyCarData?.charge_state?.charger_power, false) + " kW";
                if ( myTess.MyCarData?.charge_state?.charger_phases == 1 ) val += " (1)";
                else val += " (2+)";
                return val;
            }
        }

        public string ChargePortDoorOpen { get {
                string ret = "Port: ";
                if ( ( myTess.MyCarData != null ) && ( myTess.MyCarData.charge_state != null ) && myTess.MyCarData.charge_state.charge_port_door_open.HasValue ) {
                    ret += ( myTess.MyCarData.charge_state.charge_port_door_open.Value ? "Open" : "Closed" );
                }
                else ret += "-";
                ret += " - Lock " + myTess.MyCarData?.charge_state?.charge_port_latch;

                return ret;
        } }

        public string BatteryHeaterOn { get { return "Battery Heater: " + UiConversion.GetValueString(myTess.MyCarData?.climate_state?.battery_heater); } }

        public string ScheduledChargingPending { get { return "Scheduled Charging: " + UiConversion.GetValueString(myTess.MyCarData?.charge_state?.scheduled_charging_pending); } }
        public string ScheduledChargingStartTime { get { return "SC Start: " + GetScheduledTime(myTess.MyCarData?.charge_state?.scheduled_charging_start_time); } }

        private string GetScheduledTime(long? scheduledChargingStartTime) {
            if ( !scheduledChargingStartTime.HasValue ) return "";
            DateTime ts = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(scheduledChargingStartTime.Value).AddMinutes(20 + ( 60 * 3 )); // Keine Ahnung, ist ein Hack...
            return ts.ToShortDateString() + " - " +ts.ToShortTimeString();
        }

        #endregion Battery / Charging ...

        private void HandleResultAndRefresh(TessApiResult res) {
            if ( !res.Success ) HandleError(res);
            else if ( !AutoRefreshTimerEnabled ) RefreshData(false);
        }

        private void HandleError(TessApiResult res) {
            TessControlManager.Instance.ShowError(res.Message);
        }

        public override void NotifyPropertyChanged() {
            NotifyPropertyChanged(null); // Alles
        }
    }
}