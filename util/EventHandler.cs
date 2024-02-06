//Copyright 2023 Silver Wolf,All Rights Reserved.
using Exiled.Events.EventArgs.Server;
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
using Exiled.Events.EventArgs.Player;
using PlayerRoles.Ragdolls;
using Exiled.Events.EventArgs.Scp330;
using System.Diagnostics;
using Exiled.Events.Patches.Events.Scp330;
using InventorySystem.Items.Usables.Scp330;

namespace UtilPlugin
{
    public static class EventHandler
    {
        public static List<CoroutineHandle> SB = new List<CoroutineHandle>();
        public static CoroutineHandle _cleanupcoroutine;
        public static void Register(bool value)
        {
            Exiled.Events.Handlers.Server.RestartingRound += RainbowTag.OnRoundRestart;
            Exiled.Events.Handlers.Server.RestartingRound += () => { Timing.KillCoroutines(OmegaWarhead.ForceEnd); OmegaWarhead.OmegaActivated = false; };
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.Died += OnPlayerDied;
            Exiled.Events.Handlers.Server.RestartingRound += Music.OnRestartingRound;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundstart;
            Exiled.Events.Handlers.Server.RoundEnded += (RoundEndedEventArgs ev) => { foreach (var cor in SB) Timing.KillCoroutines(cor); } ;
            //Exiled.Events.Handlers.Player.Handcuffing += OnHandcuffing;
            Exiled.Events.Handlers.Scp330.InteractingScp330 += OnInteractingScp330;
            Exiled.Events.Handlers.Scp330.EatenScp330 += OnEatenScp330;
            //Exiled.Events.Handlers.Warhead.Detonated += () => OmegaWarhead.ForceEnd = (Timing.RunCoroutine(OmegaWarhead.ForceEndRound()));
            if (UtilPlugin.Instance.Config.MysqlEnabled)
            {
                Exiled.Events.Handlers.Server.RestartingRound += OnRoundRestart;
                Exiled.Events.Handlers.Player.Joined += OnPlayerJoined;
            }
            else
            {
                Exiled.Events.Handlers.Server.RestartingRound -= OnRoundRestart;
                Exiled.Events.Handlers.Player.Joined -= OnPlayerJoined;
            }
            if (value)
            {
                Exiled.Events.Handlers.Scp914.ChangingKnobSetting += Show914;
                Exiled.Events.Handlers.Scp914.Activating += OnActivate914;
            }
            else
            {
                Exiled.Events.Handlers.Scp914.ChangingKnobSetting -= Show914;
                Exiled.Events.Handlers.Scp914.Activating -= OnActivate914;
            }
            if (UtilPlugin.Instance.Config.EnableAutoCleanup)
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
        public static void OnEatenScp330(EatenScp330EventArgs ev)
        {
            switch (ev.Candy.Kind)
            {
                case CandyKindID.Purple:
                    ev.Player.EnableEffect(Exiled.API.Enums.EffectType.DamageReduction, 100, 15, true);
                    break;
                case CandyKindID.Blue:
                    ev.Player.AddAhp(20, decay:0, efficacy:1);
                    break;
                case CandyKindID.Yellow:
                    ev.Player.EnableEffect(Exiled.API.Enums.EffectType.MovementBoost, 30, 10, true);
                    break;
                case CandyKindID.Red:
                    ev.Player.Heal(20);
                    break;
                case CandyKindID.Green:
                    ev.Player.EnableEffect(Exiled.API.Enums.EffectType.Vitality, 60, true);
                    break;
            }
        }
        public static void OnInteractingScp330(InteractingScp330EventArgs ev)
        {
            if (ev.Candy == CandyKindID.Yellow && UtilPlugin.Instance.Config.PinkCandy && UnityEngine.Random.Range(1, 100) <= 80)
            {
                ev.Candy = CandyKindID.Pink;
            }
            if (ev.UsageCount <= UtilPlugin.Instance.Config.CandyCount - 1)
            {
                ev.ShouldSever = false;
            }
        }
        public static void OnRoundstart()
        {
            UtilPlugin.Roundtime = Stopwatch.StartNew();
        }
        public static Player player;
        public static bool BypassMaxHealth;
        public static void SetBadge(this Player player)
        {
            Badge badge = BadgeDatabase.GetBadge(player.UserId);
            if (badge == null || badge.text == "none")
            {
                Log.Info($"Player {player.Nickname}({player.UserId}) has no badge");
                return;
            }
            Log.Info($"Player {player.Nickname}({player.UserId}) has a badge {badge}");
            if (!string.Equals(badge.adminrank, "player", StringComparison.CurrentCultureIgnoreCase))
            {
                player.SetRank(Exiled.API.Extensions.UserGroupExtensions.GetValue(badge.adminrank).BadgeText, Exiled.API.Extensions.UserGroupExtensions.GetValue(badge.adminrank));
            }
            if (!string.Equals("none", badge.text, StringComparison.CurrentCultureIgnoreCase))
            {
                if (badge.cover || string.IsNullOrEmpty(player.RankName))
                {
                    player.RankName = badge.text;
                }
                else
                {
                    player.RankName = $"{player.RankName}*{badge.text}*";
                }
                if (string.Equals(badge.color, "rainbow", StringComparison.CurrentCultureIgnoreCase))
                {
                    RainbowTag.RegisterPlayer(player);
                }
                else
                {
                    player.RankColor = badge.color;
                }
            }
        }
        public static void OnPlayerJoined(JoinedEventArgs ev)
        {
            Timing.CallDelayed(10, ev.Player.SetBadge);
        }
        public static void OnRoundRestart()
        {
            BadgeDatabase.Update();
        }
        public static void OnPlayerDied(DiedEventArgs ev)
        {
            if (ev.Attacker != null && UtilPlugin.Instance.Config.HealHps != null && UtilPlugin.Instance.Config.HealHps.ContainsKey(ev.Attacker.Role))
            {
                if (ev.Attacker.Health + UtilPlugin.Instance.Config.HealHps[ev.Attacker.Role] >= ev.Attacker.MaxHealth && !BypassMaxHealth)
                {
                    ev.Attacker.Health = ev.Attacker.MaxHealth;
                }
                else
                {
                    ev.Attacker.Health += UtilPlugin.Instance.Config.HealHps[ev.Attacker.Role];
                }
            }
        }
        public static void OnSpawned(SpawnedEventArgs ev)
        {
            if (UtilPlugin.Instance.Config.HealthValues.ContainsKey(ev.Player.Role))
            {
                ev.Player.MaxHealth = UtilPlugin.Instance.Config.HealthValues[ev.Player.Role];
                ev.Player.Health = UtilPlugin.Instance.Config.HealthValues[ev.Player.Role];
            }
        }
        public static void OnActivate914(ActivatingEventArgs ev)
        {
            if (Scp914.Scp914Controller.Singleton.KnobSetting == Scp914.Scp914KnobSetting.Rough)
            {
                ServerConsole.AddLog($"[Warning]{ev.Player.Nickname}({ev.Player.UserId})activated 914 as {Scp914.Scp914Controller.Singleton.KnobSetting} mode", ConsoleColor.Yellow);
                return;
            }
            ServerConsole.AddLog($"{ev.Player.Nickname}({ev.Player.UserId})activated 914 as {Scp914.Scp914Controller.Singleton.KnobSetting} mode");
        }

        public static void Show914(ChangingKnobSettingEventArgs ev)
        {
            if (ev.KnobSetting == Scp914.Scp914KnobSetting.Rough)
            {
                ServerConsole.AddLog($"[Warning]{ev.Player.Nickname}({ev.Player.UserId})changes the 914 mode to {ev.KnobSetting}",ConsoleColor.Yellow);
                player = ev.Player;
                return;
            }
            ServerConsole.AddLog($"{ev.Player.Nickname}({ev.Player.UserId})changes the 914 mode to {ev.KnobSetting}");
        }
        
        static bool Flag;
        public static void Stopcleanup()
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
            return type == ItemType.SCP330 || type == ItemType.SCP500 || type == ItemType.SCP268 || type == ItemType.SCP244a || type==ItemType.SCP244b || type == ItemType.SCP2176 || type == ItemType.SCP207 || type == ItemType.SCP1853 || type == ItemType.SCP1576 || type == ItemType.SCP018;
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
                    if (!(IsSCPitem(pickup.Type) || pickup.Type==ItemType.GrenadeFlash || pickup.Type == ItemType.Jailbird || pickup.Type==ItemType.GrenadeHE || pickup.Type==ItemType.MicroHID || pickup.Type==ItemType.KeycardO5 || pickup.Type==ItemType.KeycardFacilityManager || pickup.Type==ItemType.ParticleDisruptor))
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
        public static IEnumerator<float> WDNMD(Player player)
        {
            long timenow = UtilPlugin.Roundtime.ElapsedMilliseconds / 1000;
            while (true)
            {
                player.EnableEffect(Exiled.API.Enums.EffectType.SeveredHands);
                yield return Timing.WaitForSeconds(0.1f);
                player.DisableEffect(Exiled.API.Enums.EffectType.SeveredHands);
                if (UtilPlugin.Roundtime.ElapsedMilliseconds / 1000 - timenow >= 30) break;
            }
            yield break;
        }
    }
}
