using System.Timers;
using System.Windows.Controls;
using RconClient;

namespace RCon_Plus
{
    /// <summary>
    /// Logique d'interaction pour Settings.xaml
    /// </summary>
    public partial class ServerSettings : UserControl
    {
        private readonly ServerSession _serverSession;

        public ServerSettings(ServerSession serverSession)
        {
            InitializeComponent();
            MapScroll.Content = new Maps(serverSession);
            _serverSession = serverSession;
            DataContext = serverSession;
            _serverSession.UpdateServerSettings();
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
        }

        private void AutoBalanceChange(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            AutoBalance_Value.Text = Slider_AutoBalance.Value.ToString();
        }

        private void TeamSwitchChange(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            Team_Sw_Value.Text = Team_Switch_Delay_Slider.Value.ToString();
        }

        private void KickCoolDownChange(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            Kick_Idle_Time_Value.Text = Kick_Idle_Time_Slider.Value.ToString();
        }

        private void QueuChange(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            Max_Queued_Value.Text = Max_Queued_Slider.Value.ToString();
        }

        private void VipChange(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            Max_VIP_Value.Text = Max_Vip_Slider.Value.ToString();
        }
    }
}
