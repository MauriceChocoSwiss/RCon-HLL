using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RCon_Plus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ConnectionControl _connectionControl = new();

        public MainWindow()
        {
            InitializeComponent();
            ServerPanel.Children.Add(_connectionControl);
            PlayerControlTabItem.Content = new PlayersControl();
            SettingsTabItem.Content = new Settings();
            LogTabItem.Content = new LogsControl();
        }

        public void AfficherServerInfo()
        {
            ServerPanel.Children.Remove(_connectionControl);
            ServerPanel.Children.Add(new ServerInfo());
        }
    }
}
