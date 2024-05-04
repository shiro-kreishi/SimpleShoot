using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TeleportTargetTest : NetworkBehaviour
{
    public Transform teleportTarget;

    [ServerRpc]
    public void TeleportPlayerServerRpc(ulong clientID)
    {
        foreach (var player in GameObject.FindObjectsOfType<NetworkObject>())
        {
            if (player.OwnerClientId == clientID)
            {
                player.transform.position = teleportTarget.position;
                Debug.Log($"ClientID: {clientID}");
                TeleportPlayerClientRpc(player.NetworkObjectId);
                break;
            }
        }
    }

    [ClientRpc]
    private void TeleportPlayerClientRpc(ulong networkObjectId)
    {
        // NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.GetLocalNetworkObject(networkObjectId);
        NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        if (networkObject != null)
        {
            networkObject.gameObject.transform.position = teleportTarget.position;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (IsOwner)
                TeleportPlayerServerRpc(other.gameObject.GetComponent<NetworkObject>().OwnerClientId);
        }
    }
}
