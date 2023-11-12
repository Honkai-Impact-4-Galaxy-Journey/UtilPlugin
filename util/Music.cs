//Copyright (C) Silver Wolf 2023,All Rights Reserved.
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Mirror;
using SCPSLAudioApi.AudioCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UtilPlugin
{
    public class FakeConnection : NetworkConnectionToClient
    {
        public FakeConnection(int networkConnectionId) : base(networkConnectionId)
        {
        }

        public override string address => "localhost";

        public override void Send(ArraySegment<byte> segment, int channelId = 0)
        {
        }

        public override void Disconnect()
        {
        }

    }
    public class WarppedAudio
    {
        public AudioPlayerBase Player { get; set; }
        public string Music { get; set; }
        public string Username { get; set; }
        public string Verfiy { get; set; }
    }
    public class Music
    {
        public static List<WarppedAudio> audios = new List<WarppedAudio>();
        public static AudioPlayerBase PlayMusic(string musicname, string name, int vol)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(NetworkManager.singleton.playerPrefab);
            System.Random rand = new System.Random();
            int num = rand.Next(250, 301);
            FakeConnection connection = new FakeConnection(num);
            NetworkServer.AddPlayerForConnection(connection, gameObject);
            ReferenceHub referenceHub = gameObject.GetComponent<ReferenceHub>();
            try
            {
                referenceHub.nicknameSync.DisplayName = name;
                referenceHub.authManager.UserId = $"{musicname}-{num}@server";
            }
            catch
            {

            }
            AudioPlayerBase playerbase = AudioPlayerBase.Get(referenceHub);
            string text = Paths.Plugins + $"\\{musicname}.ogg";
            playerbase.Enqueue(text, -1);
            playerbase.LogDebug = false;
            playerbase.Volume = vol;
            foreach (Player player in Player.List)
            {
                playerbase.BroadcastTo.Add(player.Id);
            }
            playerbase.Loop = false;
            playerbase.Play(-1);
            referenceHub.roleManager.InitializeNewRole(PlayerRoles.RoleTypeId.Overwatch, PlayerRoles.RoleChangeReason.RemoteAdmin);
            audios.Add(new WarppedAudio { Player = playerbase, Music = musicname, Username = name, Verfiy = $"{musicname}-{num}@server"});
            return playerbase;
        }
        public static void OnRestartingRound()
        {
            foreach (WarppedAudio warppedAudio in audios)
            {
                warppedAudio.Player.Stoptrack(true);
            }
            audios.Clear();
        }
    }
}
