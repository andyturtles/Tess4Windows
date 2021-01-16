using System.Windows.Controls;

namespace Tess4Windows.UserControls {

    /// <summary>
    /// Interaktionslogik für TessControl.xaml
    /// </summary>
    public partial class TessControl : UserControl {

        /// <summary>
        /// Initializes a new instance of the <see cref="TessControl" /> class.
        /// </summary>
        /// <param name="tess">The MyTess.</param>
        /// <param name="settings">The settings.</param>
        internal TessControl() {
            InitializeComponent();

            TessUiController cUiCtl = new TessUiController();
            this.DataContext        = cUiCtl;
        }
    }
}