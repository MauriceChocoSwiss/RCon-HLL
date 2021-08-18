using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RCon_Plus;

namespace RconClient
{

    public class ServerSession : INotifyPropertyChanged
    {
        private static readonly int _messageBufferSize = 8196;
        private static readonly string _rconLoginCommand = "login {0}";
        private bool _authenticated;
        private bool _statsAuthenticated;
        private string _statusMessage;
        private readonly string[] _serverInfoArray;
        private ICollection<string> _playersList0To25;
        private ICollection<string> _playersList25To50;
        private ICollection<string> _playersList50To75;
        private ICollection<string> _playersList75To100;
        private ICollection<string> _tempBanList;
        private ICollection<string> _banList;
        private ICollection<string> _adminGroup;
        private ICollection<string> _adminList;
        private ICollection<string> _vipList;
        private ICollection<string> _mapRotList;
        private ICollection<string> _profWordList;
        private string[] _serverSettings;
        private TcpClient _client;
        private byte[] _xorKey;
        private bool _lastCommandSucceeded;
        private readonly Mutex _communicationMutex;
        private readonly string _host;
        private readonly int _port;
        private readonly string _password;

        public bool Authenticated
        {
            get => _authenticated;
            private set
            {
                _authenticated = value;
                OnPropertyChanged(nameof(Authenticated));
            }
        }

        public bool StatsAuthenticated
        {
            get => _statsAuthenticated;
            private set
            {
                _statsAuthenticated = value;
                OnPropertyChanged(nameof(StatsAuthenticated));
            }
        }

        public ObservableCollection<string> ServerInfo => new(_serverInfoArray);
        public ObservableCollection<string> PlayersList0To25 => new(_playersList0To25);
        public ObservableCollection<string> PlayersList25To50 => new(_playersList25To50);
        public ObservableCollection<string> PlayersList50To75 => new(_playersList50To75);
        public ObservableCollection<string> PlayersList75To100 => new(_playersList75To100);
        public ObservableCollection<string> TempBanList => new(_tempBanList);
        public ObservableCollection<string> BanList => new(_banList);
        public ObservableCollection<string> AdminGroup => new(_adminGroup);
        public ObservableCollection<string> AdminList => new(_adminList);
        public ObservableCollection<string> VipList => new(_vipList);
        public ObservableCollection<string> MapRotList => new(_mapRotList);
        public ObservableCollection<string> ProfWordList => new(_profWordList);
        public ObservableCollection<string> ServerSettings => new(_serverSettings);


        public bool Disconnected => _client == null || !_client.Connected || !Authenticated;

        public string StatusMessage
        {
            get => _statusMessage;
            private set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
                MainWindow._main.ChangeConnectionStatus(_statusMessage);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ServerSession(string host, int port, string password)
        {
            _host = host;
            _port = port;
            _password = password;
            _serverInfoArray = new string[3];
            _communicationMutex = new Mutex();
            _tempBanList = new List<string>();
            _banList = new List<string>();
            _serverSettings = new string[8] { "", "0", "0", "0", "0", "0", "0", "0" };
            Connect();
        }

        ~ServerSession()
        {
            if (_client == null)
                return;
            _client.Close();
        }

        public bool SendMessage(string message, bool encrypted = true) => SendBytes(Encoding.UTF8.GetBytes(message), encrypted);

        private bool SendBytes(byte[] message, bool encrypted = true)
        {
            if (!_communicationMutex.WaitOne(5000))
                return false;
            try
            {
                NetworkStream stream = _client.GetStream();
                if (encrypted)
                    message = XORMessage(message);
                byte[] buffer = message;
                int length = message.Length;
                stream.Write(buffer, 0, length);
                return true;
            }
            catch (Exception)
            {
                OnConnectionProblems("Disconnected from the server.");
                _communicationMutex.ReleaseMutex();
                return false;
            }
        }

        public bool ReceiveMessage(out string receivedMessage, bool decrypted = true, bool isCommand = true, bool isMapList = false)
        {
            receivedMessage = "";
            byte[] receivedBytes;
            bool bytes = ReceiveBytes(out receivedBytes, decrypted);
            if (!bytes)
                return false;
            try
            {
                receivedMessage = Encoding.UTF8.GetString(receivedBytes, 0, receivedBytes.Length);
            }
            catch (DecoderFallbackException)
            {
                if (isCommand && !isMapList)
                {
                    StatusMessage = "Failed to decode server response.";
                    _lastCommandSucceeded = false;
                    OnPropertyChanged("Status");
                }
                return false;
            }

            if (isCommand && !isMapList)
            {
                StatusMessage = receivedMessage;
                if (RconStaticLibrary.IsSuccessReply(receivedMessage))
                    _lastCommandSucceeded = true;
                OnPropertyChanged("Status");
            }
            return bytes;
        }

        private bool ReceiveBytes(out byte[] receivedBytes, bool decrypted = true)
        {
            receivedBytes = new byte[ServerSession._messageBufferSize];
            int newSize;
            try
            {
                newSize = _client.GetStream().Read(receivedBytes, 0, receivedBytes.Length);
            }
            catch (Exception)
            {
                OnConnectionProblems("Disconnected from the server.");
                _communicationMutex.ReleaseMutex();
                return false;
            }
            Array.Resize<byte>(ref receivedBytes, newSize);
            if (decrypted)
                receivedBytes = XORMessage(receivedBytes);
            _communicationMutex.ReleaseMutex();
            return true;
        }

        private byte[] XORMessage(byte[] message)
        {
            for (int index = 0; index < message.Length; ++index)
                message[index] ^= _xorKey[index % _xorKey.Length];
            return message;
        }

        private void OnConnectionProblems(string errorMessage)
        {
            StatusMessage = errorMessage;
            Authenticated = false;
            OnPropertyChanged("Disconnected");
            OnPropertyChanged("Status");
        }

        public void Connect()
        {
            try
            {
                _client = new TcpClient();
                TcpClient client = _client;
                string serverAddress = _host;
                int serverPort = _port;
                client.ConnectAsync(serverAddress, serverPort).ContinueWith((Action<Task>)(t => OnConnected(t)));
            }
            catch (ArgumentNullException ex)
            {
                OnConnectionProblems("Provided hostname is null.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                OnConnectionProblems("Invalid portnumber provided.");
            }
            catch (SocketException ex)
            {
                OnConnectionProblems("The provided address and port are inaccessible.");
            }
        }

        protected void OnConnected(Task connectionTask)
        {
            try
            {
                try
                {
                    connectionTask.Wait();
                }
                catch (AggregateException ex)
                {
                    ex.Handle((Func<Exception, bool>)(x =>
                   {
                       throw x;
                   }));
                }
            }
            catch (ArgumentNullException ex)
            {
                OnConnectionProblems("Provided hostname is null.");
                return;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                OnConnectionProblems("Invalid portnumber provided.");
                return;
            }
            catch (SocketException ex)
            {
                OnConnectionProblems("The provided address and port are inaccessible.");
                return;
            }
            try
            {
                _communicationMutex.WaitOne();
                if (!ReceiveBytes(out _xorKey, false))
                {
                    StatusMessage = "No response from the server. Are the address and port correct?";
                }
                else
                {
                    Debug.WriteLine("XORKey received");
                    SendMessage(string.Format(ServerSession._rconLoginCommand, (object)QuoteString(_password)));
                    string receivedMessage;
                    _lastCommandSucceeded = ReceiveMessage(out receivedMessage) && RconStaticLibrary.IsSuccessReply(receivedMessage);
                    Authenticated = _lastCommandSucceeded;
                    OnPropertyChanged("Disconnected");
                    OnPropertyChanged("Status");
                    if (Authenticated)
                    {
                        Debug.WriteLine("Connected");
                        StatusMessage = "Login successful.";
                        UpdateServerInfo();
                        UpdatePlayersList();
                        MainWindow._main.AfficherServerInfo();
                    }
                    else
                        StatusMessage = "Login failed. Is your password correct?";
                }
            }
            catch (ObjectDisposedException ex)
            {
                OnConnectionProblems("Internal error: mutex has been disposed.");
            }
            catch (AbandonedMutexException ex)
            {
                OnConnectionProblems("Internal error: mutex has been abandoned.");
            }
            catch (InvalidOperationException ex)
            {
                OnConnectionProblems("Internal error: invalid mutex operation.");
            }
            catch (ArgumentNullException ex)
            {
                OnConnectionProblems("Empty login string provided.");
                _communicationMutex.ReleaseMutex();
            }
            catch (FormatException ex)
            {
                OnConnectionProblems("Invalid login string provided.");
                _communicationMutex.ReleaseMutex();
            }
        }

        public async void UpdateServerInfo()
        {
            int i = 0;
            if (Disconnected)
                return;
            foreach (RconGetter rconGetter in RconStaticLibrary.AvailableGetters.Where<RconGetter>((Func<RconGetter, bool>)(getter => getter.AutoRefresh)).ToList<RconGetter>())
            {
                RconGetter getter = rconGetter;
                string[] data = new string[1] { "" };
                if (!await Task.Run<bool>((Func<bool>)(() => getter.GetData(this, out data))))
                    data = new string[1] { "Failed to get data." };
                else if (RconStaticLibrary.IsFailReply(data[0]))
                    data = new string[1]
                    {
                        "Getter not supported by the server."
                    };
                _serverInfoArray[i] = string.Join("", data);
                i++;
            }
            OnPropertyChanged("ServerInfo");
        }

        public async void UpdatePlayersList()
        {
            if (Disconnected)
                return;

            _playersList0To25 = new List<string>();
            _playersList25To50 = new List<string>();
            _playersList50To75 = new List<string>();
            _playersList75To100 = new List<string>();
            string[] data = new string[0];

            int i = 0;

            await Task.Run(() =>
            {
                RconStaticLibrary.AvailableGetters.FirstOrDefault(r => r.Name == "PlayerNames").GetData(this, out data);
                foreach (string nickName in data.OrderBy(c => c))
                //for(int e = 0; e < 100; e++)
                {
                    switch (i)
                    {
                        case < 25:
                            _playersList0To25.Add(nickName);
                            OnPropertyChanged("PlayersList0To25");
                            break;
                        case < 50:
                            _playersList25To50.Add(nickName);
                            OnPropertyChanged("PlayersList25To50");
                            break;
                        case < 75:
                            _playersList50To75.Add(nickName);
                            OnPropertyChanged("PlayersList50To75");
                            break;
                        case < 101:
                            _playersList75To100.Add(nickName);
                            OnPropertyChanged("PlayersList75To100");
                            break;
                    }
                    i++;
                }
            });

        }

        public async void UpdateBan()
        {
            if (Disconnected)
                return;

            _tempBanList = new List<string>();
            _banList = new List<string>();

            await Task.Run(() =>
            {
                string[] data = new string[0];
                RconStaticLibrary.AvailableGetters.FirstOrDefault(r => r.Name == "TempBans").GetData(this, out data);
                foreach (string value in data.OrderBy(c => c))
                {
                    _tempBanList.Add(value);
                }
                OnPropertyChanged("TempBanList");
            });

            await Task.Run(() =>
            {
                string[] data = new string[0];
                RconStaticLibrary.AvailableGetters.FirstOrDefault(r => r.Name == "PermaBans").GetData(this, out data);
                foreach (string value in data.OrderBy(c => c))
                {
                    _banList.Add(value);
                }
                OnPropertyChanged("BanList");
            });
        }

        public async void UpdateServerSettings()
        {
            if (Disconnected)
                return;

            _adminGroup = new List<string>();
            _adminList = new List<string>();
            _vipList = new List<string>();
            _mapRotList = new List<string>();
            _profWordList = new List<string>();

            _serverSettings[0] = "false";

            await Task.Run(() =>
            {
                string[] data = new string[0];
                RconStaticLibrary.AvailableGetters.FirstOrDefault(r => r.Name == "AdminGroups").GetData(this, out data);
                foreach (string value in data.OrderBy(c => c))
                {
                    _adminGroup.Add(value);
                }

                OnPropertyChanged("AdminGroup");
            });

            await Task.Run(() =>
            {
                string[] data = new string[0];
                RconStaticLibrary.AvailableGetters.FirstOrDefault(r => r.Name == "AdminIds").GetData(this, out data);
                foreach (string value in data.OrderBy(c => c))
                {
                    _adminList.Add(value);
                }

                _adminList.OrderBy(a => a);

                OnPropertyChanged("AdminList");
            });

            await Task.Run(() =>
            {
                string[] data = new string[0];
                RconStaticLibrary.AvailableGetters.FirstOrDefault(r => r.Name == "VipIds").GetData(this, out data);
                foreach (string value in data.OrderBy(c => c))
                {
                    _vipList.Add(value);
                }

                _vipList.OrderBy(v => v);

                OnPropertyChanged("VipList");
            });

            await Task.Run(() =>
            {
                string data = "";
                SendCommand("List Maps In Rotation", new string[0], out data, true);
                string[] mapList = data.Split("\n");

                foreach (string value in mapList.OrderBy(c => c))
                {
                    if (!string.IsNullOrEmpty(value))
                        _mapRotList.Add(value);
                }

                _mapRotList.OrderBy(m => m);

                OnPropertyChanged("MapRotList");
            });

            await Task.Run(() =>
            {
                string[] data = new string[0];
                RconStaticLibrary.AvailableGetters.FirstOrDefault(r => r.Name == "ProfanityWords").GetData(this, out data);
                foreach (string value in data.OrderBy(c => c))
                {
                    _profWordList.Add(value);
                }

                _profWordList.OrderBy(w => w);

                OnPropertyChanged("ProfWordList");
            });

            await Task.Run(() =>
            {
                string[] data = new string[0];
                RconStaticLibrary.AvailableGetters.FirstOrDefault(r => r.Name == "AutoBalanceEnabled").GetData(this, out data);
                if (data[0] == "on")
                    _serverSettings[0] = "true";
            });

            await Task.Run(() =>
            {
                string[] data = new string[0];
                RconStaticLibrary.AvailableGetters.FirstOrDefault(r => r.Name == "AutoBalanceThreshold").GetData(this, out data);
                _serverSettings[1] = data[0];
            });

            await Task.Run(() =>
            {
                string[] data = new string[0];
                RconStaticLibrary.AvailableGetters.FirstOrDefault(r => r.Name == "IdleTime").GetData(this, out data);
                _serverSettings[2] = data[0];
            });

            await Task.Run(() =>
            {
                string[] data = new string[0];
                RconStaticLibrary.AvailableGetters.FirstOrDefault(r => r.Name == "HighPing").GetData(this, out data);
                _serverSettings[3] = data[0];
            });

            await Task.Run(() =>
            {
                string[] data = new string[0];
                RconStaticLibrary.AvailableGetters.FirstOrDefault(r => r.Name == "MaxQueuedPlayers").GetData(this, out data);
                _serverSettings[4] = data[0];
            });

            await Task.Run(() =>
            {
                string[] data = new string[0];
                RconStaticLibrary.AvailableGetters.FirstOrDefault(r => r.Name == "NumVipSlots").GetData(this, out data);
                _serverSettings[5] = data[0];
            });

            await Task.Run(() =>
            {
                string[] data = new string[0];
                RconStaticLibrary.AvailableGetters.FirstOrDefault(r => r.Name == "TeamSwitchCooldown").GetData(this, out data);
                _serverSettings[6] = data[0];
                OnPropertyChanged("ServerSettings");
            });
        }

        public void SendCommand(string commandName, string[] parameterToSend, out string receivedMessage, bool isMapList = false)
        {
            int index = 0;
            string tempReceived = "";
            var command = RconStaticLibrary.AvailableCommands.FirstOrDefault(r => r.Name == commandName);
            List<string> stringList = new();

            if (command != null)
            {
                foreach (string paramToSend in parameterToSend)
                {
                    RconCommandParameter parameter = command.Parameters[index];
                    string value = paramToSend;
                    if (!VerifyUserInput(value, parameter.Type, parameter.Optional))
                        break;
                    if (parameter.Quoted)
                        value = QuoteString(value);
                    stringList.Add(value);
                    index++;
                }
                receivedMessage = "";
                string messageToSend = string.Format(command.MessageTemplate, stringList.ToArray());
                if (!SendMessage(messageToSend))
                    return;
                ReceiveMessage(out tempReceived, true, true, isMapList);
            }
            receivedMessage = tempReceived;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged == null)
                return;
            propertyChanged((object)this, new PropertyChangedEventArgs(name));
        }

        public static string QuoteString(string input)
        {
            string str = "\"";
            for (int index = 0; index < input.Length; ++index)
            {
                str = input[index] != '"' ? (input[index] != '\\' ? str + input[index].ToString() : str + "\\\\") : str + "\\\"";
            }

            return str + "\"";
        }

        public bool VerifyUserInput(string userInput, string Type, bool Optional = true)
        {
            if (string.IsNullOrEmpty(userInput) && !Optional)
                return false;
            string type = Type;
            if (type == "int")
                return int.TryParse(userInput, out int _);
            if (type == "bool" || type == "string")
                return true;
            return type == "password" && !userInput.Any<char>(new Func<char, bool>(char.IsWhiteSpace));
        }
    }
}
