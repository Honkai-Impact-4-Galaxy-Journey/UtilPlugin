using Exiled.API.Features;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilPlugin
{
    public class SystemWarhead
    {
        public static CoroutineHandle _systemwarheadwaiter;
        public static void OnRoundStarted()
        {
            _systemwarheadwaiter = Timing.RunCoroutine(SystemWarheadwaiter(UtilPlugin.Instance.Config.SysteamWarheadTime));
        }
        public static void OnRoundFinished()
        {
            Timing.KillCoroutines(_systemwarheadwaiter);
        }
        public static IEnumerator<float> SystemWarheadwaiter(float time)
        {
            yield return Timing.WaitForSeconds(time);
            Detonate();
        }
        public static void Detonate(bool sendmessage = true)
        {
            Warhead.LeverStatus = true;
            Warhead.IsKeycardActivated = true;
            if (!Warhead.IsInProgress)
            {
                Warhead.Start();
            }
            Warhead.IsLocked = true;
            if (sendmessage)
            {
                PluginAPI.Core.Server.SendBroadcast(UtilPlugin.Instance.Config.SystemWarheadBroadcast, 10, Broadcast.BroadcastFlags.Normal, true);
            }
        }
    }
}
