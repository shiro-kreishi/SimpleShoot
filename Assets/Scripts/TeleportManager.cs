using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Core.Singletons
{
    public class TeleportManager : NetworkSingleton<TeleportManager>
    {
        [SerializeField] private List<Transform> transforms;
        private NetworkVariable<Vector3> pointPosition = new NetworkVariable<Vector3>();
        private NetworkVariable<Vector3> playerPosition = new NetworkVariable<Vector3>();

        public void TeleportPlayer(Transform transform)
        {
            System.Random rnd = new System.Random();
            int rand = rnd.Next(transforms.Count);
            if (IsServer)
            {
               pointPosition.Value = transforms[rand].position;
               playerPosition.Value = transform.position;
               TeleportToPointServerRpc();
               
            }
            transform.position = playerPosition.Value;
        }

        [ServerRpc]
        void TeleportToPointServerRpc()
        {
            playerPosition.Value = pointPosition.Value;
        }
    }
}