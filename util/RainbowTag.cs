using Exiled.API.Features;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilPlugin
{
    public class RainbowTag
    {
        public static Dictionary<Player, CoroutineHandle> keyValuePairs = new Dictionary<Player, CoroutineHandle>();
        public static string[] Colors = new string[] { "red", "pink", "pumpkin", "yellow", "light_green", "aqua", "cyan", "silver" };
        public static IEnumerator<float> Register(Player player)
        {
            while (true)
            {
                foreach (string s in Colors)
                {
                    player.RankColor = s;
                    yield return Timing.WaitForSeconds(0.3f);
                }
                yield return Timing.WaitForSeconds(0.3f);
            }
        }
    }
}
