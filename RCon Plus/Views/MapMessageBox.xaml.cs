using System.Windows;
using RconClient;

namespace RCon_Plus
{
    /// <summary>
    /// Logique d'interaction pour MapMessageBox.xaml
    /// </summary>
    public partial class MapMessageBox : Window
    {
        private readonly ServerSession _serverSession;
        private string _mapName;

        public MapMessageBox(ServerSession serverSession)
        {
            InitializeComponent();
            _serverSession = serverSession;
        }

        public void SetMap(string mapName)
        {
            nameMap.Text = mapName;
            _mapName = mapName;
        }

        private void AddToRotation_Click(object sender, RoutedEventArgs e)
        {
            string result = "";
            _serverSession.SendCommand("Add Map To Rotation", new string[1] { _mapName }, out result);
            if (result == "SUCCESS")
                _serverSession.UpdateServerSettings();
                Close();
        }

        private void SetButton_Click(object sender, RoutedEventArgs e)
        {
            string result = "";
            _serverSession.SendCommand("Map Change", new string[1] {_mapName}, out result);
            if (result == "SUCCESS")
                Close();
        }
    }
}
