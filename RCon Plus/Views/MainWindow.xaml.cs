using System.Reflection;
using System.Windows;
using RconClient;

namespace RCon_Plus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ConnectionControl _connectionControl = new();

        internal static MainWindow _main;

        private ServerSession _serverSession;

        private readonly string _version = "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public MainWindow()
        {
            InitializeComponent();
            ServerPanel.Children.Add(_connectionControl);
            DisclaimerConnectionStatus.Text = _version;
            RconStaticLibrary.UpdateAvailableCommandsAndGetters();
            _main = this;
        }

        public void MainConnection(string host, int port, string password)
        {
            _serverSession = new ServerSession(host, port, password);
        }

        public void ChangeConnectionStatus(string value)
        {
            Dispatcher.Invoke(() => { DisclaimerConnectionStatus.Text = _version + " Status: " + value; });
        }

        public void AfficherServerInfo()
        {
            Dispatcher.Invoke(() =>
            {
                MainTabControl.IsEnabled = true;
                ServerPanel.Children.Remove(_connectionControl);
                ServerPanel.Children.Add(new ServerInfo(_serverSession));
                PlayerControlTabItem.Content = new PlayersControl(_serverSession);
                SettingsTabItem.Content = new ServerSettings(_serverSession);
                LogTabItem.Content = new LogsControl();
            });
        }
    }
}
