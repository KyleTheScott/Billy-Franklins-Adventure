using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerGFX : MonoBehaviour
{
    private SpriteRenderer playerSprite;
    private Animator playerAnimator;
    private Player player;
    private FMODStudioFootstepScript footstepScript;
    private Player.PlayerState currentPlayerState;
    private Shooting shooting;
    private Shooting.AimLineState currentAimLineState;
    [SerializeField] private bool isFacingRight = false; //Is character facing right side? for Character flip
    private bool settingFacingRight;

    [Header("FMOD Settings")]
    [FMODUnity.EventRef]
    [SerializeField]
    private string buckKickEventRef;

    // Start is called before the first frame update
    void Start()
    {
        playerSprite = gameObject.GetComponent<SpriteRenderer>();
        playerAnimator = gameObject.GetComponent<Animator>();
        player = FindObjectOfType<Player>();
        shooting = FindObjectOfType<Shooting>();
        footstepScript = FindObjectOfType<FMODStudioFootstepScript>();

        isFacingRight = true;

    }

    // Update is called once per frame
    void Update()
    {
        ChooseAnimation();
    }

    public bool GetFacingRight()
    {
        return isFacingRight;
    }

    public void SetFacingRight(bool state)
    {
        isFacingRight = state;
        settingFacingRight = true;
    }

    private void ChooseAnimation()
    {
        if (settingFacingRight)
        {
            settingFacingRight = false;
            if (isFacingRight)
            {
                playerSprite.flipX = true;
            }
            else
            {
                playerSprite.flipX = true;
            }
        }

        if (currentPlayerState != player.GetPlayerState())
        {
            switch (player.GetPlayerState())
            {
                case Player.PlayerState.IDLE:
                    playerAnimator.SetInteger("PlayerAnimState", 0);
                    
                    break;
                case Player.PlayerState.WALKING:
                    playerAnimator.SetInteger("PlayerAnimState", 1);
                    break;
                case Player.PlayerState.JUMP:
                    playerAnimator.SetInteger("PlayerAnimState", 2);
                    break;
                case Player.PlayerState.JUMPING:

                    break;
                case Player.PlayerState.JUMP_FALLING:
                    break;
                case Player.PlayerState.FALLING:
                    playerAnimator.SetInteger("PlayerAnimState", 4);
                    break;
                case Player.PlayerState.FALL_FIX:

                    break;
                case Player.PlayerState.MOVING_OBJECT:

                    break;
                case Player.PlayerState.MOVING_OBJECT_IDLE:

                    break;
                case Player.PlayerState.MOVING_OBJECT_LEFT:

                    break; 
                case Player.PlayerState.MOVING_OBJECT_STOPPED_LEFT:

                    break;
                case Player.PlayerState.MOVING_OBJECT_RIGHT:

                    break;
                case Player.PlayerState.MOVING_OBJECT_STOPPED_RIGHT:

                    break;
                case Player.PlayerState.DEATH:

                    break;
                case Player.PlayerState.KICK_BUCKET_START:
                    playerAnimator.SetInteger("PlayerAnimState", 5);
                   
                    break;
                case Player.PlayerState.KICK_BUCKET:
                    playerAnimator.SetInteger("PlayerAnimState", 1);

                    player.SetPlayerState(Player.PlayerState.WALKING);
                    break;
                case Player.PlayerState.INTERACT:

                    break;
                    
            }
        }

        if (currentAimLineState != shooting.GetAimLineState())
        {
            if (shooting.GetAimLineState() == Shooting.AimLineState.AIMING)
            {
                playerAnimator.SetInteger("PlayerAnimState", 3);
            }
            else
            {
                playerAnimator.SetInteger("PlayerAnimState", 0);
            }
        }

        currentAimLineState = shooting.GetAimLineState();
        currentPlayerState = player.GetPlayerState();
        if (player.GetPlayerState() == Player.PlayerState.KICK_BUCKET_START)
        {
            player.SetPlayerState(Player.PlayerState.KICK_BUCKET);
        }
        else if (player.GetPlayerState() == Player.PlayerState.KICK_BUCKET)
        {
            player.SetPlayerState(Player.PlayerState.WALKING);
        }
    }
    public void KickBucketOver()
    {
        player.InteractWithObject();
    }

    public void PlayFootStep()
    {
        footstepScript.PlayerFootsteep();
    }

    public void PlayBucketKickSound()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(buckKickEventRef, gameObject);
    }
}
