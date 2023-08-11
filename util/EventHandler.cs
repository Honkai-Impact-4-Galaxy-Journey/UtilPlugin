//Copyright 2023 Silver Wolf,All Rights Reserved.
using Exiled.Events.EventArgs.Server;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MEC;
using Exiled.API.Features;
using Exiled.Events.Commands.Reload;
using InventorySystem.Items.Pickups;
using InventorySystem.Items;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Scp914;

namespace UtilPlugin
{
    public class EventHandler
    {
        private static CoroutineHandle _cleanupcoroutine;
        public static void Register(bool value)
        {
            if (value)
            {
                Exiled.Events.Handlers.Server.RoundStarted += Cleanup;
                Exiled.Events.Handlers.Server.RestartingRound += Stopcleanup;
                Exiled.Events.Handlers.Scp914.ChangingKnobSetting += Show914;
            }
            else
            {
                Exiled.Events.Handlers.Server.RoundStarted -= Cleanup;
                Exiled.Events.Handlers.Server.RestartingRound -= Stopcleanup;
                Exiled.Events.Handlers.Scp914.ChangingKnobSetting -= Show914;
            }
        }

        public static void Show914(ChangingKnobSettingEventArgs ev)
        {
            if (ev.KnobSetting == Scp914.Scp914KnobSetting.Rough)
            {
                ServerConsole.AddLog($"[Warning]{ev.Player.Nickname}({ev.Player.UserId})changes the 914 mode to {ev.KnobSetting}");
                return;
            }
            ServerConsole.AddLog($"{ev.Player.Nickname}({ev.Player.UserId})changes the 914 mode to {ev.KnobSetting}");
        }
        static bool Flag;
        private static void Stopcleanup()
        {
            Timing.KillCoroutines(_cleanupcoroutine);
        }
        public static void Cleanup()
        {
            Flag = true;
            float delay = UtilPlugin.Instance.Config.Cleanuptime;
            _cleanupcoroutine = Timing.RunCoroutine(cleanupwaiter(delay));
        }
        public static bool IsSCPitem(ItemType type)
        {
            return type == ItemType.SCP330 || type == ItemType.SCP500 || type == ItemType.SCP268 || type == ItemType.SCP2176 || type == ItemType.SCP207 || type == ItemType.SCP1853 || type == ItemType.SCP1576 || type == ItemType.SCP018;
        }
        public static IEnumerator<float> cleanupwaiter(float delay)
        {
            while(Flag)
            {
                yield return Timing.WaitForSeconds(delay-30);
                PluginAPI.Core.Server.SendBroadcast("The server will clean up after 30 seconds", 10);
                yield return Timing.WaitForSeconds(30);
                foreach(var a in UnityEngine.Object.FindObjectsOfType<ItemPickupBase>())
                {
                    Pickup pickup = Pickup.Get(a);
                    if (!(IsSCPitem(pickup.Type) || pickup.Type==ItemType.GrenadeFlash || pickup.Type == ItemType.Jailbird || pickup.Type==ItemType.GrenadeHE || pickup.Type==ItemType.MicroHID || pickup.Type==ItemType.KeycardO5 || pickup.Type==ItemType.KeycardFacilityManager || pickup.Type==ItemType.ParticleDisruptor))
                    {
                        pickup.Destroy();
                    }
                }
                foreach(var a in UnityEngine.Object.FindObjectsOfType<BasicRagdoll>())
                {
                    Ragdoll.Get(a).Destroy();
                }
                PluginAPI.Core.Server.SendBroadcast($"The cleanup is complete, and the next cleanup will occur after {delay} seconds", 10);
            }
        }
    }
}
