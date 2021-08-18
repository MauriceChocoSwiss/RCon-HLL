using System.Windows.Controls;

namespace RCon_Plus
{
    /// <summary>
    /// Logique d'interaction pour Settings.xaml
    /// </summary>
    public partial class ServerSettings : UserControl
    {
        public ServerSettings()
        {
            InitializeComponent();
            MapScroll.Content = new Maps();
        }

    }
}
