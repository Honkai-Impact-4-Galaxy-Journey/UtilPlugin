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
            if (UtilPlugin.Instance.Config.SlotIds.Contains(ev.Player.UserId))
            {
                Remain++;
            }
        }

        public static void Joining(JoinedEventArgs ev)
        {
            if (Server.PlayerCount >= Server.MaxPlayerCount - Remain)
            {
                Timing.RunCoroutine(Checkid(ev.Player));
            }
            if (UtilPlugin.Instance.Config.SlotIds.Contains(ev.Player.UserId))
            {
                Remain--;
            }
        }
        public static IEnumerator<float> Checkid(Player player)
        {
            yield return Timing.WaitForSeconds(7);
            if (!UtilPlugin.Instance.Config.SlotIds.Contains(player.UserId))
            {
                player.Disconnect(UtilPlugin.Instance.Config.ReserveSlotKickReason);
            }
            else
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