using MahApps.Metro.Controls;
using System.Windows;

namespace YuanQiUI.Views
{
    public partial class MainWindow 
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void HamburgerMenuControl_HamburgerButtonClick(object sender, RoutedEventArgs e)
        {
            HamburgerMenuControl.Width = ((HamburgerMenu)sender).IsPaneOpen?50: 245;
        }
    }
}
