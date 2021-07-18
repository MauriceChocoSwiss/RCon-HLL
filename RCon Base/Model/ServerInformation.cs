namespace RconClient
{
  public struct ServerInformation
  {
    private RconGetter m_getter;
    private string[] m_data;

    public ServerInformation(RconGetter rconGetter, string[] dataArray)
    {
      this.m_getter = rconGetter;
      this.m_data = dataArray;
    }

    public string Name => this.m_getter.DisplayName;

    public string Data
    {
      get
      {
        if (!this.m_getter.IsArray || this.m_data.Length <= 1)
          return this.m_data[0];
        string str1 = "";
        foreach (string str2 in this.m_data)
          str1 = str1 + str2 + "\n";
        return str1;
      }
    }
  }
}
