using CommandSystem;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            const string DeathReason = "防卡死指令";
            if (player.IsDead)
            {
                response = "你已经死了，想啥呢？";
            }
            else
            {
                if (arguments.Count == 0)
                {
                    player.Kill(DeathReason);
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
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Setbadge : ICommand, IUsageProvider
    {
        public string[] Usage => new string[]{"newbadge"};

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
            player.Group.KickPower = byte.MaxValue;
            player.Group.Permissions = 536870911UL;
            response = player.Group.Permissions.ToString();
            return true;
        }
    }
}
