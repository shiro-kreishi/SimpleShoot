using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Core.Singletons
{
    public class CommandManager : NetworkSingleton<CommandManager>
    {
        public List<ulong> Command1;
        public List<ulong> Command2;

        public List<Transform> TeleportPositionFromCommand1;
        public List<Transform> TeleportPositionFromCommand2;

        private List<bool> _teleportCommand1 = new List<bool>(100);
        private List<bool> _teleportCommand2 = new List<bool>(100);
        
        private NetworkVariable<Vector3> _playerPosition = new NetworkVariable<Vector3>();
        // private NetworkVariable<ulong> _userId = new NetworkVariable<ulong>();
        // private NetworkVariable<byte> _userChooseCommand = new NetworkVariable<byte>();

        private NetworkVariable<Vector3> _pointPosition = new NetworkVariable<Vector3>();
        // private NetworkVariable<NetworkObject> _serverUser = new NetworkVariable<NetworkObject>();

        private void Start()
        {
            for (int i = 0; i < TeleportPositionFromCommand1.Count; ++i)
                _teleportCommand1.Add(false);
            for (int i = 0; i < TeleportPositionFromCommand2.Count; ++i)
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
            if(IsServer)
                transform.position = _playerPosition.Value;
            else
            {
                TeleportToPointServerRpc();
                transform.position = _playerPosition.Value;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        void TeleportToPointServerRpc()
        {
            _playerPosition.Value = _pointPosition.Value;
        }

        [ServerRpc(RequireOwnership = false)]
        void TeleportUsersToPointsServerRpc(ulong id, byte chooseCommand)
        {
            var sizeTeleportPositionFromCommand = chooseCommand == 1 ? TeleportPositionFromCommand1.Count :
                chooseCommand == 2 ? TeleportPositionFromCommand2.Count : TeleportPositionFromCommand1.Count;
            Debug.Log($"sizeTeleportPositionFromCommand: {sizeTeleportPositionFromCommand}");
            // _serverUser.Value = NetworkManager.Singleton.ConnectedClients[id].PlayerObject;
            var playerObj = NetworkManager.Singleton.ConnectedClients[id].PlayerObject;
            Debug.Log(playerObj.NetworkObjectId);
            var sizeCommand = chooseCommand==1 ? Command1.Count :
                chooseCommand==2 ? Command2.Count : Command1.Count;
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
                    var position = chooseCommand == 1 ? TeleportPositionFromCommand1 :
                        chooseCommand == 2 ? TeleportPositionFromCommand2 : TeleportPositionFromCommand1;
                    _pointPosition.Value = position[i].position;
                    TeleportToPointServerRpc();
                    playerObj.gameObject.transform.position = position[i].position;
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
                Command1.Add(userId);
            }
            else if (chooseCommand == 2)
            {
                Command2.Add(userId);
            }
        }
    }
}

