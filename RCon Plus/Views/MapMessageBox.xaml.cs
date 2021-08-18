using System.Windows;

namespace RCon_Plus
{
    /// <summary>
    /// Logique d'interaction pour MapMessageBox.xaml
    /// </summary>
    public partial class MapMessageBox : Window
    {
        public MapMessageBox()
        {
            InitializeComponent();
        }

        public void SetMap(string mapName)
        {
            nameMap.Text = mapName;
        }

        private void AddToRotation_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SetButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
