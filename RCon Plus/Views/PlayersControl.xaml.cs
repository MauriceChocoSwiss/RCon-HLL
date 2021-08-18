﻿using System.Timers;
using System.Windows;
using System.Windows.Controls;
using RCon_Plus.Views;
using RconClient;

namespace RCon_Plus
{

    public partial class PlayersControl : UserControl
    {
        private readonly ServerSession _serverSession;
        private string Grid0To25SelectedValue = "";
        private string Grid25To50SelectedValue = "";
        private string Grid50To75SelectedValue = "";
        private string Grid75To100SelectedValue = "";
        private string playerNameToSend = "";

        public PlayersControl(ServerSession serverSession)
        {
            InitializeComponent();
            _serverSession = serverSession;
            GetterTimer();
            PlayersDataGrid0To25.DataContext = serverSession;
            PlayersDataGrid25To50.DataContext = serverSession;
            PlayersDataGrid50To75.DataContext = serverSession;
            PlayersDataGrid75To100.DataContext = serverSession;
        }


        private void GetterTimer()
        {
            Timer timer = new(5000) { AutoReset = true, Enabled = true };

            timer.Elapsed += new ElapsedEventHandler(GetterTimerElapsed);
        }

        private void ConcatPlayerName()
        {
            playerNameToSend = string.Concat(Grid0To25SelectedValue + Grid25To50SelectedValue + Grid50To75SelectedValue + Grid75To100SelectedValue);
        }

        private void GetterTimerElapsed(object source, ElapsedEventArgs e) => _serverSession.UpdatePlayersList();

        private void PlayersDataGrid0To25_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = PlayersDataGrid0To25.SelectedItem;
            if (selected != null)
            {
                Grid0To25SelectedValue = selected.ToString();

                PlayersDataGrid25To50.UnselectAll();
                PlayersDataGrid50To75.UnselectAll();
                PlayersDataGrid75To100.UnselectAll();
                Grid25To50SelectedValue = "";
                Grid50To75SelectedValue = "";
                Grid75To100SelectedValue = "";
            }
        }

        private void PlayersDataGrid25To50_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = PlayersDataGrid25To50.SelectedItem;
            if (selected != null)
            {
                Grid25To50SelectedValue = selected.ToString();

                PlayersDataGrid0To25.UnselectAll();
                PlayersDataGrid50To75.UnselectAll();
                PlayersDataGrid75To100.UnselectAll();
                Grid0To25SelectedValue = "";
                Grid50To75SelectedValue = "";
                Grid75To100SelectedValue = "";
            }
        }

        private void PlayersDataGrid50To75_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = PlayersDataGrid50To75.SelectedItem;
            if (selected != null)
            {
                Grid50To75SelectedValue = selected.ToString();

                PlayersDataGrid0To25.UnselectAll();
                PlayersDataGrid25To50.UnselectAll();
                PlayersDataGrid75To100.UnselectAll();
                Grid0To25SelectedValue = "";
                Grid25To50SelectedValue = "";
                Grid75To100SelectedValue = "";
            }
        }

        private void PlayersDataGrid75To100_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = PlayersDataGrid75To100.SelectedItem;
            if (selected != null)
            {
                Grid75To100SelectedValue = selected.ToString();

                PlayersDataGrid0To25.UnselectAll();
                PlayersDataGrid25To50.UnselectAll();
                PlayersDataGrid50To75.UnselectAll();
                Grid0To25SelectedValue = "";
                Grid25To50SelectedValue = "";
                Grid50To75SelectedValue = "";
            }
        }

        private void KickButton(object sender, System.Windows.RoutedEventArgs e)
        {
            ConcatPlayerName();
            if (string.IsNullOrEmpty(playerNameToSend))
                return;

            ActionForPlayer action = new ActionForPlayer("Kick", "Etes-vous sur de vouloir kicker ", playerNameToSend, _serverSession, true);
            action.Show();
        }

        private void PunishClick(object sender, RoutedEventArgs e)
        {
            ConcatPlayerName();
            if (string.IsNullOrEmpty(playerNameToSend))
                return;

            ActionForPlayer action = new ActionForPlayer("Punish", "Etes-vous sur de vouloir punish ", playerNameToSend, _serverSession, true);
            action.Show();
        }

        private void TempBanClick(object sender, RoutedEventArgs e)
        {
            ConcatPlayerName();
            if (string.IsNullOrEmpty(playerNameToSend))
                return;

            ActionForPlayer action = new ActionForPlayer("Temporary Ban By Steam 64 ID", "Etes-vous sur de vouloir TempBan ", playerNameToSend, _serverSession, true, true, true);
            action.Show();
        }

        private void PermaBanClick(object sender, RoutedEventArgs e)
        {
            ConcatPlayerName();
            if (string.IsNullOrEmpty(playerNameToSend))
                return;

            ActionForPlayer action = new ActionForPlayer("Permanent Ban By Steam 64 ID", "Etes-vous sur de vouloir PermaBan ", playerNameToSend, _serverSession, true, true);
            action.Show();
        }

        private void SwitchTeamNowClick(object sender, RoutedEventArgs e)
        {
            ConcatPlayerName();

            if (string.IsNullOrEmpty(playerNameToSend))
                return;

            ActionForPlayer action = new ActionForPlayer("Switch Teams Now", "Etes-vous sur de vouloir changer d'équipe ", playerNameToSend, _serverSession);
            action.Show();
        }

        private void SwitchTeamDeathClick(object sender, RoutedEventArgs e)
        {
            ConcatPlayerName();

            if (string.IsNullOrEmpty(playerNameToSend))
                return;

            ActionForPlayer action = new ActionForPlayer("Switch Teams On Death", "Etes-vous sur de vouloir changer d'équipe ", playerNameToSend, _serverSession);
            action.Show();
        }
    }
}