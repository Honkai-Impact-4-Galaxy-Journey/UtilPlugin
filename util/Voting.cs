using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using MEC;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace UtilPlugin
{
    public class Voting
    {
        public static Dictionary<string,VotingEvent> keyValuePairs = new Dictionary<string,VotingEvent>();
        public static ConcurrentBag<string> VotedPlayer;
        public static bool Canvote = false, voting = false;
        public static CoroutineHandle votingcoroutine;
        public static void OnEnabled(bool value)
        {
            if (value)
            {
                Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
                Exiled.Events.Handlers.Server.EndingRound += OnRoundEnded;
            }
            else
            {
                Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
                Exiled.Events.Handlers.Server.EndingRound += OnRoundEnded;
            }
        }

        public static void OnRoundEnded(EndingRoundEventArgs ev)
        {
            Timing.KillCoroutines(votingcoroutine);
            if (voting)
            {
                PluginAPI.Core.Server.SendBroadcast($"<color=red>「投票失败」</color>回合已结束", 5, Broadcast.BroadcastFlags.Normal, true);
                voting = false;
            }
        }

        public static void OnRoundStarted()
        {
            Canvote = false;
            VotedPlayer = new ConcurrentBag<string>();
            Timing.CallDelayed(60f, () => Canvote = true);
        }
        public static void OnVotingEnded(VotingEvent votingEvent)
        {
            if ((double)VotedPlayer.Count/(double)Server.PlayerCount >= votingEvent.Votingpercent)
            {
                votingEvent.Action();
                PluginAPI.Core.Server.SendBroadcast($"<color=green>「投票通过」</color>{votingEvent.AcceptBroadcast}", 5, Broadcast.BroadcastFlags.Normal, true);
            }
            else
            {
                PluginAPI.Core.Server.SendBroadcast($"<color=red>「投票失败」</color>没有足够玩家投票", 5, Broadcast.BroadcastFlags.Normal, true);
            }
            voting = false;
            VotedPlayer = new ConcurrentBag<string>();
            Timing.CallDelayed(30f, () => Canvote = true);
        }
        public static void Callvote(string name, Player player)
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
            Canvote = false;
            voting = true;
            VotedPlayer.Add(player.UserId);
            votingcoroutine = Timing.RunCoroutine(SendBroadcast(keyValuePairs[name], player));
            return;
        }

        public static (string,bool) Vote(Player player)
        {
            if (!voting)
            {
                return ("当前没有进行中的投票", false);
            }
            if (VotedPlayer.Contains(player.UserId))
            {
                return ("你已经投过票了！", false);
            }
            VotedPlayer.Add(player.UserId);
            return ("成功！", true);
        }

        public static IEnumerator<float> SendBroadcast(VotingEvent votingEvent, Player player)
        {
            int time = UtilPlugin.Instance.Config.VotingTime;
            while (time != 0)
            {
                time--;
                PluginAPI.Core.Server.SendBroadcast($"{player.Nickname}:发起<color=yellow>{votingEvent.VotingDes}</color>的投票,使用.vote来投票(<color=green>{VotedPlayer.Count}</color>/{Server.PlayerCount}({(int)((double)VotedPlayer.Count / Server.PlayerCount * 100)}%)[{time}])",1,Broadcast.BroadcastFlags.Normal,true);
                yield return Timing.WaitForSeconds(1.1f);
            }
            OnVotingEnded(votingEvent);
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
        public double Votingpercent { get; set; }
        public string AcceptBroadcast { get; set; }
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
                UtilPlugin.Voting.Callvote(arguments.At(0), Player.Get((sender as CommandSender).SenderId));
                response = "成功！已为你投了同意票";
                return true;
            }
            catch (ArgumentException ex)
            {
                response = ex.Message;
                return false;
            }
        }
    }
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Vote : ICommand
    {
        public string Command => "vote";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "投票";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            bool res;
            (response, res) = UtilPlugin.Voting.Vote(Player.Get((sender as CommandSender).SenderId));
            return res;
        }
    }
}
