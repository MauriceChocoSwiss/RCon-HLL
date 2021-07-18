namespace RconClient
{
  public struct ServerConnectionDetails
  {
    public string ServerAddress { get; private set; }

    public int ServerPort { get; private set; }

    public string ServerPassword { get; private set; }

    public ServerConnectionDetails(string serverAddress, int serverPort, string serverPassword)
    {
      this.ServerAddress = serverAddress;
      this.ServerPort = serverPort;
      this.ServerPassword = serverPassword;
    }
  }
}
