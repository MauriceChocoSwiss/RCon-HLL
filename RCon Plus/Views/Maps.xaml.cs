using System.Windows;
using System.Windows.Controls;
using RconClient;

namespace RCon_Plus
{
    /// <summary>
    /// Logique d'interaction pour Maps.xaml
    /// </summary>
    public partial class Maps : UserControl
    {
        private readonly ServerSession _serverSession;
        private string _mapName;
        public Maps(ServerSession serverSession)
        {
            InitializeComponent();
            _serverSession = serverSession;
        }

        private void ShowMessageBox(object sender, RoutedEventArgs e)
        {
            MapMessageBox mapMessageBox = new MapMessageBox(_serverSession);
            _mapName = (sender as Button).Name.ToString();
            mapMessageBox.Show();
            mapMessageBox.SetMap(_mapName);
        }
    }
}
