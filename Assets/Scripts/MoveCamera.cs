using UnityEngine;
using Unity.Netcode;

public class MoveCamera : NetworkBehaviour
{
    public Transform cameraPosition;
    
    void Update()
    {
        if (!IsOwner) return;
        // Debug.Log(cameraPosition.position);
        if (IsLocalPlayer)
            transform.position = cameraPosition.position;
    }
}
