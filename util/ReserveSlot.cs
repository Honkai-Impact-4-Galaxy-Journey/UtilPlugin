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
            Remain = UtilPlugin.Instance.Config.Slots;
        }

        public static void Disconnecting(LeftEventArgs ev)
        {
            if (ev.Player.CheckPermission("AFKImmunity"))
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
            if ((player.Group.Permissions & 4194304UL)!=0)
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
            yield return Timing.WaitForSeconds(7);
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
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class CheckPermission : ICommand
    {
        public string Command => "checkpermission";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = UtilPlugin.ReserveSlot.CheckPermission(Player.Get((sender as CommandSender).SenderId)).ToString();
            return true;
        }
    }
}