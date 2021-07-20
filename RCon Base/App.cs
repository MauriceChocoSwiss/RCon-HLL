using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace RconClient
{
    public class App : Application
    {
        public void InitializeComponent() => this.StartupUri = new Uri("Views/MainWindow.xaml", UriKind.Relative);

        [STAThread]
        public static void Main()
        {
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}
