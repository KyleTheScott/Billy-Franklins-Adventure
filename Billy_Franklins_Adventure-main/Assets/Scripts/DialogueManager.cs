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
    public float letterRate = 10.0f;

    [Header("FMOD Settings")]
    [SerializeField]
    [FMODUnity.EventRef]
    private string pageFlipEventRef;
    [SerializeField]
    [FMODUnity.EventRef]
    private string writingEventRef;
    private FMOD.Studio.EventInstance writingSoundEvent;
    [SerializeField]
    private float pageFlipVolume = 0.8f;
    [SerializeField]
    private float writingVolume = 0.8f;


    private Queue<string> sentences = new Queue<string>();
    private string currentSentence;
    // Start is called before the first frame update

    public void Start()
    {
        writingSoundEvent = FMODUnity.RuntimeManager.CreateInstance(writingEventRef);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(writingSoundEvent, transform, GetComponent<Rigidbody>());
        writingSoundEvent.setVolume(writingVolume);
    }

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
        FMODUnity.RuntimeManager.PlayOneShot(pageFlipEventRef, pageFlipVolume);
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
        writingSoundEvent.start();
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(1.0f / letterRate);
        }
        writingSoundEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
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
