using System;
using System.Diagnostics;
using System.Windows;

namespace RconClient
{
    public class App : Application
    {
        [DebuggerNonUserCode]
        //[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() => this.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);

        [STAThread]
        [DebuggerNonUserCode]
        //[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public static void Main()
        {
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}
