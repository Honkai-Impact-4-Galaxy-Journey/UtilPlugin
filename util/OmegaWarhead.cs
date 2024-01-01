//Copyright (C) Silver Wolf 2023,All Rights Reserved.
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
        public static CoroutineHandle ForceEnd;
        public static AudioPlayerBase playerBase;
        public static void StopOmega()
        {
            if (OmegaActivated)
            {
                foreach (CoroutineHandle coroutineHandle in coroutines) Timing.KillCoroutines(coroutineHandle);
                playerBase.Stoptrack(true);
                Cassie.Message("pitch_0.9 Omega Warhead detonation stopped");
                foreach (Room room in Room.List) room.ResetColor();
                OmegaActivated = false;
            }
        }
        public static IEnumerator<float> ForceEndRound()
        {
            if (OmegaActivated) { yield break; }
            yield return Timing.WaitForSeconds(3);
            Warhead.Stop();
            foreach (Room room in Room.List)
            {
                room.Color = Color.cyan;
            }
            Warhead.IsLocked = true;
            OmegaActivated = true;
            playerBase = Music.PlayMusic("Omega", "Omega核弹警报", 70);
            yield return Timing.WaitForSeconds(184);
            foreach (Player player in Player.List)
            {
                player.Kill("在Omega核弹中蒸发(强制终局)");
            }
            foreach (Room room in Room.List)
            {
                room.Color = Color.blue;
            }
        } 
        public static void ActivateOmega()
        {
            return;
            if (OmegaActivated)
            {
                return;
            }
            Warhead.Stop();
            Warhead.IsLocked = true;
            PluginAPI.Core.Server.SendBroadcast("<b><color=red>OMEGA核弹已启用.</color></b>\n请搭乘撤离直升机或前往地下掩体逃生.", 20);
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
                    switch (door.Type)
                    {
                        case Exiled.API.Enums.DoorType.CheckpointEzHczA:
                        case Exiled.API.Enums.DoorType.CheckpointEzHczB:
                        case Exiled.API.Enums.DoorType.CheckpointLczA:
                        case Exiled.API.Enums.DoorType.CheckpointLczB:
                        case Exiled.API.Enums.DoorType.GateA:
                        case Exiled.API.Enums.DoorType.GateB:
                            door.IsOpen = true;
                            door.Lock(120, Exiled.API.Enums.DoorLockType.Warhead);
                            break;
                    }
                }
                PluginAPI.Core.Server.SendBroadcast("Door opened", 3, Broadcast.BroadcastFlags.AdminChat);
            }));
            coroutines.Add(Timing.CallDelayed(165, () => {
                PluginAPI.Core.Server.SendBroadcast("撤离直升机将在12秒后到达", 12);
                RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.Selection, SpawnableTeamType.NineTailedFox);
            }));
            coroutines.Add(Timing.CallDelayed(177, () =>
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
                    if (Vector3.Distance(player.Position, Helikopterpos) <= 12)
                    {
                        player.Broadcast(4, "You escaped in the helicopter.");
                        Helikopter.Add(player);
                        player.EnableEffect(Exiled.API.Enums.EffectType.Ensnared, 12);
                        player.EnableEffect(Exiled.API.Enums.EffectType.Flashed, 12);
                    }
                }
                PluginAPI.Core.Server.SendBroadcast($"{Helikopter.Count} player fly and {Shelted.Count} player shelted", 3, Broadcast.BroadcastFlags.AdminChat);
            }));
            coroutines.Add(Timing.CallDelayed(184, () =>
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
                    player.Teleport(new Vector3(29.7f, 992.2f, -25.3f));
                }
                foreach (Room room in Room.List)
                {
                    room.Color = Color.blue;
                }
            }));
        }
    }
}
