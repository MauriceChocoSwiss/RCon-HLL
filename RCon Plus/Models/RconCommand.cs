using System.Collections.Generic;
using System.Xml.Linq;

namespace RconClient
{
    public class RconCommand
    {

        public string Name { get; private set; }
        public string MessageTemplate;
        public List<RconCommandParameter> Parameters = new List<RconCommandParameter>();

        public RconCommand(XElement commandNode)
        {
            Name = (string)commandNode.Attribute((XName)"name");
            MessageTemplate = (string)commandNode.Attribute((XName)"messagetemplate");
            foreach (XElement element in commandNode.Elements((XName)"Parameter"))
                Parameters.Add(new RconCommandParameter(element));
        }
    }
}
