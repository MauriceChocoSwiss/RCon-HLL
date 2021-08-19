using System;
using System.Timers;
using System.Windows.Controls;
using RCon_Plus.Views;
using RconClient;

namespace RCon_Plus
{
    /// <summary>
    /// Logique d'interaction pour Settings.xaml
    /// </summary>
    public partial class ServerSettings : UserControl
    {
        internal static ServerSettings _serverSettings;
        private readonly ServerSession _serverSession;
        private string _selectedMap;
        private string _selectedAdmin;
        private string _selectedVIP;
        private string _maxPing;
        private string _autoBalance;
        private string _autoBalanceOnOff;
        private string _teamSwitch;
        private string _kickCoolDown;
        private string _queuChange;
        private string _vipChange;
        private string _badWord;

        public ServerSettings(ServerSession serverSession)
        {
            InitializeComponent();
            MapScroll.Content = new Maps(serverSession);
            DataContext = serverSession;
            _serverSession = serverSession;
            _serverSettings = this;
            _serverSession.UpdateServerSettings();
        }

        public void UpdateCheckBox(string value)
        {
            Dispatcher.Invoke(() => { AutoBalanceCheckBox.IsChecked = Convert.ToBoolean(value); });
        }

        public void GetterTimer()
        {
            Timer timer = new(60000) { AutoReset = true, Enabled = true };

            timer.Elapsed += new ElapsedEventHandler(GetTimerElapased);
        }

        private void GetTimerElapased(object source, ElapsedEventArgs e)
        {
            _serverSession.UpdateServerSettings();
        }

        private void MaxPingChange(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            High_Ping_Value.Text = Slider_HighPing.Value.ToString();
            _maxPing = Slider_HighPing.Value.ToString();
        }

        private void AutoBalanceChange(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            AutoBalance_Value.Text = Slider_AutoBalance.Value.ToString();
            _autoBalance = Slider_AutoBalance.Value.ToString();
        }

        private void TeamSwitchChange(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            Team_Sw_Value.Text = Team_Switch_Delay_Slider.Value.ToString();
            _teamSwitch = Team_Switch_Delay_Slider.Value.ToString();
        }

        private void KickCoolDownChange(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            Kick_Idle_Time_Value.Text = Kick_Idle_Time_Slider.Value.ToString();
            _kickCoolDown = Kick_Idle_Time_Slider.Value.ToString();
        }

        private void QueuChange(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            Max_Queued_Value.Text = Max_Queued_Slider.Value.ToString();
            _queuChange = Max_Queued_Slider.Value.ToString();
        }

        private void VipChange(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            Max_VIP_Value.Text = Max_Vip_Slider.Value.ToString();
            _vipChange = Max_Vip_Slider.Value.ToString();
        }

        private void MapRotationListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MapRotationListView.SelectedItem != null)
                _selectedMap = MapRotationListView.SelectedItem.ToString();
        }

        private void RemoveMapClick(object sender, System.Windows.RoutedEventArgs e)
        {
            string result = "";
            _serverSession.SendCommand("Remove Map From Rotation", new string[1] { _selectedMap }, out result);
            if (result == "SUCCESS")
            {
                MapRotationListView.UnselectAll();
                _selectedMap = "";
                _serverSession.UpdateServerSettings();
            }
        }

        private void Admin_Player_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Admin_Player_List.SelectedItem != null)
                _selectedAdmin = Admin_Player_List.SelectedItem.ToString();
        }

        private void RemoveAdminClick(object sender, System.Windows.RoutedEventArgs e)
        {
            string steamId = _selectedAdmin.Split(" ")[0];
            string result = "";
            _serverSession.SendCommand("Delete Admin", new string[1] { steamId }, out result);
            if (result == "SUCCESS")
            {
                MapRotationListView.UnselectAll();
                _selectedAdmin = "";
                _serverSession.UpdateServerSettings();
            }
        }

        private void AddAdminClick(object sender, System.Windows.RoutedEventArgs e)
        {
            AddVipAdmin add = new AddVipAdmin(_serverSession);
            add.Show();
        }

        private void AddViPClick(object sender, System.Windows.RoutedEventArgs e)
        {
            AddVipAdmin add = new AddVipAdmin(_serverSession, true);
            add.Show();
        }

        private void RemoveVIPClick(object sender, System.Windows.RoutedEventArgs e)
        {
            string steamId = _selectedVIP.Split(" ")[0];
            string result = "";

            if (!string.IsNullOrEmpty(steamId))
                _serverSession.SendCommand("Delete VIP Player", new string[1] { steamId }, out result);
            if (result == "SUCCESS")
            {
                MapRotationListView.UnselectAll();
                _selectedAdmin = "";
                _serverSession.UpdateServerSettings();
            }
        }

        private void VIP_Player_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VIP_Player_List.SelectedItem != null)
                _selectedVIP = VIP_Player_List.SelectedItem.ToString();
        }

        private void Set_High_Ping_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string result = "";

            if (!string.IsNullOrEmpty(_maxPing))
                _serverSession.SendCommand("Set High Ping Threshold", new string[1] { _maxPing }, out result);
            if (result == "SUCCESS")
            {
                _serverSession.UpdateServerSettings();
            }
        }

        private void Set_Auto_Balance_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string result = "";

            if (!string.IsNullOrEmpty(_autoBalance))
                _serverSession.SendCommand("Set Auto Balance Threshold", new string[1] { _autoBalance }, out result);
            if (result == "SUCCESS")
            {
                _serverSession.UpdateServerSettings();
            }
        }

        private void CheckBox_Click()
        {
           string result = "";

            _serverSession.SendCommand("Set Auto Balance", new string[1] { _autoBalanceOnOff }, out result);
            if (result == "SUCCESS")
            {
                _serverSession.UpdateServerSettings();
            }
        }

        private void AutoBalanceCheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            _autoBalanceOnOff = "on";
            CheckBox_Click();
        }

        private void AutoBalanceCheckBox_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            _autoBalanceOnOff = "off";
            CheckBox_Click();
        }

        private void Set_Team_Sw_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string result = "";

            if (!string.IsNullOrEmpty(_teamSwitch))
                _serverSession.SendCommand("Set Team Switch Cooldown", new string[1] { _teamSwitch }, out result);
            if (result == "SUCCESS")
            {
                _serverSession.UpdateServerSettings();
            }
        }

        private void Set_Kick_Idle_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string result = "";

            if (!string.IsNullOrEmpty(_kickCoolDown))
                _serverSession.SendCommand("Set Kick Idle Time", new string[1] { _kickCoolDown }, out result);
            if (result == "SUCCESS")
            {
                _serverSession.UpdateServerSettings();
            }
        }

        private void Set_Max_Queued_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string result = "";

            if (!string.IsNullOrEmpty(_queuChange))
                _serverSession.SendCommand("Set Max Queued Players", new string[1] { _queuChange }, out result);
            if (result == "SUCCESS")
            {
                _serverSession.UpdateServerSettings();
            }
        }

        private void Set_Max_VIP_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string result = "";

            if (!string.IsNullOrEmpty(_vipChange))
                _serverSession.SendCommand("Set Num VIP Slots", new string[1] { _vipChange }, out result);
            if (result == "SUCCESS")
            {
                _serverSession.UpdateServerSettings();
            }
        }

        private void Set_Message_Server_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string result = "";

            if (!string.IsNullOrEmpty(MessageServer_TextBox.Text))
                _serverSession.SendCommand("Set Server Message", new string[1] { MessageServer_TextBox.Text }, out result);
            if (result == "SUCCESS")
            {
                _serverSession.UpdateServerSettings();
            }
        }

        private void Set_BroadCast_Message_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string result = "";

            if (!string.IsNullOrEmpty(BroadCast_Message_TextBox.Text))
                _serverSession.SendCommand("Broadcast", new string[1] { BroadCast_Message_TextBox.Text }, out result);
            if (result == "SUCCESS")
            {
                _serverSession.UpdateServerSettings();
            }
        }

        private void RemoveProfanityWordButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string result = "";

            if (!string.IsNullOrEmpty(_badWord))
                _serverSession.SendCommand("Unban Profanity Word", new string[1] { _badWord }, out result);
            if (result == "SUCCESS")
            {
                ProfanityWordList.UnselectAll();
                _badWord = "";
                _serverSession.UpdateServerSettings();
            }
        }

        private void AddProfanityWord_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string result = "";

            if (!string.IsNullOrEmpty(BadWordBox.Text))
                _serverSession.SendCommand("Ban Profanity Words", new string[1] { BadWordBox.Text }, out result);
            if (result == "SUCCESS")
            {
                ProfanityWordList.UnselectAll();
                _badWord = "";
                _serverSession.UpdateServerSettings();
            }
        }

        private void ProfanityWordList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProfanityWordList.SelectedItem != null)
                _badWord = ProfanityWordList.SelectedItem.ToString();
        }
    }
}
