using System.Windows;
using System.Windows.Controls;

namespace RCon_Plus
{
    /// <summary>
    /// Logique d'interaction pour Maps.xaml
    /// </summary>
    public partial class Maps : UserControl
    {
        private string _mapName;
        public Maps()
        {
            InitializeComponent();

        }

        private void ShowMessageBox(object sender, RoutedEventArgs e)
        {
            MapMessageBox mapMessageBox = new MapMessageBox();
            _mapName = (sender as Button).Name.ToString();
            mapMessageBox.Show();
            mapMessageBox.SetMap(_mapName);
        }
    }
}
