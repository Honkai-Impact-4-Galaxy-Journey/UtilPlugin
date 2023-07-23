using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Permissions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilPlugin
{
    public class ReserveSlot
    {
        public static int Remain = UtilPlugin.Instance.Config.SlotIds.Count;
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
            Remain = UtilPlugin.Instance.Config.SlotIds.Count;
        }

        public static void Disconnecting(LeftEventArgs ev)
        {
            if (UtilPlugin.Instance.Config.SlotIds.Contains(ev.Player.AuthenticationToken))
            {
                Remain++;
            }
        }

        public static void Joining(JoinedEventArgs ev)
        {
            if (Server.PlayerCount >= Server.MaxPlayerCount - Remain)
            {
                if (UtilPlugin.Instance.Config.SlotIds.Contains(ev.Player.AuthenticationToken))
                {
                    Remain--;
                    return;
                }
                else
                {
                    ev.Player.Disconnect("服务器已满人");
                    return;
                }
            }
            if (UtilPlugin.Instance.Config.SlotIds.Contains(ev.Player.AuthenticationToken))
            {
                Remain--;
            }
        }
    }
}
namespace CommandSystem
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Test : ICommand
    {
        public string Command => "test";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = UtilPlugin.ReserveSlot.Remain.ToString() + " " + (Server.MaxPlayerCount - UtilPlugin.ReserveSlot.Remain).ToString();
            return true;
        }
    }
}