using System;
using Unity.Netcode;
using UnityEngine;

namespace Core.Singletons
{
    public class PlayersManager : NetworkSingleton<PlayersManager>
    {
        private NetworkVariable<int> playersInGame = new NetworkVariable<int>();

        public int PlayersInGame
        {
            get
            {
                return playersInGame.Value;
            }
        }

        private void Start()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
            {
                if (IsServer)
                {
                    Debug.Log($"{id} just connected...");
                    playersInGame.Value++;
                }
            };
            NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
            {
                if (IsServer)
                {
                    Debug.Log($"{id} just disconnected...");
                    playersInGame.Value--;
                }
            };
        }
    }
}