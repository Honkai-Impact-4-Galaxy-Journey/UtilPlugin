//Copyright (C) Silver Wolf 2023,All Rights Reserved.
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
        public static void OnRoundRestart()
        {
            foreach (var item in keyValuePairs)
            {
                Timing.KillCoroutines(item.Value);
            }
            keyValuePairs = new Dictionary<Player, CoroutineHandle>();
        }
        public static string[] Colors = new string[]
            {
                "pink",
                "red",
                "brown",
                "silver",
                "light_green",
                "crimson",
                "cyan",
                "aqua",
                "deep_pink",
                "tomato",
                "yellow",
                "magenta",
                "blue_green",
                "orange",
                "lime",
                "green",
                "emerald",
                "carmine",
                "nickel",
                "mint",
                "army_green",
                "pumpkin"
            };
        public static IEnumerator<float> StartRainbow(Player player)
        {
            while (true)
            {
                foreach (string s in Colors)
                {
                    player.RankColor = s;
                    yield return Timing.WaitForSeconds(0.5f);
                }
                yield return Timing.WaitForSeconds(0.3f);
            }
        }
        public static bool RegisterPlayer(Player player)
        {
            if (keyValuePairs.ContainsKey(player))return false;
            keyValuePairs[player] = Timing.RunCoroutine(StartRainbow(player));
            return true;
        }
        public static bool UnRegisterPlayer(Player player)
        {
            if (keyValuePairs.ContainsKey(player))
            {
                Timing.KillCoroutines(keyValuePairs[player]);
                keyValuePairs.Remove(player);
                return true;
            }
            return false;
        }
    }
}
