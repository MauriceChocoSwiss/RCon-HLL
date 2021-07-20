﻿using System;
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
        ConnectionControl connectionControl = new ConnectionControl();
        public MainWindow()
        {
            InitializeComponent();
            ServerPanel.Children.Add(connectionControl);
            PlayerControlTabItem.Content = new PlayerControl();
        }

        public void AfficherServerInfo()
        {
            ServerPanel.Children.Remove(connectionControl);
            //Afficher les infos serveur
        }
    }
}
