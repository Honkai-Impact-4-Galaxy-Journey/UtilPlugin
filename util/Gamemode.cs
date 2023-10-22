//Copyright (C) Silver Wolf 2023,All Rights Reserved.
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilPlugin
{
    public enum Gamemodes : byte { Normal, Other }
    public class Gamemode
    {
        public static Gamemodes NextMode = Gamemodes.Normal;

        public static void Register()
        {
            if (UtilPlugin.Instance.Config.ArrowVoteFunny)
            {
                Voting.Register(new VotingEvent { Action = () => NextMode = Gamemodes.Other, Votingpercent = 0.5, Name = "funnygame", AcceptBroadcast = "下回合游戏模式为<color=yellow>娱乐</color>", Description = "更改下回合游戏模式为娱乐[不能在无管理员时发起]", VotingDes = "更改下回合游戏模式为娱乐", CheckBeforeVoting = CheckCanChangeMode, OnVotingEnded = () => { return (double)Voting.AcceptPlayer.Count / (Voting.AcceptPlayer.Count + Voting.AgainstPlayer.Count) >= 0.85; } });
                Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            }
        }

        public static void OnRoundStarted()
        {
            switch (NextMode)
            {
                case Gamemodes.Normal:
                    PluginAPI.Core.Server.SendBroadcast("<size=26><color=red>「回合开始」</color>当前回合已经开始</size>", 5, Broadcast.BroadcastFlags.Normal, true);
                    break;
                case Gamemodes.Other:
                    PluginAPI.Core.Server.SendBroadcast("<size=26><color=red>「回合开始」</color>本局为娱乐模式</size>", 5, Broadcast.BroadcastFlags.Normal, true);
                    NextMode = Gamemodes.Normal;
                    break;
            }
        }

        public static bool CheckCanChangeMode()
        {
            foreach(Player player in Player.List)
            {
                if (player.Group != null && (player.Group.Permissions & (ulong)PlayerPermissions.ForceclassWithoutRestrictions) != 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
