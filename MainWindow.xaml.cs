using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Otaku_Database
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetWindowPosition();
        }
        void SetWindowPosition()
        {
            var settings = Properties.Settings.Default;

            if (settings.WindowLeft >= 0 &&
                settings.WindowLeft < SystemParameters.VirtualScreenWidth)
            { Left = settings.WindowLeft; }

            if (settings.WindowTop >= 0 &&
                settings.WindowTop < SystemParameters.VirtualScreenHeight)
            { Top = settings.WindowTop; }

            if (settings.WindowWidth > 0 &&
                settings.WindowWidth <= SystemParameters.WorkArea.Width)
            { Width = settings.WindowWidth; }

            if (settings.WindowHeight > 0 &&
                settings.WindowHeight <= SystemParameters.WorkArea.Height)
            { Height = settings.WindowHeight; }

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var settings = Properties.Settings.Default;
            WindowState = WindowState.Normal;
            settings.WindowLeft = Left;
            settings.WindowTop = Top;
            settings.WindowWidth = Width;
            settings.WindowHeight = Height;
            settings.Save();
        }
    }
}
