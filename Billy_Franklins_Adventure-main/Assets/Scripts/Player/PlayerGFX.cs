using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerGFX : MonoBehaviour
{
    [Header("General")]
    private SpriteRenderer playerSprite;
    private Animator playerAnimator;
    private Player player;
    [SerializeField] private bool isFacingRight = false; //Is character facing right side? for Character flip
    private bool settingFacingRight;
    private Player.PlayerState currentPlayerState;

    [Header("Shooting")]
    private Shooting shooting;
    private Shooting.AimLineState currentAimLineState;

    [Header("Metal")]
    private MovingMetal movingMetal;
    private bool goingToRightMetal = false;

    [Header("Sound")]
    private FMODStudioFootstepScript footstepScript;

    // Start is called before the first frame update
    void Start()
    {
        playerSprite = gameObject.GetComponent<SpriteRenderer>();
        playerAnimator = gameObject.GetComponent<Animator>();
        player = FindObjectOfType<Player>();
        shooting = FindObjectOfType<Shooting>();
        footstepScript = FindObjectOfType<FMODStudioFootstepScript>();
        movingMetal = FindObjectOfType<MovingMetal>();

        isFacingRight = true;
        playerAnimator.SetInteger("PlayerAnimState", 0);
    }

    // Update is called once per frame
    void Update()
    {
        ChooseAnimation();
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
        // change animations state based on changes in player state
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
                case Player.PlayerState.MOVING_OBJECT_START:
                    if (player.GetAnimationMovement())
                    {
                        playerAnimator.SetInteger("PlayerAnimState", 8);
                    }
                    else
                    {
                        player.SetKinematic(true);
                        playerAnimator.SetInteger("PlayerAnimState", 7);
                    }

                    break;
                case Player.PlayerState.MOVING_OBJECT:
                    playerAnimator.SetInteger("PlayerAnimState", 9);
                    break;
                case Player.PlayerState.MOVING_OBJECT_END:
                    playerAnimator.SetInteger("PlayerAnimState", 10);
                    break;
                case Player.PlayerState.MOVING_OBJECT_IDLE:
                    playerAnimator.SetInteger("PlayerAnimState", 11);
                    break;
                case Player.PlayerState.MOVING_OBJECT_LEFT:
                    player.SetKinematic(true);
                    playerAnimator.SetInteger("PlayerAnimState", 9);
                    break;
                case Player.PlayerState.MOVING_OBJECT_STOPPED_LEFT:
                    playerAnimator.SetInteger("PlayerAnimState", 11);
                    break;
                case Player.PlayerState.MOVING_OBJECT_RIGHT:
                    player.SetKinematic(true);
                    playerAnimator.SetInteger("PlayerAnimState", 9);
                    break;
                case Player.PlayerState.MOVING_OBJECT_STOPPED_RIGHT:
                    playerAnimator.SetInteger("PlayerAnimState", 11);
                    break;
                case Player.PlayerState.DEATH:

                    break;
                case Player.PlayerState.KICK_BUCKET_START:
                    if (player.GetAnimationMovement())
                    {
                        playerAnimator.SetInteger("PlayerAnimState", 6);
                        player.SetPlayerState(Player.PlayerState.KICKING_BUCKET);
                    }
                    else
                    {
                        playerAnimator.SetInteger("PlayerAnimState", 5);
                        player.SetPlayerState(Player.PlayerState.KICK_BUCKET);
                    }

                    break;
                case Player.PlayerState.KICK_BUCKET:


                    break;
                case Player.PlayerState.INTERACT:

                    break;

            }
        }
        //used for automated animations
        else
        {
            switch (player.GetPlayerState())
            {
                case Player.PlayerState.KICKING_BUCKET:
                    if (Mathf.Abs(player.transform.position.x - (PlayerObjectInteractions.playerObjectIInstance
                        .GetCurrentObject().transform.position.x)) < .5f)
                    {
                        player.SetAnimationMovement(false);
                        playerAnimator.SetInteger("PlayerAnimState", 0);
                        player.SetPlayerState(Player.PlayerState.KICK_BUCKET);
                    }

                    break;
                case Player.PlayerState.MOVING_OBJECT_START: 
                    player.SetKinematic(false);
                    

                    if (goingToRightMetal)
                    {

                        if (Mathf.Abs(player.transform.position.x - (PlayerObjectInteractions.playerObjectIInstance
                            .GetMetalRightPos().transform.position.x)) < .1f)
                        {
                            player.SetPlayerState(Player.PlayerState.MOVING_OBJECT);
                            if (!isFacingRight)
                            {
                                player.SetMovingRight(true);
                            }


                            playerAnimator.SetInteger("PlayerAnimState", 9);
                        }
                    }
                    else
                    {
                        if (Mathf.Abs(player.transform.position.x - (PlayerObjectInteractions.playerObjectIInstance
                            .GetMetalLeftPos().transform.position.x)) < .1f)
                        {
                            player.SetPlayerState(Player.PlayerState.MOVING_OBJECT);
                            playerAnimator.SetInteger("PlayerAnimState", 9);

                            if (isFacingRight)
                            {
                                player.SetMovingRight(false);
                            }

                        }
                    }

                    break;
            }
        }
        //change animation state with aiming state change
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
        //sets to current player state and and aim state to check for changes later  
        currentAimLineState = shooting.GetAimLineState();
        currentPlayerState = player.GetPlayerState();
    }


    //------------------
    // General functions 
    //------------------

    //used if there is a situation where you have to set the animation state from another script
    public void SetAnimation(int animNum)
    {
        playerAnimator.SetInteger("PlayerAnimState", animNum);
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

    public void PlayFootStep()
    {
        footstepScript.PlayerFootsteep();
    }

    public void ElectrifyInteractionEnd()
    {
        //end electrify interaction
    }


    //------------------
    // Metal functions 
    //------------------
    public void SetGoingToRightSideOfMetal(bool state)
    {
        goingToRightMetal = state;
    }

    public bool GetGoingToRightSideOfMetal()
    {
        return goingToRightMetal;
    }


    public void StartDraggingMetal()
    {
        player.SetAnimationMovement(false);
    }

    public void StartPickingUpMetal()
    {
        player.SetKinematic(true);
        PlayerObjectInteractions.playerObjectIInstance.ConnectMetalToPlayer();

    }

    public void EndPuttingMetalDown()
    {
        if (isFacingRight)
        {
            player.SetMovingRight(false);
        }
        else
        {
            player.SetMovingRight(true);
        }

        PlayerObjectInteractions.playerObjectIInstance.GetCurrentObject().GetComponent<Metal>().SetMoving(false);
        player.SetPlayerState(Player.PlayerState.IDLE);
    }

    //------------------
    // Bucket functions 
    //------------------


    //when player kicks bucket
    public void KickBucketOver()
    {
        Debug.Log("KickBucketOver");
        player.InteractWithObject();
        player.SetPlayerState(Player.PlayerState.IDLE);
    }

    //--------
    //Shooting
    //--------

    public void StopAiming()
    {
        shooting.SetAimLineState(Shooting.AimLineState.NOT_AIMED);
    }
}