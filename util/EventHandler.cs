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
            }
            else
            {
                Exiled.Events.Handlers.Server.RoundStarted -= Cleanup;
                Exiled.Events.Handlers.Server.RestartingRound -= Stopcleanup;
            }
            if (UtilPlugin.Instance.Config.SystemWarheadEnabled)
            {
                Exiled.Events.Handlers.Server.RoundStarted += SystemWarhead.OnRoundStarted;
                Exiled.Events.Handlers.Server.RestartingRound += SystemWarhead.OnRoundFinished;
            }
            else
            {
                Exiled.Events.Handlers.Server.RoundStarted -= SystemWarhead.OnRoundStarted;
                Exiled.Events.Handlers.Server.RestartingRound -= SystemWarhead.OnRoundFinished;
            }
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
            return type == ItemType.SCP330 || type == ItemType.SCP500 || type == ItemType.SCP268 || type == ItemType.SCP244b || type == ItemType.SCP244a || type == ItemType.SCP2176 || type == ItemType.SCP207 || type == ItemType.SCP1853 || type == ItemType.SCP1576 || type == ItemType.SCP018;
        }
        public static IEnumerator<float> cleanupwaiter(float delay)
        {
            while(Flag)
			{
				yield return Timing.WaitForSeconds(delay-30);
				PluginAPI.Core.Server.SendBroadcast("服务器将在30秒后清理掉落物和尸体", 10);
				yield return Timing.WaitForSeconds(30);
                foreach(var a in UnityEngine.Object.FindObjectsOfType<ItemPickupBase>())
                {
                    Pickup pickup = Pickup.Get(a);
                    if (!(IsSCPitem(pickup.Type) || pickup.Type==ItemType.GrenadeFlash || pickup.Type==ItemType.GrenadeHE || pickup.Type==ItemType.MicroHID || pickup.Type==ItemType.KeycardO5 || pickup.Type==ItemType.KeycardFacilityManager || pickup.Type==ItemType.ParticleDisruptor))
                    {
                        pickup.Destroy();
                    }
                }
                foreach(var a in UnityEngine.Object.FindObjectsOfType<BasicRagdoll>())
                {
                    Ragdoll.Get(a).Destroy();
                }
                PluginAPI.Core.Server.SendBroadcast($"清理完成，下次清理将在{delay}秒后进行", 10);
			}
        }
    }
}
