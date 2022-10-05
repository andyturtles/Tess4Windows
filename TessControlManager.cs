using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using aBasics;
using aBasics.WpfBasics;
using Tess4Windows.UserControls;
using TessApi;

namespace Tess4Windows {

    /// <summary>
    /// Logik für die Anzeige des passenden Controls und Verwaltung der Tess-Klassen
    /// Vor allem interessant für die Verwendung des TessConmtrols in anderen Projekten!
    /// </summary>
    public class TessControlManager {

        public delegate void ShowErrorMessage(string msg);

        public static TessControlManager Instance { get; set; }

        public MyTess           TessApi        { get; }
        public ShowErrorMessage ShowError      { get; private set; }
        public ImageSource      TessBackground { get; private set; }

        #region Fields

        public UserControl      UserControl;
        public Tess4WinSettings Settings;

        #endregion

        public TessControlManager() {
            TessApi  = new MyTess();
            Settings = Tess4WinSettings.LoadSettings();
        }

        public void Init(UserControl ctlForDisplay, ShowErrorMessage showErrorMsg, ResourceDictionary resources) {
            UserControl = ctlForDisplay;
            ShowError   = showErrorMsg;

            try {
                LoadStyleDictionaryFromFile(resources);
                TessBackground = Images.ImageSourceFromFile("Images\\tess_win_bg.jpg");
            }
            catch ( Exception ex ) {
                Log.Error("TessControlManager.Init - Load tessBackground", ex);
            }
        }

        private void LoadStyleDictionaryFromFile(ResourceDictionary resources) {
            string styleFile = "Style\\TessColors.xaml";
            try {
                using ( FileStream fs = new FileStream(styleFile, FileMode.Open, FileAccess.Read, FileShare.Read) ) {
                    ResourceDictionary dic = (ResourceDictionary) XamlReader.Load(fs);

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
        public void ShowSuitableControl() {
            if ( !TessApi.IsLoggedIn ) ShowLogin();
            else if ( Settings?.CarId == null ) ShowSettings();
            else {
                Task.Run(async () => {
                             await RefreshToken();
                         }).Wait();

                TessApi.SetCarId(Settings.CarId.Value);
                TessControl tc = new TessControl();
                ShowControl(tc);
            }
        }

        public async Task RefreshToken() {
            await TessApi.RefreshToken();

            // TODO: if further use
            //TessApiLoginResult res = await TessApi.RefreshToken();
            //if(!res.Success)
        }

        public void ShowLogin() {
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
            new Thread(() => { Environment.Exit(-1); }).Start();
        }

    }

}
