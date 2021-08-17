using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public bool isFinalTrigger = false;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                player.dialogue = dialogue;
                if (isFinalTrigger)
                {
                    player.finalDialogue = true;
                }
                Destroy(gameObject);
            }
        }
    }
}
