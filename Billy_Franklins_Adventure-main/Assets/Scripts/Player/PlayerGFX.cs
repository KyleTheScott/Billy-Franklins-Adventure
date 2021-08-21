using System;
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
    private KiteLightning kiteLightning;
    [SerializeField] private bool isFacingRight = false; //Is character facing right side? for Character flip
    private bool settingFacingRight;
    private Player.PlayerState currentPlayerState;

    private CheckPointSystem checkPointSystem = null;

    [SerializeField] private GameObject playerLight;


    [Header("Shooting")]
    private Shooting shooting;
    private Shooting.AimLineState currentAimLineState;

    [Header("Metal")]
    private MovingMetal movingMetal;
    private bool goingToRightMetal = false;

    [Header("Sound")]
    private FMODStudioFootstepScript footstepScript;

    private float playerMetalStuckTimer = 0;
    private float playerMetalStuckTimerMax = 1;

    private bool lampIsOn = false;

    void Awake()
    {
        playerAnimator = gameObject.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerSprite = gameObject.GetComponent<SpriteRenderer>();
        
        player = FindObjectOfType<Player>();
        shooting = FindObjectOfType<Shooting>();
        footstepScript = FindObjectOfType<FMODStudioFootstepScript>();
        movingMetal = FindObjectOfType<MovingMetal>();
        kiteLightning = FindObjectOfType<KiteLightning>();
        checkPointSystem = GameObject.Find("GlobalGameController").GetComponent<CheckPointSystem>();
        playerLight = GameObject.Find("PlayerLight");
        isFacingRight = true;
        playerAnimator.SetInteger("PlayerAnimState", 0);
    }

    // Update is called once per frame
    void Update()
    {
        ChooseAnimation();
    }

    public void SetLampOn(bool state)
    {
        lampIsOn = state;
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
                    //Debug.LogError("Changed to falling");
                   
                    break;
                case Player.PlayerState.FALLING:
                    playerAnimator.SetInteger("PlayerAnimState", 4);
                    break;
                case Player.PlayerState.FALL_FIX:

                    break;
                case Player.PlayerState.MOVING_OBJECT_START:
                    playerMetalStuckTimer = 0;
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
                case Player.PlayerState.KICK_BUCKET_START:
                    if (player.GetAnimationMovement())
                    {
                        playerAnimator.SetInteger("PlayerAnimState", 6);
                        player.SetAnimationMovement(true);
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
                case Player.PlayerState.LIGHTNING_CHARGES_START:
                    if (GetAnimation() == 3)
                    {
                        SetAnimation(20);
                    }
                    else
                    {
                        playerAnimator.SetInteger("PlayerAnimState", 15);
                    }
                    break;
                case Player.PlayerState.LIGHTNING_CHARGES:
                    playerAnimator.SetInteger("PlayerAnimState", 0);
                    break;
                case Player.PlayerState.PLAYER_DEATH_ELECTRIFIED_START:
                    playerAnimator.SetInteger("PlayerAnimState", 13);
                    player.SetPlayerState(Player.PlayerState.PLAYER_DEATH_ELECTRIFIED);
                    break;
                case Player.PlayerState.PLAYER_DEATH_CHARGES_START:
                    //Debug.LogError("Out Of the Charges");
                    playerAnimator.SetInteger("PlayerAnimState", 14);
                    break;
                case Player.PlayerState.INTERACT:
                    playerAnimator.SetInteger("PlayerAnimState", 12);
                    break;
                case Player.PlayerState.PLAYER_END_GAME_START:
                    player.SetMovingRight(true);
                    playerAnimator.SetInteger("PlayerAnimState", 1);
                    break;

            }
        }
        //used for automated animations when animation movement is true in the player script
        else
        {
            switch (player.GetPlayerState())
            {
                case Player.PlayerState.KICKING_BUCKET:
                    if (Mathf.Abs(player.transform.position.x - (PlayerObjectInteractions.playerObjectIInstance
                        .GetCurrentObject().transform.position.x)) < .5f)
                    {
                        playerAnimator.SetInteger("PlayerAnimState", 0);
                        player.SetPlayerState(Player.PlayerState.KICK_BUCKET);
                    }

                    break;
                case Player.PlayerState.MOVING_OBJECT_START:
                    playerMetalStuckTimer += Time.deltaTime;
                    if (playerMetalStuckTimer >= playerMetalStuckTimerMax)
                    {
                        
                        MetalStuckStop();
                    }
                    else
                    {
                        player.SetKinematic(false);
                        //player is walking right to metal
                        if (isFacingRight)
                        {
                            //walking to right metal
                            if (goingToRightMetal)
                            {
                                if (PlayerObjectInteractions.playerObjectIInstance.GetMetalRightPos().transform.position.x -
                                    player.transform.position.x <= 0)
                                {
                                    player.SetPlayerState(Player.PlayerState.MOVING_OBJECT_LIFTING);
                                    playerAnimator.SetInteger("PlayerAnimState", 9);
                                }
                            }
                            //walking to left metal
                            else
                            {
                                if (PlayerObjectInteractions.playerObjectIInstance.GetMetalLeftPos().transform.position.x -
                                    player.transform.position.x <= 0)
                                {
                                    player.SetPlayerState(Player.PlayerState.MOVING_OBJECT_LIFTING);
                                    playerAnimator.SetInteger("PlayerAnimState", 9);
                                    player.SetMovingRight(false);
                                }
                            }
                        }
                        //player is walking left to metal
                        else
                        {
                            //walking to right metal
                            if (goingToRightMetal)
                            {
                                if (player.transform.position.x -
                                    PlayerObjectInteractions.playerObjectIInstance.GetMetalRightPos().transform.position.x <= 0)
                                {
                                    player.SetPlayerState(Player.PlayerState.MOVING_OBJECT_LIFTING);
                                    playerAnimator.SetInteger("PlayerAnimState", 9);
                                    player.SetMovingRight(true);
                                }
                            }
                            //walking to left metal
                            else
                            {
                                if (player.transform.position.x -
                                    PlayerObjectInteractions.playerObjectIInstance.GetMetalLeftPos().transform.position.x <= 0)
                                {
                                    player.SetPlayerState(Player.PlayerState.MOVING_OBJECT_LIFTING);
                                    playerAnimator.SetInteger("PlayerAnimState", 9);
                                }
                            }
                        }
                    }
                    break;
                case Player.PlayerState.PLAYER_END_GAME:

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
                if (player.GetMovementEnabled())
                {
                    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                    {
                        playerAnimator.SetInteger("PlayerAnimState", 1);
                    }
                    else
                    {
                        playerAnimator.SetInteger("PlayerAnimState", 0);
                    }
                }
            }
        }
        //sets to current player state and and aim state to check for changes later  
        currentAimLineState = shooting.GetAimLineState();
        currentPlayerState = player.GetPlayerState();
        if (currentPlayerState == Player.PlayerState.LIGHTNING_CHARGES_START)
        {
            player.SetPlayerState(Player.PlayerState.LIGHTNING_CHARGES);
        }
    }


    //------------------
    // General functions 
    //------------------

    //used if there is a situation where you have to set or get the animation state from another script
    public void SetAnimation(int animNum)
    {
        playerAnimator.SetInteger("PlayerAnimState", animNum);
    }

    public int GetAnimation()
    {
        return playerAnimator.GetInteger("PlayerAnimState");
    }

    public void StartPlayerJump()
    {
        player.StartJump();
    }

    public void EndPlayerJump()
    {
        if (!player.IsPlayerOnGround())
        {
            player.SetPlayerState(Player.PlayerState.JUMP_FALLING);
            playerAnimator.SetInteger("PlayerAnimState", 4);
        }

        //player.SetAnimationMovement(false);
    }

    public void ElectrifyInteract()
    {
        player.ElectrifyInteract();
    }
    public void EndElectrifyInteract()
    {
      
        player.SetPlayerState(Player.PlayerState.IDLE);
        if (FindObjectOfType<Charges>().GetLightCharges() <= 0 && !FindObjectOfType<Charges>().GetLampOn())
        {
            FindObjectOfType<Player>().StartPlayerOutOfChargesDeath();
        }
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

    //-------------------------
    // Electrify kite functions 
    //-------------------------

    public void ElectrifyKiteLightning()
    {
        kiteLightning.KiteElectrifyStart();
    }

    public void ElectrifyKiteEnd()
    {
        player.SetPlayerState(Player.PlayerState.WALKING);
        FindObjectOfType<Shooting>().SetAimLineState(Shooting.AimLineState.NOT_AIMED);
        //GameObject lightToTurnOn = GameObject.Find("Light");
        playerLight.SetActive(true);
        player.SetAnimationMovement(false);
        player.PlayerControlsStatus(true);
        player.StartPlayerMovement();
    }

    //----------------
    // Death functions 
    //----------------
    public void ElectrifyDeathStart()
    {
        GameplayUI.instanceGameplayUI.FadeOut();
    }

    public void ElectricityDeathEnd()
    {
        //Debug.LogError("Death Working End");
        FindObjectOfType<ObjectsCollision>().EmptyObjects();
        checkPointSystem.PlayerDeath();
    }

    public void OutOfChargesTurnLightOff()
    {
        GameplayUI.instanceGameplayUI.FadeOut();
        //playerLight.SetActive(false);
    }

    public void OutOfChargesDeathEnd()
    {
        //Debug.LogError("Death 2");
        //Debug.LogError("Death animation");
        FindObjectOfType<ObjectsCollision>().EmptyObjects();
        checkPointSystem.PlayerDeath();
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
        player.SetPlayerState(Player.PlayerState.MOVING_OBJECT);
        player.SetAnimationMovement(false);
    }

    public void StartPickingUpMetal()
    {
        player.SetKinematic(true);
        PlayerObjectInteractions.playerObjectIInstance.ConnectMetalToPlayer();
        if (player.GetOnDiagonalPlatform())
        {
            player.SetPlayerMoveMetalOnPlatform();
        }
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
        player.SetAnimationMovement(false);
        PlayerObjectInteractions.playerObjectIInstance.GetCurrentObject().GetComponent<Metal>().SetMoving(false);
        PlayerObjectInteractions.playerObjectIInstance.GetCurrentObject().GetComponent<IInteractable>().SetHighlighted(true);
        player.SetPlayerState(Player.PlayerState.IDLE);
        player.SetMovingMetalStop();
    }

    private void MetalStuckStop()
    {
        player.SetAnimationMovement(false);
        if (PlayerObjectInteractions.playerObjectIInstance.GetCurrentObject() != null)
        {
            PlayerObjectInteractions.playerObjectIInstance.GetCurrentObject().GetComponent<Metal>().SetMoving(false);
        }

        player.SetPlayerState(Player.PlayerState.IDLE);
        player.SetMovingMetalStop();
        playerAnimator.SetInteger("PlayerAnimState", 0);
    }


    //------------------
    // Bucket functions 
    //------------------


    //when player kicks bucket
    public void KickBucketOver()
    {
        player.InteractWithObject();
        
    }

    public void KickBucketOverEnd()
    {
        player.SetAnimationMovement(false);
        player.SetPlayerState(Player.PlayerState.IDLE);
        PlayerObjectInteractions.playerObjectIInstance.SetInteracting(false);
        
    }

    //------------------
    //Shooting functions
    //------------------
    public void StopAiming()
    {
        shooting.SetAimLineState(Shooting.AimLineState.NOT_AIMED);
    }

    public void StartShooting()
    {
        shooting.StartShooting();
    }

    public void EndShooting()
    {
        //Debug.LogError("Out of charges Test");

        if (FindObjectOfType<Charges>().GetLightCharges() > 0 || lampIsOn)
        {
            shooting.StopShooting();
            player.SetPlayerState(Player.PlayerState.WALKING);
            playerAnimator.SetInteger("PlayerAnimState", 1);
            lampIsOn = false;
            player.SetAnimationMovement(false);
            //Debug.LogError("Shooting end");
        }
    }

    public void EndAimForReset()
    {
        //Debug.LogError("Stop Shooting");
        //shooting.StopAiming();


    }

}