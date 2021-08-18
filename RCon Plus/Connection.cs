using System;

namespace RCon_Plus
{
    public enum ConnectionStatus
    {
        Disconnected = 0,
        Connecting = 1,
        Connected = 2,
        Disconnecting = 3,
    }

    internal class Connection : IDisposable
    {
        private const int _TIMEOUT = 5000;
        private const int _BUFFER_SIZE = 8196;
        //private const string LOGIN_CMD = "login {0}";

        private readonly string _host;
        private readonly int _port;
        private readonly string _password;
        private ConnectionStatus _status = ConnectionStatus.Disconnected;
        private System.Net.Sockets.TcpClient _client;
        private readonly byte[] _buffer = new byte[_BUFFER_SIZE];
        private byte[] _xorKey;

        public delegate void ConnectionStatusChangedtHandler(object sender, ConnectionStatus status);
        public event ConnectionStatusChangedtHandler ConnectionStatusChanged;

        public Connection(string host, int port, string password)
        {
            _host = host;
            _port = port;
            _password = password;
        }

        public ConnectionStatus Status
        {
            get => _status;
            private set
            {
                if (_status != value)
                {
                    _status = value;
                    ConnectionStatusChanged?.Invoke(this, value);
                }
            }
        }

        public string Error { get; private set; } = "";

        public void Connect()
        {
            if (_status != ConnectionStatus.Disconnected)
            {
                return;
            }

            Error = "";
            Status = ConnectionStatus.Connecting;

            if (_client == null)
            {
                _client = new System.Net.Sockets.TcpClient()
                {
                    SendTimeout = _TIMEOUT,
                    ReceiveTimeout = _TIMEOUT
                };
            }

            _ = _client.BeginConnect(_host, _port, new AsyncCallback(OnConnect), _client);
        }

        public void Disconnect()
        {
            if (_status != ConnectionStatus.Connected)
            {
                return;
            }

            Status = ConnectionStatus.Disconnecting;
            Close();
        }

        ~Connection()
        {
            Dispose();
        }

        public void Dispose()
        {
            Disconnect();
            GC.SuppressFinalize(this);
        }

        public void Write(string message)
        {
            if (_status is ConnectionStatus.Connecting or ConnectionStatus.Connected)
            {
                byte[] bytes = XORMessage(System.Text.Encoding.UTF8.GetBytes(message));
                _client.GetStream().Write(bytes);
            }
        }

        public bool Read(out string message)
        {
            message = "";

            if (_status is ConnectionStatus.Connecting or ConnectionStatus.Connected)
            {
                int read = _client.GetStream().Read(_buffer, 0, _buffer.Length);
                System.Diagnostics.Debug.WriteLine("Received " + read + " bytes");

                if (read > 0)
                {
                    byte[] bytes = new byte[read];
                    Array.Copy(_buffer, read, bytes, 0, read);

                    System.Diagnostics.Debug.WriteLine("Encoded " + System.Text.Encoding.UTF8.GetString(bytes));
                    message = System.Text.Encoding.UTF8.GetString(XORMessage(bytes));
                    System.Diagnostics.Debug.WriteLine("Decoded " + System.Text.Encoding.UTF8.GetString(XORMessage(bytes)));

                    return true;
                }
            }

            return false;
        }

        private void Close()
        {
            if (_client != null)
            {
                _client.Dispose();
                _client = null;
            }

            Status = ConnectionStatus.Disconnected;
        }

        private void OnConnect(IAsyncResult result)
        {
            try
            {
                _client = result.AsyncState as System.Net.Sockets.TcpClient;
                _client.EndConnect(result);
                _ = _client.GetStream().BeginRead(_buffer, 0, _buffer.Length, new AsyncCallback(OnReadXorKey), _client);
                System.Diagnostics.Debug.WriteLine("Connected");
            }
            catch (Exception e)
            {
                Error = e.Message;
                Close();
            }
        }

        private void OnReadXorKey(IAsyncResult result)
        {
            try
            {
                _client = result.AsyncState as System.Net.Sockets.TcpClient;
                int bytes = _client.GetStream().EndRead(result);
                _xorKey = new byte[bytes];
                Array.Copy(_buffer, bytes, _xorKey, 0, bytes);
                System.Diagnostics.Debug.WriteLine("Received XOR key");

                // TODO: Initiate a login request
                //string loginCmd = string.Format(System.Globalization.CultureInfo.InvariantCulture, LOGIN_CMD, QuoteString(_password));
                //_ = _client.GetStream().BeginWrite(Encoding.UTF8.GetBytes(loginCmd), 0, loginCmd.Length, new AsyncCallback(OnLogin), _client);
            }
            catch (Exception e)
            {
                Error = e.Message;
                Close();
            }
        }

        /*private void OnLogin(IAsyncResult result)
        {
            try
            {
                _client = result.AsyncState as System.Net.Sockets.TcpClient;
                _client.GetStream().EndWrite(result);

                Status = ConnectionStatus.Connected;
            }
            catch (Exception e)
            {
                Error = e.Message;
                Close();
            }
        }*/

        private byte[] XORMessage(byte[] message)
        {
            for (int index = 0; index < message.Length; ++index)
            {
                message[index] ^= _xorKey[index % _xorKey.Length];
            }

            return message;
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
    }
}
