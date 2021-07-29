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
    /// Logique d'interaction pour Maps.xaml
    /// </summary>
    public partial class Maps : UserControl
    {
        private string _mapName;

        MapMessageBox _mapMessageBox;
        public Maps()
        {
            InitializeComponent();
            _mapMessageBox = new MapMessageBox();
        }

        private void showMessageBox(object sender, RoutedEventArgs e)
        {
            _mapName = (sender as Button).Name.ToString();
            _mapMessageBox.Show();
            _mapMessageBox.SetMap(_mapName);
        }
    }
}
