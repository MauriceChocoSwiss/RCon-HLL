using System.Windows;
using System.Windows.Controls;
using RCon_Plus.Properties;

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
                int toReturn;
                return !int.TryParse(ServerPortText.Text, out toReturn) ? -1 : toReturn;
            }
        }

        public ConnectionControl()
        {
            InitializeComponent();
            ServerAdress.Text = Settings.Default.ServerAdress;
            ServerPortText.Text = Settings.Default.ServerPort.ToString();
            ServerPassword.Password = Settings.Default.ServerPass;
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

            Settings.Default.ServerAdress = ServerAdress.Text;
            Settings.Default.ServerPort = ServerPort;
            Settings.Default.ServerPass = ServerPassword.Password;
            Settings.Default.Save();

            MainWindow._main.MainConnection(ServerAdress.Text.Trim(), ServerPort, ServerPassword.Password);
        }
    }
}
