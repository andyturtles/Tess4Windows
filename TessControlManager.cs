﻿using System.Windows.Controls;
using Tess4Windows.UserControls;
using TessApi;

namespace Tess4Windows {

    /// <summary>
    /// Logik für die Anzeige des passenden Controls und Verwaltung der Tess-Klassen
    /// Vor allem interessant für die Verwendung des TessConmtrols in anderen Projekten!
    /// </summary>
    public class TessControlManager {

        public static TessControlManager Instance { get; set; }

        public delegate void ShowErrorMessage(string msg);

        public MyTess               TessApi { get; private set; }
        public UserControl          UserControl;
        public Tess4WinSettings     Settings;
        public ShowErrorMessage     ShowError { get; private set; }

        public TessControlManager() {
            TessApi         = new MyTess();
            Settings        = Tess4WinSettings.LoadSettings();
        }

        public void Init(UserControl ctlForDisplay, ShowErrorMessage showErrorMsg) {
            UserControl     = ctlForDisplay;
            ShowError       = showErrorMsg;
        }

        /// <summary>
        /// Zeigt das jeweils 'passende' Control an
        /// </summary>
        /// <param name="tess">The MyTess.</param>
        /// <param name="toSet">To UserControl.</param>
        /// <param name="set">The set.</param>
        public void ShowSuitableControl() {
            if ( !TessApi.IsLoggedIn )          ShowLogin();
            else if ( Settings?.CarId == null ) ShowSettings();
            else {
                TessApi.SetCarId(Settings.CarId.Value);
                TessControl tc = new TessControl();
                ShowControl(tc);
            }
        }

        public void ShowLogin() {
#warning TODO: bestehendes Login löschen !?
            TessLoginControl tlc = new TessLoginControl();
            ShowControl(tlc);
        }

        public void ShowSettings() {
            TessSettingsControl tsc = new TessSettingsControl();
            ShowControl(tsc);
        }

        public void ShowControl(UserControl ctl) {
            UserControl.Content = ctl;
        }
    }
}