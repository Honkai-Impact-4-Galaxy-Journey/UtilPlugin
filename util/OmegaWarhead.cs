using Exiled.API.Features;
using Exiled.API.Features.Doors;
using MEC;
using Respawning;
using SCPSLAudioApi.AudioCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UtilPlugin
{
    public class OmegaWarhead
    {
        public static List<Player> Helikopter = new List<Player>();
        public static List<Player> Shelted = new List<Player>();
        public static List<CoroutineHandle> coroutines = new List<CoroutineHandle>();
        public static bool OmegaActivated = false;
        public static AudioPlayerBase playerBase;
        public static void StopOmega()
        {
            if (OmegaActivated)
            {
                foreach (CoroutineHandle coroutineHandle in coroutines) Timing.KillCoroutines(coroutineHandle);
                playerBase.StopAllCoroutines();
                Cassie.Message("pitch_0.9 Omega Warhead detonation stopped");
                foreach (Room room in Room.List) room.ResetColor();
            }
        }
        public static void ActivateOmega()
        {
            OmegaActivated = true;
            Shelted.Clear();
            Helikopter.Clear();
            foreach (Room room in Room.List)
            {
                room.Color = Color.cyan;
            }
            playerBase = Music.PlayMusic("Omega", "Omega核弹警报", 70);
            coroutines.Clear();
            coroutines.Add(Timing.CallDelayed(150, () => { 
                foreach(Door door in Door.List)
                {
                    if(door.IsCheckpoint || door.IsGate)
                    {
                        door.IsOpen = true;
                        door.Lock(120, Exiled.API.Enums.DoorLockType.Warhead);
                    }
                }
            }));
            coroutines.Add(Timing.CallDelayed(161, () => {
            RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.Selection, SpawnableTeamType.NineTailedFox);
            }));
            coroutines.Add(Timing.CallDelayed(179, () =>
            {
                foreach (Player player in Player.List)
                {
                    if (player.CurrentRoom.RoomName == MapGeneration.RoomName.EzEvacShelter)
                    {
                        Shelted.Add(player);
                        player.EnableEffect(Exiled.API.Enums.EffectType.Ensnared, 5);
                        player.EnableEffect(Exiled.API.Enums.EffectType.Flashed, 5);
                    }
                    Vector3 Helikopterpos = new Vector3(126, 995, -44);
                    if (Vector3.Distance(player.Position, Helikopterpos) <= 10)
                    {
                        player.Broadcast(4, "You escaped in the helicopter.");
                        Helikopter.Add(player);
                        player.EnableEffect(Exiled.API.Enums.EffectType.Ensnared, 8);
                        player.EnableEffect(Exiled.API.Enums.EffectType.Flashed, 8);
                    }
                }
            }));
            coroutines.Add(Timing.CallDelayed(180, () =>
            {
                foreach(Player player in Player.List)
                {
                    if (!Helikopter.Contains(player) && !Shelted.Contains(player))
                    {
                        player.Kill("在Omega核弹中蒸发");
                    }
                }
                foreach(Player player in Shelted)
                {
                    player.Teleport(new Vector3(126, 998, -41));
                }
                foreach (Room room in Room.List)
                {
                    room.Color = Color.blue;
                }
            }));
        }
    }
}
