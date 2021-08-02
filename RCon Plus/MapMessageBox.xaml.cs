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
using System.Windows.Shapes;

namespace RCon_Plus
{
    /// <summary>
    /// Logique d'interaction pour MapMessageBox.xaml
    /// </summary>
    public partial class MapMessageBox : Window
    {
        private string _mapName;
        public MapMessageBox()
        {
            InitializeComponent();
        }

        public void SetMap(string mapName)
        {
            _mapName = mapName;
        }

        private void CloseMessageBox(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void AddToRotation_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SetButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
