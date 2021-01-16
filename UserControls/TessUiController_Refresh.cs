using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;

namespace Tess4Windows.UserControls {

    /// <summary>
    /// Partial für Refresh Timer etc.
    /// </summary>
    internal partial class TessUiController {

        private Timer autoRefreshTimer;
        private Stopwatch refreshWatch;

        private void InitRefresh() {
            autoRefreshTimer            = new Timer();
            autoRefreshTimer.Elapsed    += autoRefreshTimer_Elapsed;
            autoRefreshTimer.Interval   = 50;
            autoRefreshTimer.Start();

            refreshWatch                = new Stopwatch();
        }

        private void autoRefreshTimer_Elapsed(object sender, ElapsedEventArgs e) {
            if ( !AutoRefreshTimerEnabled ) {
                refreshWatch.Reset();
                return;
            }

            if ( !refreshWatch.IsRunning ) refreshWatch.Start();

            AutoRefreshRotate += 20;
            if ( AutoRefreshRotate > 340 ) AutoRefreshRotate = 0;
            NotifyPropertyChanged(nameof(AutoRefreshRotate));

            if ( refreshWatch.Elapsed.TotalSeconds >= 5 ) {
#warning abschalten wenn schlafen möglich
                RefreshData(false);
                refreshWatch.Restart();
            }
        }

        public int AutoRefreshRotate { get; private set; }
        public bool AutoRefreshTimerEnabled { get; set; }
        public int WaitRotate { get; private set; }

        private async void AnimateRefresh() {
            int cnt = 0;
            while ( cnt++ < 50 ) {
                WaitRotate  = cnt * 10;

                NotifyPropertyChanged(nameof(WaitRotate));
                await Task.Delay(5);
            }
        }
    }
}