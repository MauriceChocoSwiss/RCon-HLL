using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCon_Plus
{
    internal class Request
    {
        private readonly Connection _connection;
        private readonly string _message;

        public Request(Connection connection, string message)
        {
            _connection = connection;
            _message = message;
        }

        public void Execute()
        {
            System.Diagnostics.Debug.WriteLine("request message: " + _message);
            _connection.Write(_message);
            string response;

            System.Threading.Thread.Sleep(1000);

            if (_connection.Read(out response))
            {
                System.Diagnostics.Debug.WriteLine("response: " + response);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("no response");
            }
        }
    }
}
