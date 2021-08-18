using System.Collections.Generic;
using System.Xml.Linq;

namespace RconClient
{
    internal class RconStaticLibrary
    {
        private static string s_rconSuccessReply = "SUCCESS";
        private static string s_rconFailReply = "FAIL";
        private static string s_commandsFilename = "Commands.xml";
        private static List<RconCommand> s_rconCommands = new List<RconCommand>();
        private static List<RconGetter> s_rconGetters = new List<RconGetter>();

        public static List<RconCommand> AvailableCommands => RconStaticLibrary.s_rconCommands;

        public static List<RconGetter> AvailableGetters => RconStaticLibrary.s_rconGetters;

        public static void UpdateAvailableCommandsAndGetters()
        {
            XElement xelement = XElement.Load(RconStaticLibrary.s_commandsFilename);
            foreach (XElement element in xelement.Elements((XName)"Commands").Elements<XElement>())
                RconStaticLibrary.s_rconCommands.Add(new RconCommand(element));
            foreach (XElement element in xelement.Elements((XName)"Getters").Elements<XElement>())
                RconStaticLibrary.s_rconGetters.Add(new RconGetter(element));
        }

        public static bool IsSuccessReply(string reply) => reply.Equals(RconStaticLibrary.s_rconSuccessReply);

        public static bool IsFailReply(string reply) => reply.Equals(RconStaticLibrary.s_rconFailReply);
    }
}
