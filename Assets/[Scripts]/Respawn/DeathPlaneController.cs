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
            var player = other.gameObject.GetComponent<PlayerBehaviour>();
            player.lives.LoseLife();
            player.health.ResetHealth();
            

            if (player.lives.value > 0)
            {
                Respawn(other.gameObject);

                FindObjectOfType<SoundManager>().PlaySoundFX(Channel.PLAYER_DEATH_FX, Sound.DEATH);
            }
            
        }
    }

    public void Respawn(GameObject go)
    {
        go.transform.position = currentCheckpoint.position;
    }
}
