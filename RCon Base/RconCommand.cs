using System.Collections.Generic;
using System.Xml.Linq;

namespace RconClient
{
  public class RconCommand
  {
    private string m_messageTemplate;
    private List<RconCommandParameter> m_parameters = new List<RconCommandParameter>();

    public string m_name { get; private set; }

    public RconCommand(XElement commandNode)
    {
      this.m_name = (string) commandNode.Attribute((XName) "name");
      this.m_messageTemplate = (string) commandNode.Attribute((XName) "messagetemplate");
      foreach (XElement element in commandNode.Elements((XName) "Parameter"))
        this.m_parameters.Add(new RconCommandParameter(element));
    }

    public void StartExecuting(ServerSession serverSession)
    {
      List<string> stringList = new List<string>();
      if (this.m_parameters.Count > 0)
      {
        ParameterDialog parameterDialog = new ParameterDialog(this.m_parameters, serverSession);
        bool? nullable = parameterDialog.ShowDialog();
        bool flag = false;
        if ((nullable.GetValueOrDefault() == flag ? (nullable.HasValue ? 1 : 0) : 0) != 0)
          return;
        foreach (RconCommandParameter parameter in this.m_parameters)
        {
          string s = parameterDialog.ParameterToUserInput[parameter].Text;
          if (parameter.Quoted)
            s = RconCommand.QuoteString(s);
          stringList.Add(s);
        }
      }
      if (!serverSession.SendMessage(string.Format(this.m_messageTemplate, (object[]) stringList.ToArray())))
        return;
      string receivedMessage = "";
      serverSession.ReceiveMessage(out receivedMessage);
    }

    public static string QuoteString(string s)
    {
      string str = "\"";
      for (int index = 0; index < s.Length; ++index)
        str = s[index] != '"' ? (s[index] != '\\' ? str + s[index].ToString() : str + "\\\\") : str + "\\\"";
      return str + "\"";
    }
  }
}
