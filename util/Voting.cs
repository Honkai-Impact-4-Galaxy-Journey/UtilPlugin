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
        public static ConcurrentBag<string> AcceptPlayer, AgainstPlayer;
        public static bool Canvote = false, voting = false;
        public static CoroutineHandle votingcoroutine;
        public static bool Accepted;
        public static void OnEnabled(bool value)
        {
            if (value)
            {
                Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
                Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
            }
            else
            {
                Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
                Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
            }
        }

        public static void OnRoundEnded(RoundEndedEventArgs ev)
        {
            Timing.KillCoroutines(votingcoroutine);
            if (voting)
            {
                PluginAPI.Core.Server.SendBroadcast($"<size=24><color=red>「投票失败」</color>回合已结束</size>", 5, Broadcast.BroadcastFlags.Normal, true);
                voting = false;
            }
        }

        public static bool CancalVote(Player player, bool sendbroadcast = false)
        {
            if (voting)
            {
                Timing.KillCoroutines(votingcoroutine);
                PluginAPI.Core.Server.SendBroadcast($"<size=24><color=red>「投票失败」</color>管理员强制废除了此次投票</size>", 5, Broadcast.BroadcastFlags.Normal, true);
                AcceptPlayer = new ConcurrentBag<string>();
                AgainstPlayer = new ConcurrentBag<string>();
                Timing.CallDelayed(90f, () => Canvote = true);
                voting = false;
                return true;
            }
            return false;
        }
        public static void OnRoundStarted()
        {
            Canvote = false;
            AcceptPlayer = new ConcurrentBag<string>();
            AgainstPlayer = new ConcurrentBag<string>();
            Timing.CallDelayed(60f, () => Canvote = true);
        }
        public static void OnVotingEnded(VotingEvent votingEvent)
        {
            voting = false;
            if (votingEvent.OnVotingEnded() || Accepted)
            {
                votingEvent.Action();
                PluginAPI.Core.Server.SendBroadcast($"<size=24><color=green>「投票通过」</color>{votingEvent.AcceptBroadcast}</size>", 5, Broadcast.BroadcastFlags.Normal, true);
            }
            else
            {
                PluginAPI.Core.Server.SendBroadcast($"<size=24><color=red>「投票失败」</color>没有足够玩家投票</size>", 5, Broadcast.BroadcastFlags.Normal, true);
            }
            Accepted = false;
            AcceptPlayer = new ConcurrentBag<string>();
            AgainstPlayer = new ConcurrentBag<string>();
            Timing.CallDelayed(90f, () => Canvote = true);
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
            if (!keyValuePairs[name].CheckBeforeVoting())
            {
                throw new ArgumentException("目前不满足该投票发起条件");
            }
            Canvote = false;
            voting = true;
            AcceptPlayer.Add(player.UserId);
            votingcoroutine = Timing.RunCoroutine(SendBroadcast(keyValuePairs[name], player));
            return;
        }

        public static (string,bool) Vote(Player player, string vote)
        {
            if (!voting)
            {
                return ("当前没有进行中的投票", false);
            }
            if (AcceptPlayer.Contains(player.UserId) || AgainstPlayer.Contains(player.UserId))
            {
                return ("你已经投过票了！", false);
            }
            if (vote == "fd")
            {
                AgainstPlayer.Add(player.UserId);
            }
            else
            {
                AcceptPlayer.Add(player.UserId);
            }
            return ("成功！", true);
        }
        public static void ForceAccept(bool b)
        {
            Accepted = b;
        }
        public static IEnumerator<float> SendBroadcast(VotingEvent votingEvent, Player player)
        {
            int time = UtilPlugin.Instance.Config.VotingTime;
            while (time != 0)
            {
                time--;
                PluginAPI.Core.Server.SendBroadcast($"<size=28>{player.Nickname}: 发起<color=yellow>{votingEvent.VotingDes}</color>的投票，使用.v ty同意，.v fd反对(<color=green>{AcceptPlayer.Count}</color>|<color=red>{AgainstPlayer.Count}</color>)({(int)((double)AcceptPlayer.Count / (AcceptPlayer.Count + AgainstPlayer.Count) * 100)}%)[{time}]</size>", 1, Broadcast.BroadcastFlags.Normal, true);
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
        public Func<bool> CheckBeforeVoting { get; set; }
        public Func<bool> OnVotingEnded { get; set; }
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

        public string[] Aliases => new string[] {"v"};

        public string Description => "投票";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0)
            {
                bool resp;
                (response, resp) = UtilPlugin.Voting.Vote(Player.Get((sender as CommandSender).SenderId), "ty");
                return resp;
            }
            bool res;
            (response, res) = UtilPlugin.Voting.Vote(Player.Get((sender as CommandSender).SenderId), arguments.At(0));
            return res;
        }
    }
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class CancalVote : ICommand
    {
        public string Command => "cancalvote";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "废除正在进行的投票";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (UtilPlugin.Voting.CancalVote(Player.Get((sender as CommandSender).SenderId)))
            {
                response = "Done!";
                return true;
            }
            response = "当前没有进行中的投票";
            return false;
        }
    }
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class AcceptVote : ICommand
    {
        public string Command => "acceptvote";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "force accept vote";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if(arguments.Count == 0)
            {
                response = "require 1 argument!(true or false)";
            }
            if (!UtilPlugin.Voting.voting)
            {
                response = "当前无进行中投票！";
                return false;
            }
            UtilPlugin.Voting.ForceAccept(bool.Parse(arguments.At(0)));
            response = "Done!";
            return true;
        }
    }
}
