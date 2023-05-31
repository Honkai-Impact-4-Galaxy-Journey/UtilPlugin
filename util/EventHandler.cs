using Exiled.Events.EventArgs.Server;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MEC;

namespace util
{
    public static class EventHandler
    {
        public static void Register(bool value)
        {
            if (value)
            {
                Exiled.Events.Handlers.Server.RoundStarted += Cleanup;
                Exiled.Events.Handlers.Server.RoundEnded += Stopcleanup;
            }
            else
            {
                Exiled.Events.Handlers.Server.RoundStarted -= Cleanup;
                Exiled.Events.Handlers.Server.RoundEnded -= Stopcleanup;
            }
        }
        static bool Flag;
        private static void Stopcleanup(RoundEndedEventArgs ev)
        {
            Flag = false;
        }

        public static void Cleanup()
        {
            Flag = true;
            while (Flag)
            {
                Timing.WaitForSeconds(UtilPlugin.Instance.Config.Cleanuptime-10);
                Server.SendBroadcast("服务器将在60秒后清除掉落物", 10);
                Timing.WaitForSeconds(10);
                ServerConsole.EnterCommand("cleanup item");
            }
            if (UtilPlugin.Instance.Config.Debug)
            {
                Server.SendBroadcast("stop cleanup",5);
            }
        }
        
    }
}
