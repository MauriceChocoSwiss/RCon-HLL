using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace RconClient
{
    public class ServerControl : System.Windows.Controls.UserControl, IComponentConnector
    {
        private static int s_getterInterval = 10000;
        private System.Timers.Timer m_getterTimer;
        private ServerSession m_serverSession;
        private Dictionary<RconGetter, string> m_getterToData = new Dictionary<RconGetter, string>();
        internal WrapPanel wrapPanelCommands;
        internal Rectangle rectangleFeedback;
        internal System.Windows.Controls.TextBox textBlockFeedback;
        internal System.Windows.Controls.ListBox listBoxServerInfo;
        private bool _contentLoaded;

        public ServerControl(ServerSession serverSession)
        {
            this.InitializeComponent();
            this.m_serverSession = serverSession;
            this.DataContext = (object)serverSession;
            this.CreateReconnectButton();
            this.CreateCommandButtons();
            this.CreateVIPsExportImportButtons();
            this.CreateAdminsExportImportButtons();
            this.CreateServerInfoView();
            this.StartGetterTimer();
        }

        ~ServerControl() => this.m_getterTimer.Enabled = false;

        private void CreateCommandButtons()
        {
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding("Authenticated");
            binding.Source = (object)this.m_serverSession;
            foreach (RconCommand availableCommand in RconStaticLibrary.AvailableCommands)
            {
                System.Windows.Controls.Button button1 = new System.Windows.Controls.Button();
                button1.Content = (object)availableCommand.m_name;
                button1.Margin = new Thickness(0.0, 5.0, 0.0, 5.0);
                button1.Padding = new Thickness(4.0);
                button1.DataContext = (object)availableCommand;
                System.Windows.Controls.Button button2 = button1;
                button2.Click += new RoutedEventHandler(this.OnCommandButtonClicked);
                button2.SetBinding(UIElement.IsEnabledProperty, (BindingBase)binding);
                this.wrapPanelCommands.Children.Add((UIElement)button2);
            }
        }

        private void CreateReconnectButton()
        {
            System.Windows.Controls.Button button1 = new System.Windows.Controls.Button();
            button1.Content = (object)"Connect to...";
            button1.Margin = new Thickness(0.0, 5.0, 0.0, 5.0);
            button1.Padding = new Thickness(4.0);
            System.Windows.Controls.Button button2 = button1;
            button2.Click += new RoutedEventHandler(this.OnReconnectClicked);
            this.wrapPanelCommands.Children.Add((UIElement)button2);
        }

        private void OnReconnectClicked(object sender, RoutedEventArgs routedEventArgs)
        {
            ConnectDialog connectDialog = new ConnectDialog(this.m_serverSession.ConnectionDetails);
            bool? nullable = connectDialog.ShowDialog();
            bool flag = true;
            if ((nullable.GetValueOrDefault() == flag ? (nullable.HasValue ? 1 : 0) : 0) == 0)
                return;
            (System.Windows.Application.Current.MainWindow as MainWindow).OnNewSessionStarted(connectDialog);
        }

        private void CreateVIPsExportImportButtons()
        {
            System.Windows.Controls.Button button1 = new System.Windows.Controls.Button();
            button1.Content = (object)"Export VIPs";
            button1.Margin = new Thickness(0.0, 5.0, 0.0, 5.0);
            button1.Padding = new Thickness(4.0);
            System.Windows.Controls.Button button2 = button1;
            button2.Click += new RoutedEventHandler(this.OnExportVIPsClicked);
            System.Windows.Controls.Button button3 = new System.Windows.Controls.Button();
            button3.Content = (object)"Import VIPs";
            button3.Margin = new Thickness(0.0, 5.0, 0.0, 5.0);
            button3.Padding = new Thickness(4.0);
            System.Windows.Controls.Button button4 = button3;
            button4.Click += new RoutedEventHandler(this.OnImportVIPsClicked);
            this.wrapPanelCommands.Children.Add((UIElement)button2);
            this.wrapPanelCommands.Children.Add((UIElement)button4);
        }

        private void OnExportVIPsClicked(object sender, RoutedEventArgs routedEventArgs)
        {
            string[] data;
            if (!new RconGetter("VIPs", "GettingVIPs", true, false, "get VipIds").GetData(this.m_serverSession, out data))
                return;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.DefaultExt = "txt";
                openFileDialog.CheckFileExists = false;
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                    return;
                File.WriteAllLines(openFileDialog.FileName, data);
            }
        }

        private void OnImportVIPsClicked(object sender, RoutedEventArgs routedEventArgs)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                    return;
                string fileName = openFileDialog.FileName;
                foreach (string readAllLine in File.ReadAllLines(openFileDialog.FileName))
                {
                    this.m_serverSession.SendMessage("VipAdd " + readAllLine);
                    string receivedMessage;
                    this.m_serverSession.ReceiveMessage(out receivedMessage);
                    if (RconStaticLibrary.IsFailReply(receivedMessage))
                        break;
                }
            }
        }

        private void CreateAdminsExportImportButtons()
        {
            System.Windows.Controls.Button button1 = new System.Windows.Controls.Button();
            button1.Content = (object)"Export Admins";
            button1.Margin = new Thickness(0.0, 5.0, 0.0, 5.0);
            button1.Padding = new Thickness(4.0);
            System.Windows.Controls.Button button2 = button1;
            button2.Click += new RoutedEventHandler(this.OnExportAdminsClicked);
            System.Windows.Controls.Button button3 = new System.Windows.Controls.Button();
            button3.Content = (object)"Import Admins";
            button3.Margin = new Thickness(0.0, 5.0, 0.0, 5.0);
            button3.Padding = new Thickness(4.0);
            System.Windows.Controls.Button button4 = button3;
            button4.Click += new RoutedEventHandler(this.OnImportAdminsClicked);
            this.wrapPanelCommands.Children.Add((UIElement)button2);
            this.wrapPanelCommands.Children.Add((UIElement)button4);
        }

        private void OnExportAdminsClicked(object sender, RoutedEventArgs routedEventArgs)
        {
            string[] data;
            if (!new RconGetter("Admins", "GettingAdmins", true, false, "get AdminIds").GetData(this.m_serverSession, out data))
                return;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.DefaultExt = "txt";
                openFileDialog.CheckFileExists = false;
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                    return;
                File.WriteAllLines(openFileDialog.FileName, data);
            }
        }

        private void OnImportAdminsClicked(object sender, RoutedEventArgs routedEventArgs)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                    return;
                string fileName = openFileDialog.FileName;
                foreach (string readAllLine in File.ReadAllLines(openFileDialog.FileName))
                {
                    this.m_serverSession.SendMessage("AdminAdd " + readAllLine);
                    string receivedMessage;
                    this.m_serverSession.ReceiveMessage(out receivedMessage);
                    if (RconStaticLibrary.IsFailReply(receivedMessage))
                        break;
                }
            }
        }

        private void CreateServerInfoView() => this.listBoxServerInfo.DataContext = (object)this.m_serverSession;

        private void StartGetterTimer()
        {
            this.m_getterTimer = new System.Timers.Timer((double)ServerControl.s_getterInterval)
            {
                AutoReset = true,
                Enabled = true
            };
            this.m_getterTimer.Elapsed += new ElapsedEventHandler(this.OnGetterIntervalElapsed);
        }

        private void OnCommandButtonClicked(object sender, RoutedEventArgs routedEventArgs) => ((sender as FrameworkElement).DataContext as RconCommand).StartExecuting(this.m_serverSession);

        private void OnGetterIntervalElapsed(object source, ElapsedEventArgs e) => this.m_serverSession.UpdateServerInfo();

        [DebuggerNonUserCode]
        //[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
                return;
            this._contentLoaded = true;
            System.Windows.Application.LoadComponent((object)this, new Uri("/RconClient;component/servercontrol.xaml", UriKind.Relative));
        }

        [DebuggerNonUserCode]
        //[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.wrapPanelCommands = (WrapPanel)target;
                    break;
                case 2:
                    this.rectangleFeedback = (Rectangle)target;
                    break;
                case 3:
                    this.textBlockFeedback = (System.Windows.Controls.TextBox)target;
                    break;
                case 4:
                    this.listBoxServerInfo = (System.Windows.Controls.ListBox)target;
                    break;
                default:
                    this._contentLoaded = true;
                    break;
            }
        }
    }
}
