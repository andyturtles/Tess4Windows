using System.Collections.ObjectModel;
using aBasics.WpfBasics;
using TessApi.JsonData;

namespace Tess4Windows.UserControls {

    /// <summary>
    /// UI Controller für das Choose Window
    /// </summary>
    /// <seealso cref="aBasics.WpfBasics.UIController" />
    internal class ChooseUiController : UIController {

        public ObservableCollection<Product> ObservProductList {
            get {
                if ( TessControlManager.Instance.TessApi.ProductList == null ) return null;
                else return new ObservableCollection<Product>(TessControlManager.Instance.TessApi.ProductList);
            }
        }

        public ChooseUiController() {
        }

        public override void NotifyPropertyChanged() {
            NotifyPropertyChanged(nameof(ObservProductList));
        }
    }
}