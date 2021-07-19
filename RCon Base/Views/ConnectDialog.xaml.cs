using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using RconClient.Properties;

namespace RconClient
{
    public partial class ConnectDialog : Window, IComponentConnector
    {
        public ServerConnectionDetails ConnectionDetails { get; private set; }

        private string ServerAddress => this.txtAddress.Text;

        private int ServerPort
        {
            get
            {
                int result = -1;
                return !int.TryParse(this.txtPort.Text, out result) ? -1 : result;
            }
        }

        private string ServerPassword => this.txtPassword.Password;

        public ConnectDialog(ServerConnectionDetails connectionDetails = default(ServerConnectionDetails))
        {
            this.InitializeComponent();
            if (connectionDetails.Equals((object)new ServerConnectionDetails()))
            {
                this.txtAddress.Text = Settings.Default.LastUsedServerAddress;
                this.txtPort.Text = Settings.Default.LastUsedServerPort.ToString();
            }
            else
            {
                this.txtAddress.Text = connectionDetails.ServerAddress;
                this.txtPort.Text = connectionDetails.ServerPort.ToString();
                this.txtPassword.Password = connectionDetails.ServerPassword;
            }
            this.txtblockError.Text = this.VersionString();
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.txtblockError.Text = "";
            bool flag = true;
            if (string.IsNullOrWhiteSpace(this.ServerAddress))
            {
                this.txtblockError.Text += "A server address is needed!\n";
                flag = false;
            }
            if (this.ServerPort < 0 || this.ServerPort > (int)ushort.MaxValue)
            {
                this.txtblockError.Text += "Please provide a valid portnumber!\n";
                flag = false;
            }
            if (string.IsNullOrWhiteSpace(this.ServerPassword))
            {
                this.txtblockError.Text += "No rcon password provided!\n";
                flag = false;
            }
            if (!flag)
                return;
            this.ConnectionDetails = new ServerConnectionDetails(this.ServerAddress.Trim(), this.ServerPort, this.ServerPassword);
            Settings.Default.LastUsedServerAddress = this.ServerAddress;
            Settings.Default.LastUsedServerPort = this.ServerPort;
            Settings.Default.Save();
            this.DialogResult = new bool?(true);
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtAddress.Text))
            {
                this.txtAddress.Focus();
            }
            else
            {
                this.txtPassword.Focus();
                this.txtPassword.SelectAll();
            }
        }

        protected string VersionString()
        {
            string[] strArray = Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.');
            string str = "$Change: 642594 $";
            return "v" + string.Join(".", new string[3]
            {
        strArray[0],
        strArray[1],
        str.Substring(9).TrimEnd('$')
            });
        }
    }
}
