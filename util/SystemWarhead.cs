//Copyright 2023 Silver Wolf,All Rights Reserved.
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace UtilPlugin
{
    public class SystemWarhead
    {
        public static CoroutineHandle _systemwarheadwaiter;
        public static void OnRoundStarted()
        {
            _systemwarheadwaiter = Timing.RunCoroutine(SystemWarheadwaiter(UtilPlugin.Instance.Config.SystemWarheadTime));
        }
        public static void Register()
        {
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
            if (UtilPlugin.Instance.Config.DetonateOnRoundEnded)
            {
                Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
            }
            else
            {
                Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
            }
            if (true)
            {
                Voting.Register(new VotingEvent { Action = () => Detonate(false), Name = "systemwarhead", Description = "启动系统核[55%]", VotingDes = "系统核弹", Votingpercent = 0.55, AcceptBroadcast = "系统核弹已经启动", CheckBeforeVoting = () => { return true; }, OnVotingEnded = () => { return ((double)Voting.AcceptPlayer.Count + (double)(Server.PlayerCount - Voting.AcceptPlayer.Count - Voting.AgainstPlayer.Count)*0.3) / Server.PlayerCount >= 0.55; } });
            }
        }

        public static void OnRoundEnded(RoundEndedEventArgs roundEndedEventArgs)
        {
            Timing.CallDelayed(5, () => { Warhead.Detonate(); });
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
        public static void Detonate(bool sendbroadcast = true)
        {
            if (Warhead.IsDetonated)
            {
                return;
            }
            Warhead.LeverStatus = true;
            Warhead.IsKeycardActivated = true;
            if (!Warhead.IsInProgress)
            {
                Warhead.Start();
            }
            Warhead.IsLocked = true;
            if (sendbroadcast)
            {
                PluginAPI.Core.Server.SendBroadcast(UtilPlugin.Instance.Config.SystemWarheadBroadcast, 10, Broadcast.BroadcastFlags.Normal, true);
            }
        }
    }
}
