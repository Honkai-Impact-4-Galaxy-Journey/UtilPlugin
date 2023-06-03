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
            if (player.IsDead)
            {
                response = "你已经死了，想啥呢？";
            }
            else
            {
                player.Kill("防卡死指令");
                response = "Success!";
            }
            return true;
        }

    }
}
