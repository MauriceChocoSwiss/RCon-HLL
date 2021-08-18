using System.Timers;
using System.Windows.Controls;
using RconClient;

namespace RCon_Plus
{
    /// <summary>
    /// Logique d'interaction pour ServerInfo.xaml
    /// </summary>
    public partial class ServerInfo : UserControl
    {
        private readonly ServerSession _serverSession;

        internal ServerInfo(ServerSession serverSession)
        {
            InitializeComponent();

            _serverSession = serverSession;
            GetterTimer();
            DataContext = serverSession;
        }

        private void GetterTimer()
        {
            System.Timers.Timer timer = new System.Timers.Timer(10000) { AutoReset = true, Enabled = true };

            timer.Elapsed += new ElapsedEventHandler(GetterTimerElapsed);
        }

        private void GetterTimerElapsed(object source, ElapsedEventArgs e) => _serverSession.UpdateServerInfo();
    }
}

