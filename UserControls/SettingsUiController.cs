using System;
using System.Collections.ObjectModel;
using aBasics.WpfBasics;
using TessApi.JsonData;

namespace Tess4Windows.UserControls {

    /// <summary>
    /// UI Controller für das Settings Window
    /// </summary>
    /// <seealso cref="aBasics.WpfBasics.UIController" />
    internal class SettingsUiController : UIController {

        private Tess4WinSettings settings;

        public ObservableCollection<Product> ObservProductList {
            get {
                if ( TessControlManager.Instance.TessApi.ProductList == null ) return null;
                else return new ObservableCollection<Product>(TessControlManager.Instance.TessApi.ProductList);
            }
        }

        public string LatHomelink {
            get { return settings.latHomelink.ToString(); }
            set {
                if ( String.IsNullOrEmpty(value) )  settings.latHomelink = null;
                else                                settings.latHomelink = Double.Parse(value);
            }
        }

        public string LonHomelink {
            get { return settings.lonHomelink.ToString(); }
            set {
                if ( String.IsNullOrEmpty(value) )  settings.lonHomelink = null;
                else                                settings.lonHomelink = Double.Parse(value);
            }
        }

        public string LatWindows {
            get { return settings.latWindows.ToString(); }
            set {
                if ( String.IsNullOrEmpty(value) )  settings.latWindows = null;
                else                                settings.latWindows = Double.Parse(value);
            }
        }

        public string LonWindows {
            get { return settings.lonWindows.ToString(); }
            set {
                if ( String.IsNullOrEmpty(value) )  settings.lonWindows = null;
                else                                settings.lonWindows = Double.Parse(value);
            }
        }

        public SettingsUiController(Tess4WinSettings set) {
            settings = set;
        }

        public override void NotifyPropertyChanged() {
            NotifyPropertyChanged(null);
        }

        internal void SaveSettings(long? carId) {
            if ( carId.HasValue ) settings.CarId = carId.Value;
            settings.SaveSettings();
        }
    }
}