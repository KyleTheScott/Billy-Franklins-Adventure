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
    private List<Ghost> ghosts = new List<Ghost>();
    private bool isLowered;
    [SerializeField]
    private float lowerWallSpirteAlpha = 0f;
    private float currentWallSpirteAlpha;
    [SerializeField]
    private float raiseWallSpirteAlpha = 1f;
    [SerializeField]
    private float dissipationRate = 0.01f;
    [SerializeField]
    private bool RaiseOnStart = false;
    public bool IsLowered => isLowered;
    [Header("FMOD Settings")]
    [FMODUnity.EventRef][SerializeField]
    private string ghostwallEventPath;
    [SerializeField]
    private float ghostWallSoundVolume;
    private FMOD.Studio.EventInstance ghostWallSoundEvent;
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
        foreach (Ghost ghost in ghosts)
        {
            ghost.SetDissipationRate(dissipationRate);
        }

        ghostWallSoundEvent = FMODUnity.RuntimeManager.CreateInstance(ghostwallEventPath);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(ghostWallSoundEvent, transform, GetComponent<Rigidbody>());
        ghostWallSoundEvent.start();
        ghostWallSoundEvent.setVolume(ghostWallSoundVolume);

        ghostWallspriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, lowerWallSpirteAlpha);
        currentWallSpirteAlpha = lowerWallSpirteAlpha;

        if (RaiseOnStart)
        {
            RaiseGhostWall();
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (ghostWallState)
        {
            case (GhostWallState.LOWERING):
                LoweringGhostWall();
                break;
            case (GhostWallState.RAISING):
                RaisingGhostWall();
                break;
        }
    }

    private void RaisingGhostWall()
    {
        currentWallSpirteAlpha -= dissipationRate;
        ghostWallspriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, currentWallSpirteAlpha);
        if (currentWallSpirteAlpha <= raiseWallSpirteAlpha)
        {
            ghostWallState = GhostWallState.RAISED;
        }
    }

    private void LoweringGhostWall()
    {
        currentWallSpirteAlpha += dissipationRate;
        ghostWallspriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, currentWallSpirteAlpha);
        if (currentWallSpirteAlpha >= lowerWallSpirteAlpha)
        {
            ghostWallState = GhostWallState.LOWERED;
            boxCollider.isTrigger = false;
        }
    }

    public void LowerGhostWall()
    {
        ghostWallSoundEvent.start();
        ghostWallSoundEvent.setVolume(ghostWallSoundVolume);
        foreach (Ghost ghost in ghosts)
        {
            ghost.SetGhostAppearing();
        }
        ghostWallState = GhostWallState.LOWERING;
    }

    public void RaiseGhostWall()
    {
        ghostWallSoundEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        boxCollider.isTrigger = true;
        foreach (Ghost ghost in ghosts)
        {
            ghost.SetGhostDissipation();
        }
        ghostWallState = GhostWallState.RAISING;
    }

    
}
