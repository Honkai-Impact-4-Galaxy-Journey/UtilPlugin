using Exiled.API.Features;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilPlugin
{
   public enum ModeList { Normal, Rush173 }
   public class Gamemode
    {
        public static void Register(bool reg)
        {
            if (reg)
            {
                Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            }
            else
            {
                Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            }
        }
        public static ModeList NextRoundMode;
        public static void OnRoundStarted()
        {
            //PluginAPI.Core.Server.SendBroadcast("「」");
            switch (NextRoundMode)
            {
                case ModeList.Rush173:
                    Timing.RunCoroutine(Rush173Waiter());
                    Exiled.Events.Handlers.Warhead.Detonated += Rush173End;
                    break;

            }
            NextRoundMode = ModeList.Normal;
        }
        public static IEnumerator<float> Rush173Waiter()
        {
            Round.IsLocked = true;
            yield return Timing.WaitForSeconds(5);
            foreach (Player player in Player.List)
            {
                player.Role.Set(PlayerRoles.RoleTypeId.Scp173, Exiled.API.Enums.SpawnReason.ForceClass, PlayerRoles.RoleSpawnFlags.All);
                player.Teleport(new UnityEngine.Vector3(150, 13, 112));
            }
            Door door = Door.GetClosest(new UnityEngine.Vector3(150, 13, 112), out float _f);
            SystemWarhead.Detonate(false);
            yield return Timing.WaitForSeconds(7);
            door.TryPryOpen();
        }
        public static void Rush173End()
        {
            int remain = 0;
            foreach (Player player in Player.List)
            {
                if(!player.IsDead)
                {
                    ++remain;
                }
            }
           foreach (Player player in Player.List)
            {
                player.ShowHint($"本局玩家存活{remain}/{Server.PlayerCount}");
            }
            Exiled.Events.Handlers.Warhead.Detonated -= Rush173End;
        }
    }
}
