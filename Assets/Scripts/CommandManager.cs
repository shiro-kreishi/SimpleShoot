using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Singletons
{
    public class CommandManager : NetworkSingleton<CommandManager>
    {
        [FormerlySerializedAs("Command1")] public List<ulong> command1;
        [FormerlySerializedAs("Command2")] public List<ulong> command2;

        [FormerlySerializedAs("TeleportPositionFromCommand1")] public List<Transform> teleportPositionFromCommand1;
        [FormerlySerializedAs("TeleportPositionFromCommand2")] public List<Transform> teleportPositionFromCommand2;

        private List<bool> _teleportCommand1 = new List<bool>(100);
        private List<bool> _teleportCommand2 = new List<bool>(100);
        
        private NetworkVariable<Vector3> _playerPosition = new NetworkVariable<Vector3>();
        // private NetworkVariable<ulong> _userId = new NetworkVariable<ulong>();
        // private NetworkVariable<byte> _userChooseCommand = new NetworkVariable<byte>();

        private NetworkVariable<Vector3> _pointPosition = new NetworkVariable<Vector3>();
        // private NetworkVariable<NetworkObject> _serverUser = new NetworkVariable<NetworkObject>();

        private void Start()
        {
            for (int i = 0; i < teleportPositionFromCommand1.Count; ++i)
                _teleportCommand1.Add(false);
            for (int i = 0; i < teleportPositionFromCommand2.Count; ++i)
                _teleportCommand2.Add(false);
        }

        public void SetUserToCommand(ulong userId, byte chooseCommand)
        {
           
            // _userId.Value = userId;
            // _userChooseCommand.Value = chooseCommand;
            SendUserIdToCommandServerRpc(userId, chooseCommand);
            // Player connected to server 
            Debug.Log("teleport..."); 
            TeleportUsersToPointsServerRpc(userId, chooseCommand); 
            // TeleportPlayerServerRpc(userId);
            if(IsServer)
                // transform.position = _playerPosition.Value;
                TestTeleportClientRpc(_pointPosition.Value);
            else
            {
                TeleportToPointServerRpc();
                //TeleportToPointClientRpc();
                transform.position = _playerPosition.Value;
                Debug.Log($"client transform: {_playerPosition.Value}");
                Debug.Log($"player position: {transform.position}");
            }
        }

        [ServerRpc(RequireOwnership = false)]
        void TeleportToPointServerRpc()
        {
            _playerPosition.Value = _pointPosition.Value;
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void TeleportPlayerServerRpc(ulong clientID)
        {
            foreach (var player in GameObject.FindObjectsOfType<NetworkObject>())
            {
                if (player.OwnerClientId == clientID)
                {
                    player.transform.position = _pointPosition.Value;
                }
            }
        }

        [ClientRpc]
        void TestTeleportClientRpc(Vector3 point)
        {
            Debug.Log("clientrpc");
            transform.position = point;
        }

        [ServerRpc(RequireOwnership = false)]
        void TeleportUsersToPointsServerRpc(ulong id, byte chooseCommand)
        {
            var sizeTeleportPositionFromCommand = chooseCommand == 1 ? teleportPositionFromCommand1.Count :
                chooseCommand == 2 ? teleportPositionFromCommand2.Count : teleportPositionFromCommand1.Count;
            Debug.Log($"sizeTeleportPositionFromCommand: {sizeTeleportPositionFromCommand}");
            // _serverUser.Value = NetworkManager.Singleton.ConnectedClients[id].PlayerObject;
            var playerObj = NetworkManager.Singleton.ConnectedClients[id].PlayerObject;
            Debug.Log(playerObj.NetworkObjectId);
            var sizeCommand = chooseCommand==1 ? command1.Count :
                chooseCommand==2 ? command2.Count : command1.Count;
            Debug.Log($"sizeCommand: {sizeCommand}");
            var teleport = chooseCommand == 1 ? _teleportCommand1 :
                chooseCommand == 2 ? _teleportCommand2 : _teleportCommand1;
            Debug.Log($"teleportBoolArr: {teleport.Count}");
            for (int i = 0; i < teleport.Count; i++)
            {
                if (!teleport[i])
                {
                    teleport[i] = !teleport[i];
                    _playerPosition.Value = playerObj.transform.position;
                    var position = chooseCommand == 1 ? teleportPositionFromCommand1 :
                        chooseCommand == 2 ? teleportPositionFromCommand2 : teleportPositionFromCommand1;
                    _pointPosition.Value = position[i].position;
                    //TeleportToPointServerRpc();
                    // TeleportPlayerServerRpc(id);
                    TestTeleportClientRpc(position[i].position);
                    // playerObj.gameObject.transform.position = position[i].position;
                    Debug.Log($"position: {position[i].position}");
                    Debug.Log($"player position: {playerObj.gameObject.transform.position}");
                    break;
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        void SendUserIdToCommandServerRpc(ulong userId, byte chooseCommand)
        {
            if (chooseCommand == 1)
            {
                command1.Add(userId);
            }
            else if (chooseCommand == 2)
            {
                command2.Add(userId);
            }
        }
    }
}

