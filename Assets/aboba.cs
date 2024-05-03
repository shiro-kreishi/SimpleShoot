using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aboba : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if ( other.gameObject.GetComponent<PlayerMovement>())
                other.gameObject.GetComponent<PlayerMovement>().test123();
        }
    }
}
