using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private int ServerPort
        {
            get
            {
                int toReturn = -1;
                return !int.TryParse(ServerPortText.Text, out toReturn) ? -1 : toReturn;
            }
        }

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
            RequestInfo.Text = "";
            bool flag = true;           
            if (string.IsNullOrWhiteSpace(ServerAdress.Text))
            {
                RequestInfo.Text += "A valid server address is needed!\n";
                flag = false;
            }
            if (ServerPort < 0 || ServerPort > (int)ushort.MaxValue)
            {
                RequestInfo.Text += "Please provide a valid portnumber!\n";
                flag = false;
            }
            if (string.IsNullOrWhiteSpace(ServerPassword.Password))
            {
                RequestInfo.Text += "No rcon password provided!\n";
                flag = false;
            }
            if (!flag)
                return;
            //Si connection sucess passer à la suite
            //Validation
            MainWindow window = (MainWindow)Window.GetWindow(this);
            window.AfficherServerInfo();
        }
    }
}
