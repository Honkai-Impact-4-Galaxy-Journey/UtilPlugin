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
            if (UtilPlugin.Instance.Config.SystemWarheadEnabled != SystemWarheadMode.none)
            {
                Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
                Exiled.Events.Handlers.Server.RestartingRound += OnRoundFinished;
            }
            else
            {
                Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
                Exiled.Events.Handlers.Server.RestartingRound -= OnRoundFinished;
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
                Voting.Register(new VotingEvent { Action = () => Detonate(false), Name = "sw", Description = "启动系统核[60%]", VotingDes = "系统核弹", Votingpercent = 0.55, AcceptBroadcast = "系统核弹已经启动", CheckBeforeVoting = () => { return UtilPlugin.Roundtime.ElapsedMilliseconds / 1000 > 720; }, OnVotingEnded = () => { return (double)Voting.AcceptPlayer.Count / (Voting.AcceptPlayer.Count + Voting.AgainstPlayer.Count) >= 0.6; } });
                //Voting.Register(new VotingEvent { Action = () => OmegaWarhead.ActivateOmega(), Name = "omega", Description = "启动Omega核弹", VotingDes = "启动Omega核弹", AcceptBroadcast = "Omega核弹已启动", CheckBeforeVoting = () => true, OnVotingEnded = () => { return (double)Voting.AcceptPlayer.Count / (Voting.AcceptPlayer.Count + Voting.AgainstPlayer.Count) >= 0.7; } });
            }
        }

        public static void OnRoundEnded(RoundEndedEventArgs roundEndedEventArgs)
        {
            Timing.CallDelayed(5, () => { Warhead.Detonate(); });
            OmegaWarhead.StopOmega();
        }

        public static void OnRoundFinished()
        {
            Timing.KillCoroutines(_systemwarheadwaiter);
        }
        public static IEnumerator<float> SystemWarheadwaiter(float time)
        {
            yield return Timing.WaitForSeconds(time);
            if (UtilPlugin.Instance.Config.SystemWarheadEnabled == SystemWarheadMode.Alpha)
            {
                Detonate();
            }
            else
            {
                OmegaWarhead.ActivateOmega();
            }
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
