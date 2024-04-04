using Exiled.API.Features;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilPlugin
{
    public class BroadcastItem
    {
        public int time;
        public string prefix, text;
        public Func<Player, bool> Check;
        public int priority;
        public List<string> targets;
        public static bool operator <(BroadcastItem lhs, BroadcastItem rhs) => lhs.priority < rhs.priority;
        public static bool operator >(BroadcastItem lhs, BroadcastItem rhs) => lhs.priority > rhs.priority; 
    }
    public class BroadcastMain
    {
        public static List<BroadcastItem> globals;
        public static List<BroadcastItem> normals;
        public static void SendGlobalcast(BroadcastItem item)
        {
            globals.Add(item);
        }
        public static void SendNormalCast(BroadcastItem item)
        {
            normals.Add(item);
        }
        public static IEnumerator<float> Maincoroutine()
        {
            while (true)
            {
                globals.Sort();
                normals.Sort();
                foreach (Player player in Player.List)
                {
                    string result = "";
                    int remain = 3;
                    foreach (BroadcastItem item in globals)
                    {
                        if (remain > 0) result += $"「{item.prefix}」:{item.text}[{item.time}]\n";
                        remain--;
                    }
                    foreach (BroadcastItem item in normals)
                    {
                        if ((item.targets.Contains(player.UserId)) || (item.Check != null && item.Check(player)))
                        {
                            result += $"「{item.prefix}」:{item.text}[{item.time}]\n";
                        }
                        remain--;
                    }
                    if (remain < 0)
                    {
                        result += $"还有{-remain}条消息没有显示";
                    }
                    player.ShowHint(result, 2);
                }
                for (int i = 0; i < globals.Count; i++)
                {
                    globals[i].time--;
                    if (globals[i].time < 0) globals.Remove(globals[i]);
                }
                for (int i = 0; i < normals.Count; i++)
                {
                    normals[i].time--;
                    if (normals[i].time < 0) normals.Remove(normals[i]);
                }
                yield return Timing.WaitForSeconds(1);
            }
        }
    }
}
