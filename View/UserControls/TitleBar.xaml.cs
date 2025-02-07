using System.Windows;
using System.Windows.Controls;

namespace Calculator.View.UserControls
{
    public partial class TitleBar : UserControl
    {
        private Window mainWindow;
        public TitleBar()
        {
            InitializeComponent();
            mainWindow = Application.Current.MainWindow;
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mainWindow.DragMove();

            if (e.ClickCount > 1)
                btnMaximize_Click(sender, e);
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.WindowState = WindowState.Minimized;
        }
        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (mainWindow.WindowState == WindowState.Maximized)
            {
                mainWindow.WindowState = WindowState.Normal;
                btnMaximize.Content = "🗖";
            }

            else
            {
                mainWindow.WindowState = WindowState.Maximized;
                btnMaximize.Content = "🗗";
            }
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Close();
        }
    }
}
