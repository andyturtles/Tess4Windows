using System;
using System.Windows;
using aBasics;
using aBasics.WpfBindingErrors;

namespace Tess4Windows {

    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application {

        internal static App myApp { get { return Application.Current as App; } }
        internal MainWindow myMainWindow { get { return (MainWindow)Application.Current.MainWindow; } }

        public App() {
            Log.SetLogFilename("Tess4Windows.txt");
            Log.SetLevel(Log.LEVEL.INFO);
            Log.Info("Tess4Windows - Start");

            this.DispatcherUnhandledException += HandleUnhandledExceptions;
            BindingExceptionThrower.Attach();

            TessControlManager tcm      = new TessControlManager();
            TessControlManager.Instance = tcm;
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

            if ( ShowErrorMsg("Error: " + eMsg + e?.Exception?.ToString()) ) ExitWithThread();
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

        private void ExitWithThread() {
            // The App does not terminate without running the exit in a separate thread.
            // A suggestion for the reason: The problem does only occur, if the UI thread runs exit. Shutdown the UI seems to be part of the exit routine.
            // So the UI thread could block itself by requesting itself to exit.
            new System.Threading.Thread(() => { Environment.Exit(-1); }).Start();
        }

        #endregion UnhandledExceptions...
    }
}