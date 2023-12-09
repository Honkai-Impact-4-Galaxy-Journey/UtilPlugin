//Copyright 2023 Silver Wolf,All Rights Reserved.
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Permissions.Extensions;
using MEC;
using Mirror.LiteNetLib4Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilPlugin
{
    public class ReserveSlot
    {
        public static int Remain => UtilPlugin.Instance.Config.Slots - Player.List.Where(p => Database.badges.Find(badge => badge.userid == p.UserId)?.reverseslot == "yes").Count();
        public static bool Check(string id)
        {
            if(Database.badges.Find(badge => badge.userid == id)?.reverseslot == "yes")
            {
                return true;
            }
            return false;
        }
        public static void Register(bool isEnabled)
        {
            if(isEnabled)
            {
                Exiled.Events.Handlers.Player.PreAuthenticating += OnJoining;
            }
            else
            {
                Exiled.Events.Handlers.Player.PreAuthenticating -= OnJoining;
            }
        }
        public static void OnJoining(PreAuthenticatingEventArgs ev)
        {
            Log.Debug($"Player {ev.UserId} joining, now remain {Server.MaxPlayerCount - LiteNetLib4MirrorCore.Host.ConnectedPeersCount}");
            if (Server.MaxPlayerCount - LiteNetLib4MirrorCore.Host.ConnectedPeersCount <= Remain)
            {
                Log.Debug($"Checking reservesolt of {ev.UserId}");
                if (Check(ev.UserId))
                {
                    Log.Debug($"Player {ev.UserId} has a reserveslot");
                    return;
                }
                Log.Debug($"Player {ev.UserId} has no reserveslot, Rejecting...");
                ev.Reject(UtilPlugin.Instance.Config.ReserveSlotKickReason, true);
            }
        }
    }
}
