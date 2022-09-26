using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using aBasics;
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
        public ImageSource          TessBackground { get; private set; }

        public TessControlManager() {
            TessApi         = new MyTess();
            Settings        = Tess4WinSettings.LoadSettings();
        }

        public void Init(UserControl ctlForDisplay, ShowErrorMessage showErrorMsg, ResourceDictionary resources) {
            UserControl     = ctlForDisplay;
            ShowError       = showErrorMsg;

            try {
                LoadStyleDictionaryFromFile(resources);
                TessBackground = aBasics.WpfBasics.Images.ImageSourceFromFile("Images\\tess_win_bg.jpg");
            }
            catch ( Exception ex ) {
                Log.Error("TessControlManager.Init - Load tessBackground", ex);
            }
        }

        private void LoadStyleDictionaryFromFile(ResourceDictionary resources) {
            string styleFile = "Style\\TessColors.xaml";
                try {
                    using ( var fs = new FileStream(styleFile, FileMode.Open, FileAccess.Read, FileShare.Read) ) {
                        var dic = (ResourceDictionary)XamlReader.Load(fs);

                        ResourceDictionary rd = resources.MergedDictionaries.Where(x => x.Source.OriginalString == "style\\TessColors.xaml").SingleOrDefault();
                        if ( rd == null ) return;

                        resources.MergedDictionaries.Remove(rd);
                        resources.MergedDictionaries.Add(dic);
                    }
                }
                catch ( Exception ex ) {
                    Log.Warning("TessControlManager.LoadStyleDictionaryFromFile", ex);
                }
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

        public void ShowToken() {
            TessShowTokenControl tsc = new TessShowTokenControl();
            ShowControl(tsc);
        }

        public void ShowControl(UserControl ctl) {
            UserControl.Content = ctl;
        }

        internal void ExitWithThread() {
            // The App does not terminate without running the exit in a separate thread.
            // A suggestion for the reason: The problem does only occur, if the UI thread runs exit. Shutdown the UI seems to be part of the exit routine.
            // So the UI thread could block itself by requesting itself to exit.
            new System.Threading.Thread(() => { Environment.Exit(-1); }).Start();
        }
    }
}