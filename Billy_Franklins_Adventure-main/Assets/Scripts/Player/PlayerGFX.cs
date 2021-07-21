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
    private MovingMetal movingMetal;

    private bool goingToRightMetal = false;

    [Header("FMOD Settings")] [FMODUnity.EventRef] [SerializeField]
    private string buckKickEventRef;

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

    public bool GetFacingRight()
    {
        return isFacingRight;
    }

    public void SetFacingRight(bool state)
    {
        isFacingRight = state;
        settingFacingRight = true;
    }

    public void SetGoingToRightMetalDirection(bool state)
    {
        goingToRightMetal = state;
    }

    public bool GetGoingToRightMetalDirection()
    {
        return goingToRightMetal;
    }

    //public void SetAnimator(int animState)
    //{
    //    playerAnimator.SetInteger("PlayerAnimState", animState);
    //}

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
                case Player.PlayerState.MOVING_OBJECT_START:
                    if (player.GetAnimationMovement())
                    {
                        Debug.Log("Moving");
                        player.SetKinematic(true);
                        playerAnimator.SetInteger("PlayerAnimState", 8);
                    }
                    else
                    {
                        Debug.Log("Not Moving");
                        player.SetKinematic(true);
                        playerAnimator.SetInteger("PlayerAnimState", 7);
                    }

                    break;
                case Player.PlayerState.MOVING_OBJECT:
                    //player.SetKinematic(true);
                    playerAnimator.SetInteger("PlayerAnimState", 9);
                    break;
                case Player.PlayerState.MOVING_OBJECT_END:
                    playerAnimator.SetInteger("PlayerAnimState", 10);
                    //player.SetKinematic(true);
                    break;
                //case Player.PlayerState.MOVING_OBJECT_SWITCH_START:
                //    player.SetKinematic(true);
                //    Debug.Log("Switch start");
                //    playerAnimator.SetInteger("PlayerAnimState", 10);
                //    break;
                //case Player.PlayerState.MOVING_OBJECT_SWITCH:
                //    playerAnimator.SetInteger("PlayerAnimState", 8);
                //    break;
                //case Player.PlayerState.MOVING_OBJECT_SWITCH_END:
                //    Debug.Log("Switch End");
                //    //player.SetAnimationSwitch(false);
                //    player.SetKinematic(true);
                //    playerAnimator.SetInteger("PlayerAnimState", 9);
                //    break;
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
                        Debug.Log("Moving");
                        playerAnimator.SetInteger("PlayerAnimState", 6);
                        player.SetPlayerState(Player.PlayerState.KICKING_BUCKET);
                    }
                    else
                    {
                        Debug.Log("Not Moving");
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
                            Debug.Log("Going to right");
                            player.SetPlayerState(Player.PlayerState.MOVING_OBJECT);
                            if (isFacingRight)
                            {
                                player.MetalFacingFix(true);
                                isFacingRight = false;
                            }

                            playerAnimator.SetInteger("PlayerAnimState", 9);
                        }
                    }
                    else
                    {
                        if (Mathf.Abs(player.transform.position.x - (PlayerObjectInteractions.playerObjectIInstance
                            .GetMetalLeftPos().transform.position.x)) < .1f)
                        {
                            if (!isFacingRight)
                            {
                                player.MetalFacingFix(true);
                                isFacingRight = true;
                            }
                            Debug.Log("Going to left");
                            //player.MetalFacingFix(true);
                            player.SetPlayerState(Player.PlayerState.MOVING_OBJECT);
                            playerAnimator.SetInteger("PlayerAnimState", 9);
                        }
                    }

                    break; 
                //case Player.PlayerState.MOVING_OBJECT_SWITCH:
                //    player.SetKinematic(false);
                //    Debug.Log("Switching");
                //    if (player.GetAnimationMovingRight())
                //    {
                //        Debug.Log("Going Right: " + Mathf.Abs(player.transform.position.x - (PlayerObjectInteractions.playerObjectIInstance
                //            .GetMetalRightPos().transform.position.x)));
                //        if (Mathf.Abs(player.transform.position.x - (PlayerObjectInteractions.playerObjectIInstance
                //            .GetMetalRightPos().transform.position.x)) < .5f)
                //        {
                //            Debug.Log("Switching End 1");
                //            player.SetKinematic(true);
                //            player.SetPlayerState(Player.PlayerState.MOVING_OBJECT);
                //            player.MetalFacingFix(false);
                //            //playerAnimator.SetInteger("PlayerAnimState", 9);
                //            player.SetAnimationSwitch(false);
                //            //player.SetKinematic(true);
                //            playerAnimator.SetInteger("PlayerAnimState", 9);

                //        }
                //    }
                //    else
                //    {
                //        Debug.Log("Going Left: " + Mathf.Abs(player.transform.position.x - (PlayerObjectInteractions.playerObjectIInstance
                //            .GetMetalLeftPos().transform.position.x)));
                //        if (Mathf.Abs(player.transform.position.x - (PlayerObjectInteractions.playerObjectIInstance
                //            .GetMetalLeftPos().transform.position.x)) < .5f)
                //        {
                //            Debug.Log("Switching End 2");
                //            player.SetKinematic(true);
                //            player.MetalFacingFix(false);
                //            player.SetPlayerState(Player.PlayerState.MOVING_OBJECT);
                //            //playerAnimator.SetInteger("PlayerAnimState", 9);
                //            player.SetAnimationSwitch(false);
                //            //player.SetKinematic(true);
                //            playerAnimator.SetInteger("PlayerAnimState", 9);
                //        }
                //    }
                //    break;

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
        //if (player.GetPlayerState() == Player.PlayerState.KICK_BUCKET_START)
        //{
        //    player.SetPlayerState(Player.PlayerState.KICK_BUCKET);
        //}
        //else if (player.GetPlayerState() == Player.PlayerState.KICK_BUCKET)
        //{

        //}
    }

    //when player kicks bucket
    public void KickBucketOver()
    {
        Debug.Log("KickBucketOver");
        player.InteractWithObject();
        PlayBucketKickSound();
        player.SetPlayerState(Player.PlayerState.IDLE);
    }

    public void PlayFootStep()
    {
        footstepScript.PlayerFootsteep();
    }

    public void PlayBucketKickSound()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(buckKickEventRef, gameObject);
    }

    public void StartDraggingMetal()
    {
        
        Debug.Log("Dragging Metal");
        player.SetAnimationMovement(false);
    }

    public void StartPickingUpMetal()
    {
        player.SetKinematic(true);
        PlayerObjectInteractions.playerObjectIInstance.ConnectMetalToPlayer();
        
    }

    public void EndPuttingMetalDown()
    {
        //if (player.GetAnimationSwitch())
        //{

        //    player.SetPlayerState(Player.PlayerState.MOVING_OBJECT_SWITCH);
        //    //PlayerObjectInteractions.playerObjectIInstance.GetCurrentObject().GetComponent<Metal>().SetMoving(false);
        //    PlayerObjectInteractions.playerObjectIInstance.GetCurrentObject().GetComponent<Metal>().DisconnectMetalFromPlayerSwitch();
        //    if (player.GetAnimationMovingRight())
        //    {
        //        isFacingRight = true;
        //        player.SetAnimationMovingRight(false);
                
        //    }
        //    else
        //    {
        //        isFacingRight = false;
        //        player.SetAnimationMovingRight(true);
                
        //    }
        //}
        //else
        //{
        PlayerObjectInteractions.playerObjectIInstance.GetCurrentObject().GetComponent<Metal>().SetMoving(false);
        

        if (isFacingRight)
        {
            isFacingRight = false;
        }
        else
        {
            isFacingRight = true;
        }

        player.OtherMetalFacingFix();

        player.SetPlayerState(Player.PlayerState.IDLE);
        //}
    }

    //public void PutMetalDownSwitch()
    //{
    //    Debug.Log("SWITCH");
    //    player.SetKinematic(true);
    //    playerAnimator.SetInteger("PlayerAnimState", 10);
    //}
}