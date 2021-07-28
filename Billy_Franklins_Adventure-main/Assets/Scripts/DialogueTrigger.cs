using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public void TriggerDialogue()
    {
        Debug.Log("Dialogue");
        DialogueManager.instance.StartDialogue(dialogue);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                TriggerDialogue();
                player.isReading = true;
                player.PlayerControlsStatus(false);
                Destroy(gameObject);
            }
        }
    }
}
