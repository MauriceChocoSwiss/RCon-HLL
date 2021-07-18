using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace RconClient
{
  public class RconGetter
  {
    private string m_messageTemplate;

    public string Name { get; private set; }

    public string DisplayName { get; }

    public bool ShowInServerInfo => !string.IsNullOrEmpty(this.DisplayName);

    public bool IsArray { get; }

    public bool AutoRefresh { get; }

    public RconGetter(XElement commandNode)
    {
      this.Name = (string) commandNode.Attribute((XName) "name");
      this.DisplayName = (string) commandNode.Attribute((XName) "showinserverinfoas");
      this.IsArray = this.HasAttribute(commandNode, "isarray");
      this.AutoRefresh = this.HasAttribute(commandNode, "autorefresh");
      this.m_messageTemplate = (string) commandNode.Attribute((XName) "messagetemplate");
    }

    public RconGetter(
      string name,
      string displayName,
      bool isArray,
      bool autoRefresh,
      string messageTemplate)
    {
      this.Name = name;
      this.DisplayName = displayName;
      this.IsArray = isArray;
      this.AutoRefresh = autoRefresh;
      this.m_messageTemplate = messageTemplate;
    }

    public bool GetData(ServerSession serverSession, out string[] data)
    {
      data = new string[0];
      string receivedMessage = "";
      if (!serverSession.SendMessage(this.m_messageTemplate) || !serverSession.ReceiveMessage(out receivedMessage, isCommand: false))
        return false;
      if (this.IsArray)
      {
        string[] strArray = Regex.Split(receivedMessage, "\t");
        int count = int.Parse(strArray[0]);
        data = ((IEnumerable<string>) strArray).Skip<string>(1).Take<string>(count).ToArray<string>();
      }
      else
        data = new string[1]{ receivedMessage };
      return true;
    }

    private bool HasAttribute(XElement node, string attributename)
    {
      XAttribute xattribute = node.Attribute((XName) attributename);
      try
      {
        return xattribute != null && (bool) xattribute;
      }
      catch (Exception ex)
      {
        return false;
      }
    }
  }
}
