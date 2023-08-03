using System;
using System.Collections.Generic;
using System.Threading;

namespace UtilPlugin
{
    public class Voting
    {
        public static Dictionary<string,VotingEvent> keyValuePairs = new Dictionary<string,VotingEvent>();
        public static void Register(VotingEvent votingEvent)
        {

        }
    }
    public class VotingEvent
    {
        public static Action Action { get; set; }
        public static string Name { get; set; }
        public static string Description { get; set; }
    }
}
