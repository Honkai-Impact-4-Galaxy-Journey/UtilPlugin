using MEC;
using System;
using System.Collections.Generic;
using System.Threading;

namespace UtilPlugin
{
    public class Voting
    {
        public static Dictionary<string,VotingEvent> keyValuePairs = new Dictionary<string,VotingEvent>();
        public static bool Canvote = false, voting = false;
        public static void OnEnabled(bool value)
        {
            if (value)
            {
                Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            }
            else
            {
                Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            }
        }

        public static void OnRoundStarted()
        {
            Canvote = false;
            Timing.CallDelayed(60f, () => Canvote = true);
        }
        public static void OnVotingEnded()
        {
            Timing.CallDelayed(30f, () => Canvote = true);
        }
        public static void Callvote(string name)
        {
            bool found = false;
            foreach (KeyValuePair<string,VotingEvent> keyValuePair in keyValuePairs)
            {
                if (keyValuePair.Key == name)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                throw new ArgumentException("没有此投票");
            }
            
        }
        public static void Register(VotingEvent votingEvent)
        {
            keyValuePairs[votingEvent.Name] = votingEvent;
        }
        public static string Foreach()
        {
            string s = "";
            foreach(KeyValuePair<string,VotingEvent> keyValuePair in keyValuePairs)
            {
                s += $"{keyValuePair.Value.Description}:{keyValuePair.Key}\n";
            }
            return s;
        }
    }
    public class VotingEvent
    {
        public Action Action { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string VotingDes { get; set; }
    }
}
namespace CommandSystem
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Callvote : ICommand
    {
        public string Command => "callvote";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "发起投票";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0)
            {
                response = UtilPlugin.Voting.Foreach();
                return true;
            }
            if (!UtilPlugin.Voting.Canvote)
            {
                response = "当前不可发起投票";
                return false;
            }
            try
            {

            }
            catch (ArgumentException ex)
            {
                response = ex.Message;
                return false;
            }
        }
    }
}
