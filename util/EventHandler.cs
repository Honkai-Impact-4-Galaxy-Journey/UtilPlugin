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

namespace util
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
        }
        static bool Flag;
        private static void Stopcleanup()
        {
            Flag = false;
        }

        public static void Cleanup()
        {
			Flag = true;
            float delay = UtilPlugin.Instance.Config.Cleanuptime;
            if (Flag)
            {
                _cleanupcoroutine = Timing.RunCoroutine(cleanupwaiter(delay));
            }
        }
        public static IEnumerator<float> cleanupwaiter(float delay)
        {
            while(Flag)
			{
				yield return Timing.WaitForSeconds(20);
				if (!Flag) {
					yield break;
				}
				PluginAPI.Core.Server.SendBroadcast("服务器将在60秒后清理掉落物和尸体", 10);
				yield return Timing.WaitForSeconds(15);
				Exiled.API.Features.Server.RunCommand("cleanup ragdolls");
				Exiled.API.Features.Server.RunCommand("cleanup items");
				PluginAPI.Core.Server.SendBroadcast($"清理完成，下次清理将在{delay}秒后进行", 10);
			}
        }
    }
}
