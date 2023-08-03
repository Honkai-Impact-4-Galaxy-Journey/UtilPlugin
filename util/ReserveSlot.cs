//Copyright 2023 Silver Wolf,All Rights Reserved.
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Permissions.Extensions;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilPlugin
{
    public class ReserveSlot
    {
        public static int Remain = UtilPlugin.Instance.Config.Slots;
        public static CoroutineHandle _autorefresh;
        public static void Register(bool reg)
        {
            if (reg)
            {
                Exiled.Events.Handlers.Player.Joined += Joining;
                Exiled.Events.Handlers.Player.Left += Disconnecting;
                Exiled.Events.Handlers.Server.RestartingRound += RoundRestart;
            }
            else
            {
                Exiled.Events.Handlers.Player.Joined -= Joining;
                Exiled.Events.Handlers.Player.Left -= Disconnecting;
                Exiled.Events.Handlers.Server.RestartingRound -= RoundRestart;
            }
        }

        public static void RoundRestart()
        {
            Timing.KillCoroutines(_autorefresh);
            Remain = UtilPlugin.Instance.Config.Slots;
            _autorefresh = Timing.RunCoroutine(Autorefresher(60));
        }

        private static IEnumerator<float> Autorefresher(int time)
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(time);
                Remain = UtilPlugin.Instance.Config.Slots;
                foreach (Player player in Player.List)
                {
                    if (ReserveSlot.CheckPermission(player))
                    {
                        ReserveSlot.Remain--;
                    }
                }
            }
        }

        public static void Disconnecting(LeftEventArgs ev)
        {
            if (CheckPermission(ev.Player))
            {
                Remain++;
            }
        }

        public static void Joining(JoinedEventArgs ev)
        {
            Timing.RunCoroutine(Checkid(ev.Player));
        }

        public static bool CheckPermission(Player player)
        {
            if (!(player.Group == null) && (player.Group.Permissions & (ulong)PlayerPermissions.AFKImmunity) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static IEnumerator<float> Checkid(Player player)
        {
            yield return Timing.WaitForSeconds(10);
            if (Server.PlayerCount > Server.MaxPlayerCount - Remain)
            {
                if (!CheckPermission(player))
                {
                    player.Disconnect(UtilPlugin.Instance.Config.ReserveSlotKickReason);
                    yield break;
                }
                else
                {
                    Remain--;
                    yield break;
                }
            }
            if (CheckPermission(player))
            {
                Remain--;
            }

        }
    }
}
namespace CommandSystem
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Refresh : ICommand
    {
        public string Command => "refresh";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "刷新预留位剩余";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            UtilPlugin.ReserveSlot.RoundRestart();
            foreach (Player player in Player.List)
            {
                if (UtilPlugin.ReserveSlot.CheckPermission(player))
                {
                    UtilPlugin.ReserveSlot.Remain--;
                }
            }
            response = $"Remain:{UtilPlugin.ReserveSlot.Remain}";
            return true;
        }
    }
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Fetch : ICommand
    {
        public string Command => "FetchRemain";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "查询预留位剩余";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = $"Remain:{UtilPlugin.ReserveSlot.Remain},Total:{Server.PlayerCount}";
            return true;
        }
    }
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class CheckPermission : ICommand
    {
        public string Command => "setremain";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count != 1)
            {
                response = "need 1 argument!Usage:setremain <newremain>";
                return false;
            }
            UtilPlugin.ReserveSlot.Remain = int.Parse(arguments.At(0));
            response = "Done!";
            return true;
        }
    }
    
}