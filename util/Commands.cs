//Copyright 2023 Silver Wolf,All Rights Reserved.
using CommandSystem;
using Exiled.API.Features;
using Google.Protobuf.WellKnownTypes;
using InventorySystem;
using InventorySystem.Disarming;
using MEC;
using SCPSLAudioApi.AudioCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilPlugin;
using Utils.Networking;
using VoiceChat;

namespace CommandSystem
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Killme : ICommand
    {
        public string[] Aliases => Array.Empty<string>();

        public string Description => "防卡死指令";

        string ICommand.Command => "killme";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get((sender as CommandSender).SenderId);
            if (player.IsDead)
            {
                response = "你已经死了，想啥呢？";
            }
            else
            {
                Vector3 pos = player.Position;
                player.Teleport(new Vector3(40, 1014, -32.6f));
                player.ReferenceHub.inventory.ServerDropEverything();
                player.Teleport(pos);
                if (arguments.Count == 0)
                {
                    player.Kill(Exiled.API.Enums.DamageType.Falldown);
                }
                else
                {
                    player.Kill(arguments.At(0));
                }
                response = "Success!";
            }
            return true;
        }

    }
    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Setbadge : ICommand, IUsageProvider
    {
        public string[] Usage => new string[] { "newbadge", "newcolor", "id" };

        public string Command => "setbadge";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "change badge";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0)
            {
                response = "at least 1 arguments";
                return false;
            }
            Player player = Player.Get((sender as CommandSender).SenderId);
            player.RankName = arguments.At(0);
            if (arguments.Count == 2)
            {
                if (arguments.At(1) == "Rainbow")
                {
                    RainbowTag.RegisterPlayer(player);
                }
                else
                {
                    player.RankColor = arguments.At(1);
                }
            }
            response = "Done!";
            return true;
        }
    }
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ActivateSystemWarhead : ICommand
    {
        public string Command => "activatesystemwarhead";

        public string[] Aliases => new string[] {"asw"};

        public string Description => "启动系统核";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Timing.KillCoroutines(SystemWarhead._systemwarheadwaiter);
            SystemWarhead.Detonate();
            response = "Done!";
            return true;
        }
    }
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class StopCleanup : ICommand
    {
        public string Command => "stopautoclean";

        public string[] Aliases => new string[] {"sc"};

        public string Description => "停止自动清理";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Timing.KillCoroutines(UtilPlugin.EventHandler._cleanupcoroutine);
            if (arguments.Count != 1 || arguments.At(0) != "false")
            {
                PluginAPI.Core.Server.SendBroadcast($"管理员 {Player.Get((sender as CommandSender).SenderId).Nickname} 关闭了本局的自动清理", 10);
            }
            response = "Done!";
            return true;
        }
    }
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class StopSystemWarhead : ICommand
    {
        public string Command => "stopsystemwarhead";

        public string[] Aliases => new string[] {"stopautonuke"};

        public string Description => "停止系统核";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Timing.KillCoroutines(UtilPlugin.SystemWarhead._systemwarheadwaiter);
            if (arguments.Count != 1 || arguments.At(0) != "false")
            {
                PluginAPI.Core.Server.SendBroadcast($"管理员 {Player.Get((sender as CommandSender).SenderId).Nickname} 关闭了本局的系统核弹", 10);
            }
            response = "Done!";
            return true;
        }
    }
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(ClientCommandHandler))]
    public class UtilInfo : ICommand
    {
        public string Command => "utilplugininfo";

        public string[] Aliases => new string[] { "utilinfo", "uinfo" };

        public string Description => "UtilPlugin插件相关";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Copyright (C) 2023 Silver Wolf,All Rights Reserved.\n仓库地址: https://github.com/dargoncat/UtilPlugin";
            return true;
        }
    }
    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class StartRainbow : ICommand
    {
        public string Command => "startrainbow";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "startrainbow";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            UtilPlugin.RainbowTag.RegisterPlayer(Player.Get((sender as CommandSender).SenderId));
            response = "Done!";
            return true;
        }
    }
    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class StopRainbow : ICommand
    {
        public string Command => "stoprainbow";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "stoprainbow";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if(UtilPlugin.RainbowTag.UnRegisterPlayer(Player.Get((sender as CommandSender).SenderId)))
            {
                response = "Done!";
                return true;
            }
            if (arguments.At(0) == "true")
            {
                UtilPlugin.EventHandler.BypassMaxHealth = true;
            }
            else
            {
                UtilPlugin.EventHandler.BypassMaxHealth = false;
            }
            response = "no rainbow";
            return false;
        }
    }
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SetScale : ICommand, IUsageProvider
    {
        public string Command => "setscale";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "";

        public string[] Usage => new string[] { "id", "big" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(int.Parse(arguments.At(0)));
            float f = float.Parse(arguments.At(1));
            player.Scale = new UnityEngine.Vector3(f, f, f);
            response = "Done!";
            return true;
        }
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Music : ICommand, IUsageProvider
    {
        public string Command => "startmusic";

        public string[] Aliases => Array.Empty<string>();

        public string Description => " ";

        public string[] Usage => new string[] { "name" , "username", "volume" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0)
            {
                response = "";
                return false;
            }
            UtilPlugin.Music.PlayMusic(arguments.At(0), arguments.At(1), int.Parse(arguments.At(2)));
            response = "Done!";
            return true;
        }
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Uaow : ICommand
    {
        public string Command => "uaow";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            OmegaWarhead.ActivateOmega();
            response = "Done!";
            return true;
        }
    }
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Usow : ICommand
    {
        public string Command => "usow";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            OmegaWarhead.StopOmega();
            response = "Done!";
            return true;
        }
    }
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Stopmusic : ICommand
    {
        public string Command => "stopmusic";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            WarppedAudio audio = UtilPlugin.Music.audios.Find(au => string.Equals(au.Verfiy, arguments.At(0), StringComparison.CurrentCultureIgnoreCase));
            audio?.Player.Stoptrack(true);
            response = "Done!";
            return true;
        }
    }
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ChannelManager : ICommand
    {
        public string Command => "channelmanager";

        public string[] Aliases => new string[] { "cm" };

        public string Description => "";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(arguments.At(0));
            VoiceChatChannel channel;
            switch (arguments.At(1))
            {
                case "Intercom":
                case "intercom":
                    channel = VoiceChatChannel.Intercom; break;
                case "Scpchat":
                case "ScpChat":
                case "scpchat":
                    channel = VoiceChatChannel.ScpChat; break;
                case "Radio":
                case "radio":
                    channel = VoiceChatChannel.Radio; break;
                case "EndScreen":
                case "Endscreen":
                case "endscreen":
                case "RoundSummary":
                    channel = VoiceChatChannel.RoundSummary; break;
                default:
                    channel = VoiceChatChannel.Proximity; break;
            }
            if (bool.Parse(arguments.At(2)))
            {
                UtilPlugin.Music.audios.Find(w => w.Verfiy == arguments.At(0)).Player.BroadcastChannel = channel;
            }
            else
            {
                player.VoiceChannel = channel;
            }
            response = "Done!";
            return true;
        }
    }
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class StartEndingRound : ICommand
    {
        public string Command => "startendround";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "force end round in 180s";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            OmegaWarhead.ForceEnd = Timing.RunCoroutine(OmegaWarhead.ForceEndRound());
            response = "Done!";
            return true;
        }
    }
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class StopEndingRound : ICommand
    {
        public string Command => "stopendround";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "stop end round";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Timing.KillCoroutines(OmegaWarhead.ForceEnd);
            OmegaWarhead.playerBase.Stoptrack(true);
            OmegaWarhead.OmegaActivated = false;
            foreach (Room room in Room.List) room.ResetColor();
            response = "Done!";
            return true;
        }
    }
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Tps : ICommand
    {
        public string Command => "tps";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "show server tps";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = $"{Server.Tps}";
            return true;
        }
    }
    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ReloadBadge : ICommand
    {
        public string Command => "reloadbadge";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0) BadgeDatabase.Update();
            else BadgeDatabase.Update(arguments.At(0));
            response = "Done!";
            return true;
        }
    }
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Callkick : ICommand
    {
        public string Command => "callkick";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "投票踢人，用法：.callkick [id]";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0)
            {
                response = "";
                foreach (Player player in Player.List)
                {
                    response += $"{player.DisplayNickname}({player.Id})\n";
                }
                return false;
            }
            else
            {
                Player origin = Player.Get((sender as CommandSender).SenderId);
                Player target = Player.Get(int.Parse(arguments.At(0)));
                if (target == null)
                {
                    response = "不存在该玩家";
                    return false;
                }
                if (Check())
                {
                    response = "当前服务器内有管理员！请联系管理员";
                    return false;
                }
                if (!Voting.Canvote)
                {
                    response = "当前不可发起投票";
                    return false;
                }
                Voting.Canvote = false;
                Voting.voting = true;
                Voting.AcceptPlayer.Add(origin.UserId);
                Voting.votingcoroutine = Timing.RunCoroutine(Voting.SendBroadcast(new VotingEvent
                {
                    VotingDes = $"踢出{target.DisplayNickname}",
                    AcceptBroadcast = $"正在踢出{target.DisplayNickname}",
                    Action = () => { target.Ban(3, "你已被投票踢出"); },
                    OnVotingEnded = () => { return (double)Voting.AcceptPlayer.Count / Server.PlayerCount >= 0.7; }
                }, origin));
                response = "Done!";
                return true;
            }
        }
        public static bool Check()
        {
            foreach (Player player in Player.List)
            {
                if (player.Group != null && (player.Group.Permissions & (ulong)(PlayerPermissions.KickingAndShortTermBanning | PlayerPermissions.BanningUpToDay | PlayerPermissions.LongTermBanning)) != 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
