using Exiled.API.Features;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilPlugin
{
    public enum BroadcastPriority : byte { Lowest = 1, Lower = 50, Normal = 100, Higher = 150, Highest = 200, eme = 255}
    public class BroadcastItem
    {
        public bool showtime = false;
        public int time;
        public string prefix, text;
        public Func<RoleTypeId, bool> Check;
        public byte priority;
        public List<string> targets;
        public static bool operator <(BroadcastItem lhs, BroadcastItem rhs) => lhs.priority < rhs.priority;
        public static bool operator >(BroadcastItem lhs, BroadcastItem rhs) => lhs.priority > rhs.priority;
        public override string ToString()
        {
           string result = $"<size=26>「{prefix}」:{text}";
           if (showtime) result += $"[{time}]</size>";
           else result += "</size>";
           return result;
        }
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
                    int remain = 5;
                    foreach (BroadcastItem item in globals)
                    {
                        if (remain > 0 && item.time > 0)
                        {
                            result += $"{item}\n";
                        }
                        remain--;
                    }
                    foreach (BroadcastItem item in normals)
                    {
                        if (item.time > 0 && remain > 0 && (item.targets.Contains(player.UserId) || (item.Check != null && item.Check(player.Role.Type))))
                        {
                            result += $"{item}\n";
                        }
                        remain--;
                    }
                    if (remain < 0)
                    {
                        result += $"还有{-remain}条消息没有显示";
                    }
                    player.Broadcast(2, result);
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
