using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    #region Singleton
    public static DialogueManager instance;

    private void Awake()
    {
        //Make sure there is only one instance
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    #endregion

    public TextMeshProUGUI dialogueText;
    public bool isOpen;
    public bool isWriting;
    public Animator animator;
    public GameObject keysUI;
    public GameObject continueTextObject;
    public float letterTimer = 1f;

    private Queue<string> sentences = new Queue<string>();
    private string currentSentence;
    // Start is called before the first frame update

    
    public void StartDialogue (Dialogue dialogue)
    {
        animator.SetBool("IsOpen", true);
        isOpen = true;
        keysUI.SetActive(false);
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        AdvanceSentence();
    }

    public void AdvanceSentence()
    {
        StopAllCoroutines();
        if (isWriting)
        {
            dialogueText.text = currentSentence;
            isWriting = false;
            continueTextObject.SetActive(true);
        }

        else
        {
            if (sentences.Count == 0)
            {
                EndDialogue();
                return;
            }
            currentSentence = sentences.Dequeue();
            continueTextObject.SetActive(false);
            StartCoroutine(TypeSentence(currentSentence));
        }
        
    }


    IEnumerator TypeSentence (string sentence)
    {
        isWriting = true;
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(letterTimer);
        }

        continueTextObject.SetActive(true);
        isWriting = false;
    }

    private void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
        isOpen = false;
        keysUI.SetActive(true);
    }
}
