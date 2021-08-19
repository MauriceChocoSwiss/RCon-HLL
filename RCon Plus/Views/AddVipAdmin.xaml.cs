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
using System.Windows.Shapes;
using RconClient;

namespace RCon_Plus.Views
{
    /// <summary>
    /// Logique d'interaction pour AddVipAdmin.xaml
    /// </summary>
    public partial class AddVipAdmin : Window
    {
        private readonly ServerSession _serverSession;
        private readonly bool _vip;
        private string _selectedGroup;

        public AddVipAdmin(ServerSession serverSession, bool vip = false)
        {
            InitializeComponent();
            _serverSession = serverSession;
            _vip = vip;
            DataContext = _serverSession;
            textBlock.Text = "Ajouter un admin";
            if (vip)
            {
                textBlock.Text = "Ajouter un VIP";
                ListBoxEntry.IsEnabled = false;
            }
        }

        private void ClocseBox(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            string command = "";
            string result = "";
            string[] parameter = new string[0];

            if (_vip)
            {
                if (!string.IsNullOrEmpty(SteamId.Text) && !string.IsNullOrEmpty(DescriptionBox.Text))
                {
                    command = "Add VIP Player";
                    parameter = new string[2] { SteamId.Text, DescriptionBox.Text };
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(SteamId.Text) && !string.IsNullOrEmpty(_selectedGroup))
                {
                    command = "Add Admin";
                    parameter = new string[3] { SteamId.Text, _selectedGroup, DescriptionBox.Text };
                }
            }

            if (parameter.Count() > 0)
                _serverSession.SendCommand(command, parameter, out result);

            if (result == "SUCCESS")
            {
                _serverSession.UpdateServerSettings();
                Close();
            }
        }

        private void ListBoxEntry_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxEntry.SelectedItem != null)
                _selectedGroup = ListBoxEntry.SelectedItem.ToString();
        }
    }
}
