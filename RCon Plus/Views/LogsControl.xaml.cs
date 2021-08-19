using System.Timers;
using System.Windows.Controls;
using RconClient;

namespace RCon_Plus
{
    /// <summary>
    /// Logique d'interaction pour LogsControl.xaml
    /// </summary>
    public partial class LogsControl : UserControl
    {
        private readonly ServerSession _serverSession;

        public LogsControl(ServerSession serverSession)
        {
            InitializeComponent();
            _serverSession = serverSession;
            DataContext = _serverSession;
            _serverSession.UpdateLog();
            GetData();
        }

        public void GetData()
        {
            Timer timer = new(5000) { AutoReset = true, Enabled = true };

            timer.Elapsed += new ElapsedEventHandler(TimerElapsed);
        }

        public void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            _serverSession.UpdateLog();
        }
    }
}
