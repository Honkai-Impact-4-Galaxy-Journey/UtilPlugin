using PlayerRoles;
using System;
using Exiled.API.Features;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandSystem;

namespace UtilPlugin
{
    public class ScpSwap
    {
        public static Dictionary<string, List<string>> swapids = new Dictionary<string, List<string>>();
        public static bool TrySwap(string origin, string target)
        {
            if (!swapids.ContainsKey(target))
            {
                swapids[target] = new List<string> { origin };
                goto Send;
            }
            if (swapids[origin].Contains(target))
            {
                RoleTypeId roleTypeId = Player.Get(origin).Role.Type;
                Player.Get(origin).Role.Set(Player.Get(target).Role.Type);
                Player.Get(target).Role.Set(roleTypeId);
                return true;
            }
            swapids[target].Add(origin);
        Send:
            SendMessage(target, origin);
            return false;
        }
        public static void SendMessage(string target, string origin)
        {
            Player.Get(target).Broadcast(5, $"<size=24>「SCP交换」{Player.Get(origin).Role.Name}请求与你交换，输入.scp {Player.Get(origin).Role.Name}完成交换</size>", shouldClearPrevious: true);
        }
    }
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Scp : ICommand
    {
        public string Command => "scp";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "scp交换";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player origin = Player.Get((sender as CommandSender).SenderId);
            if (!origin.IsScp)
            {
                response = "你不是SCP，想啥呢？";
                return false;
            }
            if (UtilPlugin.Roundtime.ElapsedMilliseconds / 1000 > 120)
            {
                response = "当前禁止角色交换";
                return false;
            }
            if (arguments.Count == 0)
            {
                response = "————当前回合scp列表————\n";
                foreach (Player player in Player.List)
                {
                    if (player.IsScp)
                    {
                        response += $"{player.Role.Name}\n";
                    }
                }
                response += "————————————————";
                return true;
            }
            Player target = Player.List.ToList().Find(p => p.Role.Name == arguments.At(0));
            bool flag = false;
            if (target != null)
            {
                flag = ScpSwap.TrySwap(origin.UserId, target.UserId);
                response = flag ? "正在交换" : "请求已发送";
            }
            else response = "不存在此scp";
            return flag;

        }
    }
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ForceSwapScp : ICommand, IUsageProvider
    {
        public string[] Usage => new string[] { "originname", "targetname"};

        public string Command => "forceswapscp";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string origin = arguments.At(0), target = arguments.At(1);
            RoleTypeId roleTypeId = Player.Get(origin).Role.Type;
            Player.Get(origin).Role.Set(Player.Get(target).Role.Type);
            Player.Get(target).Role.Set(roleTypeId);
            response = "Done!";
            return true;
        }
    }
}
