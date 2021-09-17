using System.Windows;

namespace RCon_Plus.Views
{
    /// <summary>
    /// Logique d'interaction pour PlayerInfo.xaml
    /// </summary>
    public partial class PlayerInfo : Window
    {
        public PlayerInfo(string value)
        {
            InitializeComponent();
            playerInfoBox.Text = value;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
