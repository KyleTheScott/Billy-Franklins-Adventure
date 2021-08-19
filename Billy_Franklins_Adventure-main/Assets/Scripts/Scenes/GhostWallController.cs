using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWallController : MonoBehaviour
{
    [SerializeField]
    private Collider2D boxCollider;
    [SerializeField]
    private SpriteRenderer ghostWallspriteRenderer;
    [SerializeField]
    private Animator animator;
    private List<Ghost> ghosts = new List<Ghost>();
    private bool isLowered;
    [SerializeField]
    private bool RaiseOnStart = false;
    public bool IsLowered => isLowered;
    [Header("FMOD Settings")]
    [FMODUnity.EventRef][SerializeField]
    private string ghostwallEventPath;
    [SerializeField]
    private float ghostWallSoundVolume;
    private FMOD.Studio.EventInstance ghostWallSoundEvent;
    [FMODUnity.EventRef]
    [SerializeField]
    private string ghostwallDisappearEventPath;
    [SerializeField]
    private float ghostWallDisappearVolume;
    private GhostWallState ghostWallState = GhostWallState.LOWERED;
    public enum GhostWallState
    {
        LOWERED,
        LOWERING,
        RAISING,
        RAISED
    }

    // Start is called before the first frame update
    void Start()
    {
        ghosts.AddRange(GetComponentsInChildren<Ghost>());
        //foreach (Ghost ghost in ghosts)
        //{
        //    ghost.SetDissipationRate(dissipationRate);
        //}

        ghostWallSoundEvent = FMODUnity.RuntimeManager.CreateInstance(ghostwallEventPath);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(ghostWallSoundEvent, transform, GetComponent<Rigidbody>());
        ghostWallSoundEvent.start();
        ghostWallSoundEvent.setVolume(ghostWallSoundVolume);

        if (RaiseOnStart)
        {
            foreach (Ghost ghost in ghosts)
            {
                ghost.SetTransparency(0);
                ghost.gameObject.SetActive(false);
            }
            RaiseGhostWall();
        }
    }

    public void LowerGhostWall()
    {
        animator.SetTrigger("Appear");
        ghostWallspriteRenderer.enabled = true;
        ghostWallSoundEvent.start();
        ghostWallSoundEvent.setVolume(ghostWallSoundVolume);
        ghostWallState = GhostWallState.LOWERING;
    }

    public void MakeGhostsAppear()
    {
        foreach (Ghost ghost in ghosts)
        {
            ghost.gameObject.SetActive(true);
            ghost.SetGhostAppearing();
        }
    }

    public void LowerGhostWallEnd()
    {
        Debug.Log("LowerGhostWallEnd");
        boxCollider.isTrigger = false;
 
        ghostWallState = GhostWallState.LOWERED;
    }

    public void RaiseGhostWall()
    {
        FMODUnity.RuntimeManager.PlayOneShot(ghostwallDisappearEventPath, ghostWallDisappearVolume, transform.position);
        ghostWallSoundEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        boxCollider.isTrigger = true;
        foreach (Ghost ghost in ghosts)
        {
            ghost.SetGhostDissipation();
        }
        animator.SetTrigger("Disappear");
        ghostWallState = GhostWallState.RAISING;
    }

    public void RaiseGhostWallEnd()
    {
        foreach (Ghost ghost in ghosts)
        {
            ghost.gameObject.SetActive(false);
        }
        ghostWallspriteRenderer.enabled = false;
        ghostWallState = GhostWallState.RAISING;
    }



}
