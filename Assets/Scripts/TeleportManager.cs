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
            // var rnd = Random.Range(0, transforms.Count);
            // if (IsServer)
            // {
            //    // pointPosition.Value = transforms[rnd].position;
            //    // playerPosition.Value = transform.position;
            //    TeleportToPointServerRpc();
            //    
            // }
            TeleportToPointServerRpc();
            transform.position = playerPosition.Value;
        }

        [ServerRpc]
        void TeleportToPointServerRpc()
        {
            var rnd = Random.Range(0, transforms.Count);
            playerPosition.Value = transforms[rnd].position;
            
            // playerPosition.Value = pointPosition.Value;
        }
    }
}