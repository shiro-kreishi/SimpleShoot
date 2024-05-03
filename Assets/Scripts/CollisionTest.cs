using Core.Singletons;
using Unity.Netcode;
using UnityEngine;

public class CollisionTest : NetworkBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("col1"))
        {
            Debug.Log("Collision");
            TeleportManager.Instance.TeleportPlayer(transform);
        }
        if (other.gameObject.CompareTag("command1"))
        {
            Debug.Log($"Red: {OwnerClientId}");
            CommandManager.Instance.SetUserToCommand(OwnerClientId, 1);
        }
        if (other.gameObject.CompareTag("command2"))
        {
            Debug.Log($"Blue: {OwnerClientId}");
            CommandManager.Instance.SetUserToCommand(OwnerClientId, 2);
        }
    }
}
