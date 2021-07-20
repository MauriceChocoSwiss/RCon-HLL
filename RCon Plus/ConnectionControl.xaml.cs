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

namespace RCon_Plus
{
    /// <summary>
    /// Logique d'interaction pour ConnectionControl.xaml
    /// </summary>
    public partial class ConnectionControl : UserControl
    {
        public ConnectionControl()
        {
            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            //Si connection sucess passer à la suite
            MainWindow window = (MainWindow)Window.GetWindow(this);
            window.AfficherServerInfo();
        }
    }
}
