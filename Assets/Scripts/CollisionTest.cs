using Core.Singletons;
using UnityEngine;

public class CollisionTest : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("col1"))
        {
            Debug.Log("Collision");
            TeleportManager.Instance.TeleportPlayer(transform);
        }
    }
}
