using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                player.dialogue = dialogue;
                Destroy(gameObject);
            }
        }
    }
}
