using Exiled.API.Features;
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
    public class Music
    {
        public static void PlayMusic(string name)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(NetworkManager.singleton.playerPrefab);
            System.Random rand = new System.Random();
            int num = rand.Next(250, 301);
            FakeConnection connection = new FakeConnection(num);
            ReferenceHub referenceHub = gameObject.GetComponent<ReferenceHub>();
            NetworkServer.AddPlayerForConnection(connection, gameObject);
            referenceHub.characterClassManager._privUserId = $"Music-{num}@Server";
            referenceHub.characterClassManager.InstanceMode = ClientInstanceMode.Unverified;
            try
            {
                referenceHub.nicknameSync.SetNick("HOYO-Mix");
            }
            catch
            {

            }
            AudioPlayerBase playerbase = AudioPlayerBase.Get(referenceHub);
            string text = Paths.Plugins + $"\\{name}.ogg";
            playerbase.Enqueue(text, -1);
            playerbase.LogDebug = false;
            playerbase.Volume = 70;
            foreach (Player player in Player.List)
            {
                playerbase.BroadcastTo.Add(player.Id);
            }
            playerbase.Loop = false;
            playerbase.Play(-1);
        }
    }
}
