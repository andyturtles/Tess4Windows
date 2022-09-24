using System;
using System.IO;
using System.Reflection;
using System.Windows;
using aBasics;
using aBasics.WpfBindingErrors;

namespace Tess4Windows {

    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application {

        public App() {
            string logsFolder   = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            Uri uri             = new Uri(logsFolder);
            logsFolder          = uri.LocalPath;
            logsFolder          = Path.Combine(logsFolder, "logs");

            Log.SetLogFileDirectory(logsFolder);
            Log.SetLogFilename("Tess4Windows.txt");
            Log.SetLevel(Log.LEVEL.INFO);
            Log.Info("Tess4Windows - Start");

            DeleteOldLogs(logsFolder);

            this.DispatcherUnhandledException += HandleUnhandledExceptions;
            BindingExceptionThrower.Attach();

            TessControlManager tcm      = new TessControlManager();
            TessControlManager.Instance = tcm;
        }

        private void DeleteOldLogs(string logsFolder) {
            try {
                DirectoryInfo di = new DirectoryInfo(logsFolder);
                foreach ( FileInfo fi in di.GetFiles() ) {
                    if ( ( DateTime.Now - fi.LastWriteTime ).TotalDays > 14 ) fi.Delete();
                }
            }
            catch ( Exception ex) {
                Log.Error("DeleteOldLogs", ex);
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e) {
            bool pwMode = false;
            if ( ( e.Args.Length > 0 ) && ( e.Args[0] == "/paranoid" ) ) pwMode = true;

            MainWindow wnd = new MainWindow(pwMode, this.ShowError, this.Resources);
            wnd.Show();
        }

        #region UnhandledExceptions...

        /// <summary>
        /// Handles the unhandled exceptions.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DispatcherUnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void HandleUnhandledExceptions(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            Log.Error("UNHANDLED EXCEPTION", e?.Exception);
            string eMsg = "";
            if ( e != null ) eMsg = e.Exception.Message;

            if ( ShowErrorMsg("Error: " + eMsg + e?.Exception?.ToString()) ) TessControlManager.Instance.ExitWithThread();
        }

        private static bool errorOpen = false;

        internal void ShowError(string msg) {
            ShowErrorMsg(msg);
        }

        private bool ShowErrorMsg(string msg) {
#warning Log?
            if ( errorOpen ) return false;

            if ( !Application.Current.Dispatcher.CheckAccess() ) {
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    ShowErrorMsg(msg);
                }));
                return false;
            }
            errorOpen = true;
            MessageBox.Show(msg, "Error!");
            errorOpen = false;
            return true;
        }

        #endregion UnhandledExceptions...
    }
}