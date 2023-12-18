using System.Windows.Controls;

namespace Module.Home.Views
{
    /// <summary>
    /// Home.xaml 的交互逻辑
    /// </summary>
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
        }

        private void txtRecord_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtRecord.ScrollToEnd();
        }
    }
}
