using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeathPlaneController : MonoBehaviour
{
    public Transform currentCheckpoint;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            Respawn(other.gameObject);
        }
    }

    public void Respawn(GameObject go)
    {
        go.transform.position = currentCheckpoint.position;
    }
}
