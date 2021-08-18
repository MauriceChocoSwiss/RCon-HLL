using System.Collections.Generic;
using System.Windows;
using RconClient;

namespace RCon_Plus.Views
{
    /// <summary>
    /// Logique d'interaction pour ActionForPlayer.xaml
    /// </summary>
    public partial class ActionForPlayer : Window
    {
        private readonly string _actionName;
        private readonly string _userName;
        private readonly ServerSession _serverSession;
        private readonly bool _reason;
        private readonly bool _adminName;
        private readonly bool _duration;

        public ActionForPlayer(string actionName, string text, string userName, ServerSession serverSession, bool reason = false, bool adminName = false, bool duration = false)
        {
            InitializeComponent();
            textBlock.Text = text + userName;
            _actionName = actionName;
            _userName = userName;
            _serverSession = serverSession;
            _reason = reason;
            _adminName = adminName;
            _duration = duration;
            if (reason)
                reasonBox.IsEnabled = true;
            if (adminName)
                adminNameBox.IsEnabled = true;
            if (duration)
                durationBox.IsEnabled = true;
        }

        private void CloseMessageBox(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Action_Click(object sender, RoutedEventArgs e)
        {
            List<string> paramToSend = new();

            paramToSend.Add(_userName);
                      
            if (_duration)
            {
                if (string.IsNullOrEmpty(durationBox.Text))
                {
                    MessageBox.Show("Vous devez entrer une durée");
                    return;
                }
                paramToSend.Add(durationBox.Text);
            }
            if (_reason)
                paramToSend.Add(reasonBox.Text);
            if (_adminName)
                paramToSend.Add(adminNameBox.Text);

            string result = "";

            _serverSession.SendCommand(_actionName, paramToSend.ToArray(), out result);

            if (result == "SUCCESS")
            {
                _serverSession.UpdateBan();
                Close();
            }
        }

    }
}
