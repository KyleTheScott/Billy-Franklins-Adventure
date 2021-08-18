using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
[DefaultExecutionOrder(-100)] //ensure this script runs before all other player scripts to prevent laggy input
public class Player : MonoBehaviour
{
    //to merge
    private CheckPointSystem checkPointDeathSystem = null;

    [SerializeField] private Charges charges;
    [SerializeField] private Shooting shooting;

    [Header("General")] Rigidbody2D rb = null; //player's rigid body

    [SerializeField] private PlayerGFX playerGFX; // players animations
    [SerializeField] private GameObject aimLine;
    [SerializeField] private PlayerCollision playerCollision; // player collision with ground

    public enum PlayerState
    {
        IDLE,
        JUMP,
        JUMPING,
        JUMP_FALLING,
        FALLING,
        FALL_FIX,
        WALKING,
        MOVING_OBJECT_START,
        MOVING_OBJECT,
        MOVING_OBJECT_END,
        MOVING_OBJECT_IDLE,
        MOVING_OBJECT_STOPPED_LEFT,
        MOVING_OBJECT_LEFT,
        MOVING_OBJECT_STOPPED_RIGHT,
        MOVING_OBJECT_RIGHT,
        DEATH,
        KICK_BUCKET_START,
        KICKING_BUCKET,
        KICK_BUCKET,
        INTERACT,
        LIGHTNING_CHARGES_START,
        LIGHTNING_CHARGES,
        LIGHTNING_CHARGES_END,
        PLAYER_DEATH_ELECTRIFIED_START,
        PLAYER_DEATH_ELECTRIFIED,
        PLAYER_DEATH_CHARGES_START,
        PLAYER_DEATH_CHARGES
    }

    [SerializeField] private PlayerState playerState = PlayerState.WALKING;
    public PlayerState PlayersState => playerState;
    [SerializeField] bool debugMode = false;

    //public enum PlayerInLevelState
    //{
    //    IN_LEVEL,
    //    CHANGING_LEVEL,
    //    LEVEL_CHANGE
    //}

    //[SerializeField] private PlayerInLevelState playerInLevelState = PlayerInLevelState.IN_LEVEL;
    [SerializeField] private bool playerInLevel;


    [Header("Movement")] [SerializeField] private bool onGround = true; // keeps track if player is  on ground

    private Vector3 lastPosition; // used to store the players position each frame
    [SerializeField] private float moveSpeed = 4.0f; // regular speed of the player
    [SerializeField] private float jumpMoveSpeed = 10.0f; // jump speed of the player
    [SerializeField] private float moveObjectSpeed = 3.0f; // speed of the player moving while moving an object
    [SerializeField] private float moveVelocity = 0; //


    //changes in gravity
    [SerializeField] private float jumpGravity = 1f; // gravity while jumping
    [SerializeField] private float fallGravity = 5f; // gravity while falling

    [SerializeField]
    private float groundGravity = 10f; // gravity while the player is walking on a surface tagged ground 

    //variable for jump and fall from jump
    [SerializeField] private float jumpForce = 10f; //How strong does player jump
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float fallMultiplier = 4f;

    //used to fix an issue with jumping by delaying when collision with ground is checked when jumping  
    private bool jumpFix = false;
    private float jumpFixTimer = 0;
    private float jumpFixTime = .1f;


    [SerializeField] private PlayerState currentPlayerState; // player state used to set player state after a jump or fall when hitting ground

    //variables used for when player is on moving platforms to make the player fall and move with the platform
    [SerializeField] private float fallFixTimer = 0;
    [SerializeField] private float fallFixMax = .1f;
    [SerializeField] private float fallFixStayMax = 2f;
    private bool fallFixSwitch = false;

    //[SerializeField] private bool fallFromMetal = false; // might use this in the future

    //delay used in metal movement
    private bool droppingMetal = false;
    private float dropMetalTimer = 0;
    private float dropMetalTime = 2f;


    [SerializeField] private bool onDiagonalPlatform = false;

    [SerializeField] private GameObject currentPlatform;

    //private Quaternion rotation;

    //public GameObject worldScale;


    [Header("FMOD Setting")]
    //FMOD Event Refs
    [FMODUnity.EventRef]
    public string jumpSound;

    [SerializeField] private float jumpSoundVolume = 0.8f;
    [FMODUnity.EventRef] [SerializeField] private string metalMovingEventRef;
    private FMOD.Studio.EventInstance metalMovingEvent;
    [SerializeField] private float metalMovingVolume = 0.8f;

    [Header("Interact")]
    //stores object the player is currently moving 
    [SerializeField]
    private GameObject currentMovingObject;

    [SerializeField] private Metal currentPlayerMetal;

    [Header("Animation")] 
    [SerializeField] private bool animationMovement = false; // an automated animation is happening

    [SerializeField] private bool movingMetal = false; // player is moving metal

    [SerializeField] private GameObject pickUpMetalPos;


    [Header("Menus")] [SerializeField] private Canvas pauseMenuUI = null;
    private Canvas settingsMenuUI = null;
    private bool movementEnabled = true;

    [Header("Dialogue")] public bool isReading = false;
    public Dialogue dialogue = new Dialogue();
    public bool finalDialogue = false;
    [SerializeField]
    private string MainMenuScene;

    public void SetPlayerKiteLightning()
    {
        StartCoroutine(WaitTillOnGround());
    }
    //public 

    IEnumerator WaitTillOnGround()
    {
        while (!IsPlayerOnGround())
        {
            yield return null;
        }
        Debug.LogError("On Ground Start");
        SetPlayerState(Player.PlayerState.LIGHTNING_CHARGES_START);
        SetAnimationMovement(true);
        SetPlayerInLevel(true);
    }

    public void SetPlayerInLevel(bool state)
    {
        playerInLevel = state;
    }

    public bool GetPlayerInLevel()
    {
        return playerInLevel;
    }

    public void SetCurrentPlatform(GameObject obj)
    {
        //rotation = transform.rotation;
        currentPlatform = obj;
        if (onDiagonalPlatform)
        {
            Vector3 fixRotation = transform.eulerAngles;
            fixRotation.z = 0;
            transform.eulerAngles = fixRotation;
            //transform.localScale = new Vector3(transform.localToWorldMatrix.lossyScale.x, transform.localToWorldMatrix.lossyScale.y, transform.localToWorldMatrix.lossyScale.z);
            ////transform.localScale = new Vector3(1, 1, transform.localToWorldMatrix.lossyScale.z);

            //transform.localScale = new  Vector3 (worldScale.transform.worldToLocalMatrix.lossyScale.x, 
            //    worldScale.transform.worldToLocalMatrix.lossyScale.y, worldScale.transform.worldToLocalMatrix.lossyScale.z)  ;
            //Vector3 worldScale = new Vector3(1, 1, 1);
            //transform.parent.InverseTransformVector(worldScale);
            //float xDiff = 1 / transform.parent.localScale.x;
            //float yDiff = 1 / transform.parent.localScale.y;
            //transform.localScale = new Vector3(transform.localScale.x + xDiff, transform.localScale.y + yDiff, transform.localScale.z);

            //Debug.LogError("Platform Scale x: " + currentPlatform.transform.localScale.x);
            //Debug.LogError("Platform Scale y: " + currentPlatform.transform.localScale.y);
            //Debug.LogError("Platform Scale z: " + currentPlatform.transform.localScale.z);
            //Debug.LogError("Player Scale z: " + transform.lossyScale.x);
            //Debug.LogError("Player Scale z: " + transform.lossyScale.y);
            //Debug.LogError("Player Scale z: " + transform.lossyScale.z);

            //float x = 1 / currentPlatform.transform.lossyScale.x;
            //float y = 1 / currentPlatform.transform.lossyScale.y;

            //transform.localScale = new Vector3(transform.lossyScale.x * x, transform.lossyScale.y * y, 1);
            //;

            //transform.localScale = transform.parent.InverseTransformVector(worldScale);
        }
        //Vector3 fixRotation = transform.eulerAngles;
        //fixRotation.z = 0;
        //if (currentPlatform != null)
        //{
        //    ////fixRotation.z += 0 - currentPlatform.transform.eulerAngles.z;
        //    if (currentPlatform.transform.eulerAngles.z > 0)
        //    {
        //        Debug.LogError("Platform Euler z: " + currentPlatform.transform.eulerAngles.z);
        //        Debug.LogError("Player Euler z: " + fixRotation.z);
        //        //transform.localRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z - currentPlatform.transform.rotation.z);
        //        //fixRotation.z += currentPlatform.transform.eulerAngles.z - 45;
        //        //fixRotation.z = 0;
        //    }
        //    else
        //    {
        //        //fixRotation.z = 0;
        //        //transform.localRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z - currentPlatform.transform.rotation.z);
        //        //fixRotation.z -= Mathf.Abs(45 - currentPlatform.transform.eulerAngles.z);
        //    }

        //    transform.eulerAngles = fixRotation;
        //}

    }
    public void SetRotation()
    {
        Vector3 fixRotation = transform.eulerAngles;
        fixRotation.z = 0;
        transform.eulerAngles = fixRotation;
    }



// Start is called before the first frame update
    void Start()
    {
        
        metalMovingEvent = FMODUnity.RuntimeManager.CreateInstance(metalMovingEventRef);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(metalMovingEvent, transform, GetComponent<Rigidbody>());

        //reference checkpoint + death system script 
        checkPointDeathSystem = GameObject.Find("GlobalGameController").GetComponent<CheckPointSystem>();

        rb = GetComponent<Rigidbody2D>(); // player rigidbody

        shooting.SetAimLineState(Shooting.AimLineState.NOT_AIMED);
        playerGFX.SetFacingRight(true);

        // setting some generally player movement variables
        //playerState = PlayerState.JUMPING;
        GameplayUI.instanceGameplayUI.ResetFadeIn();
        GameplayUI.instanceGameplayUI.FadeIn();
        SetAnimationMovement(true);

        transform.Rotate(0f, 180f, 0f);
        playerCollision = GameObject.FindObjectOfType<PlayerCollision>();

        //parentConstraint = gameObject.GetComponent<ParentConstraint>();

        try
        {
            pauseMenuUI = FindObjectOfType<PauseMenu>().GetComponent<Canvas>();
            pauseMenuUI.gameObject.SetActive(false);
        }
        catch
        {
            Debug.Log("Couldn't find pause menu UI...");
        }
        //worldScale = GameObject.Find("WorldScale");
        aimLine = FindObjectOfType<AimLine>().gameObject;
        //rotation = transform.rotation;
        //kite.SetKiteStartPosition(transform.position);
        
        
        
    }

    //used for moving player depending on state
    private void FixedUpdate()
    {
        //for if an automated animation is happening
        if (animationMovement)
        {
            if (playerState != PlayerState.MOVING_OBJECT)
            {
                rb.isKinematic = false;
            }

            switch (playerState)
            {
                case PlayerState.KICKING_BUCKET:
                    rb.velocity = new Vector2(moveVelocity, rb.velocity.y);
                    break;
                case PlayerState.KICK_BUCKET:
                    rb.velocity = Vector2.zero;
                    rb.isKinematic = true;
                    break;
                case PlayerState.MOVING_OBJECT_START:
                    rb.velocity = new Vector2(moveVelocity, rb.velocity.y);
                    break;
                case PlayerState.MOVING_OBJECT:
                    animationMovement = false;
                    rb.velocity = new Vector2(moveVelocity, rb.velocity.y);
                    break;
            }
        }
        else
        {

            switch (playerState)
            {
                case PlayerState.IDLE:
                    if (lastPosition != transform.position)
                    {
                        rb.velocity = Vector2.zero;
                        rb.isKinematic = true;
                    }

                    break;
                //the start of the jump
                case PlayerState.JUMP:
                    //rb.velocity = new Vector2(moveVelocity / 4, rb.velocity.y);
                    break;
                //in the air of the jump
                case PlayerState.JUMPING:
                    //Debug.Log("Jumping");
                    if (rb.velocity.y > 0)
                    {
                        rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier * 1) * Time.deltaTime;
                        if (playerGFX.GetFacingRight())
                        {
                            if (moveVelocity != 0)
                            {
                                rb.velocity = new Vector2(jumpMoveSpeed, rb.velocity.y);
                            }
                            else
                            {
                                rb.velocity = new Vector2(0, rb.velocity.y);
                            }
                        }
                        else
                        {
                            if (moveVelocity != 0)
                            {
                                rb.velocity = new Vector2(-jumpMoveSpeed, rb.velocity.y);
                            }
                            else
                            {
                                rb.velocity = new Vector2(0, rb.velocity.y);
                            }
                        }
                    }
                    else
                    {
                        playerState = PlayerState.JUMP_FALLING;
                    }
                    break;
                //falling from a jump
                case PlayerState.JUMP_FALLING:
                    //Debug.Log("Falling");
                    rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier * 1) * Time.deltaTime;
                    if (moveVelocity != 0)
                    {
                        rb.velocity = new Vector2(moveVelocity, rb.velocity.y);
                    }
                    else
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                    }
                    break;
                //falling but not from a jump
                case PlayerState.FALLING:
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    break;
                //state for when player is on a moving platform
                case PlayerState.FALL_FIX:
                    if (fallFixTimer >= fallFixStayMax)
                    {
                        rb.isKinematic = false;
                        playerState = currentPlayerState;
                        fallFixTimer = 0;
                        onGround = true;
                    }
                    else
                    {
                        fallFixTimer += Time.deltaTime;
                    }

                    if (currentPlayerState != PlayerState.IDLE)
                    {
                        rb.velocity = new Vector2(moveVelocity, rb.velocity.y);
                    }

                    break;
                case PlayerState.WALKING:
                    rb.velocity = new Vector2(moveVelocity, rb.velocity.y);
                    //Debug.Log(rb.velocity);
                    break;
                //walking moving object
                case PlayerState.MOVING_OBJECT:
                    rb.velocity = new Vector2(moveVelocity, rb.velocity.y);
                    break;
                //stopped while moving an object
                case PlayerState.MOVING_OBJECT_IDLE:
                    rb.velocity = Vector2.zero;
                    rb.isKinematic = true;
                    break;
                //these are used for when the object you are moving hits a wall
                case PlayerState.MOVING_OBJECT_LEFT:
                    rb.velocity = new Vector2(moveVelocity, rb.velocity.y);
                    break;
                case PlayerState.MOVING_OBJECT_RIGHT:
                    rb.velocity = new Vector2(moveVelocity, rb.velocity.y);
                    break;
            }
            lastPosition = transform.position;
        }
    }


    // Update is called once per frame
    void Update()
    {
        //delay for when player is dropping metal on SuspendedPlatforms
        if (droppingMetal)
        {
            dropMetalTimer += Time.deltaTime;
            if (dropMetalTimer >= dropMetalTime)
            {
                droppingMetal = false;
                dropMetalTimer = 0;
            }
        }
        //delay for when player is jump to non-moving diagonal platforms
        if (jumpFix)
        {
            jumpFixTimer += Time.deltaTime;
            if (jumpFixTimer >= jumpFixTime)
            {
                jumpFix = false;
                jumpFixTimer = 0;
            }
        }

     
        if (!isReading && dialogue != null && dialogue.sentences.Length != 0 && IsPlayerOnGround())
        {
            isReading = true;
            DialogueManager.instance.StartDialogue(dialogue);
            PlayerControlsStatus(false);
            dialogue = null;
        }
        


        //player input
        HandleInput();
    }

    public void SetOnDiagonalPlatform(bool state)
    {
        onDiagonalPlatform = state;
        //worldScale = transform.localScale;
    }

    public bool GetOnDiagonalPlatform()
    {
        return onDiagonalPlatform;
    }

    void HandleInput()
    {
        

        //Debug.Log("Input");
        //for when player is reading tutorials
        if (isReading)
        {
            playerState = PlayerState.IDLE;
            if (Input.GetKeyUp(KeyCode.Return))
            {
                DialogueManager.instance.AdvanceSentence();
                if (!DialogueManager.instance.isOpen)
                {
                    isReading = false;
                    if (finalDialogue)
                    {
                        PlayerPrefs.SetInt("LoadCredits", 1);
                        SceneManager.LoadScene(MainMenuScene, LoadSceneMode.Single);
                    }
                    PlayerControlsStatus(true);
                }
            }
        }
        //movementEnabled is used when menu is open
        else if (movementEnabled && !animationMovement && playerState != PlayerState.JUMP 
                 && playerState != PlayerState.INTERACT && playerState != PlayerState.PLAYER_DEATH_CHARGES_START)
        {
            //Debug.Log("Input2");
            //to open menu
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //Debug.Log("Pause");
                if (pauseMenuUI != null)
                {
                    //Debug.Log("Pause 2");
                    pauseMenuUI.gameObject.SetActive(true);
                }
                PlayerControlsStatus(false);
            }
            //Horizontal move
            if (Input.GetKey(KeyCode.A))
            {
                fallFixSwitch = true;
                //Character flip
                if (playerGFX.GetFacingRight() && !movingMetal && shooting.GetAimLineState() == Shooting.AimLineState.NOT_AIMED)
                {
                    if (playerState != PlayerState.JUMP && playerState != PlayerState.JUMPING &&
                        playerState != PlayerState.FALLING && playerState != PlayerState.JUMP_FALLING &&
                        playerState != PlayerState.MOVING_OBJECT_STOPPED_LEFT)
                    {
                        /*hit an object to the right of the object that the player is moving.
                         In this case it is ok to move left
                         */
                        if (playerState == PlayerState.MOVING_OBJECT_STOPPED_RIGHT)
                        {
                            playerState = PlayerState.MOVING_OBJECT_LEFT;

                            rb.isKinematic = true;
                        }

                        if (playerState == PlayerState.MOVING_OBJECT || playerState == PlayerState.MOVING_OBJECT_IDLE)
                        {
                            movingMetal = true;
                        }
                        //Debug.Log("Input5");
                        playerGFX.SetFacingRight(false); // facing left
                        transform.Rotate(0f, 180f, 0f); //rotate player and aiming to the left 
                        shooting.SetLastShootingLine(-1);
                    }
                }
                else
                {
                    if (playerState == PlayerState.MOVING_OBJECT_IDLE)
                    {
                        if (!playerGFX.GetFacingRight() && movingMetal)
                        {
                            playerState = PlayerState.MOVING_OBJECT;
                            metalMovingEvent.setPaused(false);
                        }
                    }
                }
                if (playerState != PlayerState.JUMP && playerState != PlayerState.JUMPING &&
                    playerState != PlayerState.FALLING && playerState != PlayerState.JUMP_FALLING &&
                    playerState != PlayerState.MOVING_OBJECT_STOPPED_LEFT && playerState != PlayerState.MOVING_OBJECT_END)
                {
                    if (shooting.GetAimLineState() == Shooting.AimLineState.NOT_AIMED)
                    {
                        if ((movingMetal && !playerGFX.GetFacingRight()) || !movingMetal)
                        {
                            rb.isKinematic = false;
                            //changes player speed depending if the player is walking or moving an object
                            if (playerState == PlayerState.MOVING_OBJECT)
                            {
                                moveVelocity = 0f - moveObjectSpeed;
                            }
                            else if (playerState == PlayerState.JUMP || playerState == PlayerState.JUMPING ||
                                     playerState == PlayerState.JUMP_FALLING || playerState == PlayerState.FALLING)
                            {
                                moveVelocity = 0f - jumpMoveSpeed;
                            }
                            else
                            {
                                moveVelocity = 0f - moveSpeed;
                                //Debug.Log("Input3");
                            }
                        }
                    }
                }
                if (playerState == PlayerState.IDLE || playerState == PlayerState.WALKING || playerState == PlayerState.FALL_FIX)
                {
                    //only can move when not aiming
                    if (shooting.GetAimLineState() == Shooting.AimLineState.NOT_AIMED)
                    {
                        if (playerState == PlayerState.FALL_FIX)
                        {
                            onGround = true;
                        }
                        playerState = PlayerState.WALKING;
                    }
                    else
                    {
                        playerState = PlayerState.IDLE;
                    }
                }
            }
            else if (Input.GetKey(KeyCode.D))
            {
                fallFixSwitch = true;

                //Character flip
                if (!playerGFX.GetFacingRight() && !movingMetal && shooting.GetAimLineState() == Shooting.AimLineState.NOT_AIMED)
                {
                    if (playerState != PlayerState.JUMP && playerState != PlayerState.JUMPING &&
                        playerState != PlayerState.FALLING && playerState != PlayerState.JUMP_FALLING &&
                        playerState != PlayerState.MOVING_OBJECT_STOPPED_RIGHT)
                    {
                        /*hit an object to the left of the object that the player is moving.
                         In this case it is ok to move right
                         */
                        if (playerState == PlayerState.MOVING_OBJECT_STOPPED_LEFT)
                        {
                            playerState = PlayerState.MOVING_OBJECT_RIGHT;
                            rb.isKinematic = true;
                        }
                        if (playerState == PlayerState.MOVING_OBJECT || playerState == PlayerState.MOVING_OBJECT_IDLE)
                        {
                            movingMetal = true;
                        }
                        playerGFX.SetFacingRight(true);
                        transform.Rotate(0f, 180f, 0f); //rotate player and aiming to the left
                        shooting.SetLastShootingLine(1);
                        //Debug.Log("Input6");
                    }
                }
                else
                {
                    if (playerGFX.GetFacingRight() && movingMetal)
                    {
                        if (playerState == PlayerState.MOVING_OBJECT_IDLE)
                        {
                            playerState = PlayerState.MOVING_OBJECT;
                            metalMovingEvent.setPaused(false);
                        }
                    }
                }
                if (playerState != PlayerState.JUMP && playerState != PlayerState.JUMPING &&
                    playerState != PlayerState.FALLING && playerState != PlayerState.JUMP_FALLING &&
                    playerState != PlayerState.MOVING_OBJECT_STOPPED_RIGHT && playerState != PlayerState.MOVING_OBJECT_END)
                {
                    if (shooting.GetAimLineState() == Shooting.AimLineState.NOT_AIMED )
                    {
                        if ((movingMetal && playerGFX.GetFacingRight()) || !movingMetal)
                        {
                            rb.isKinematic = false;
                            //changes player speed depending if the player is walking or moving an object
                            if (playerState == PlayerState.MOVING_OBJECT)
                            {
                                moveVelocity = moveObjectSpeed;
                            }
                            else if (playerState == PlayerState.JUMP || playerState == PlayerState.JUMPING ||
                                     playerState == PlayerState.JUMP_FALLING)
                            {
                                moveVelocity = jumpMoveSpeed;
                            }
                            else
                            {
                                moveVelocity = moveSpeed;
                                //Debug.Log("Input4");
                            }
                        }
                    }

                }
                if (playerState == PlayerState.IDLE || playerState == PlayerState.WALKING || playerState == PlayerState.FALL_FIX)
                {
                    //only can move when not aiming
                    if (shooting.GetAimLineState() == Shooting.AimLineState.NOT_AIMED)
                    {
                        if (playerState == PlayerState.FALL_FIX)
                        {
                            onGround = true;
                        }
                        playerState = PlayerState.WALKING;
                    }
                    else
                    {
                        playerState = PlayerState.IDLE;
                    }
                }
            }
            else
            {
                if (playerState == PlayerState.WALKING)
                {
                    playerState = PlayerState.IDLE;
                }
                else if (playerState == PlayerState.FALL_FIX)
                {
                    if (fallFixSwitch)
                    {
                        playerState = PlayerState.IDLE;
                        onGround = true;
                        rb.isKinematic = true;
                        fallFixSwitch = false;
                    }
                }
                else if (playerState == PlayerState.MOVING_OBJECT)
                {
                    playerState = PlayerState.MOVING_OBJECT_IDLE;
                    metalMovingEvent.setPaused(true);
                }
                moveVelocity = 0;
            }

            if (playerState != PlayerState.MOVING_OBJECT && playerState != PlayerState.MOVING_OBJECT_IDLE &&
                playerState != PlayerState.JUMP && playerState != PlayerState.JUMPING &&
                 playerState != PlayerState.JUMP_FALLING && !animationMovement)
            {
                shooting.MouseInputHandle();
            }

            //Jump
            if (Input.GetButtonDown("Jump"))
            {
                //Only can jump if player is on ground and not loaded projectile
                if (!shooting.GetLoadedProjectile() && (
                    playerState == PlayerState.WALKING || playerState == PlayerState.IDLE || playerState == PlayerState.FALL_FIX))
                {
                    if (playerState == PlayerState.FALL_FIX)
                    {
                        onGround = true;
                    }

                    if (onGround)
                    {
                        //jumpFix = true;
                        //rb.isKinematic = true;
                        currentPlayerState = PlayerState.IDLE;
                        playerState = PlayerState.JUMP;
                        //SetAnimationMovement(true);
                        //rb.gravityScale = jumpGravity;
                        //onGround = false;
                        //FMODUnity.RuntimeManager.PlayOneShot(jumpSound, jumpSoundVolume);
                    }
                }
            }
            //Interact
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (charges.GetLightCharges() != 0)
                {
                    if (playerState == PlayerState.FALL_FIX)
                    {
                        onGround = true;
                    }

                    if (onGround && shooting.GetAimLineState() == Shooting.AimLineState.NOT_AIMED)
                    {
                        GameObject comp = PlayerObjectInteractions.playerObjectIInstance.GetCurrentObject();
                        if (comp != null)
                        {
                            if (comp.GetComponent<Collider2D>().CompareTag("Bucket"))
                            {
                                if (!comp.GetComponent<Bucket>().GetTippedOver())
                                {
                                    playerState = PlayerState.KICK_BUCKET_START;
                                    comp.GetComponent<Bucket>().SetInKickingRange();
                                    PlayerObjectInteractions.playerObjectIInstance.SetInteracting(true);
                                }
                            }

                            if (comp.GetComponent<Collider2D>().CompareTag("Lantern") ||
                                comp.GetComponent<Collider2D>().CompareTag("Switch"))
                            {
                                playerState = PlayerState.INTERACT;
                            }
                            
                            if (comp.GetComponent<Collider2D>().CompareTag("Metal"))
                            {
                                if (comp.GetComponent<Metal>().GetMovable())
                                {
                                    if (comp.GetComponent<Metal>().IsMoving())
                                    {
                                        Debug.Log("Was moving and now isn't moving");
                                        movingMetal = false;
                                        playerState = PlayerState.MOVING_OBJECT_END;
                                        metalMovingEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                                        rb.velocity = Vector2.zero;
                                        rb.isKinematic = true;
                                        currentMovingObject = null;
                                        droppingMetal = true;
                                    }
                                    else
                                    {
                                        Debug.Log("Wasn't moving and now is moving");
                                        StartMovingMetal(comp);

                                    }
                                    InteractWithObject();
                                    PlayerObjectInteractions.playerObjectIInstance.SetInteracting(true);
                                }
                            }
                        }
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                PlayerObjectInteractions.playerObjectIInstance.ToggleObjects();
            }
        }
    }


    //-------------------------
    // General player functions
    //-------------------------

    public void StartPlayerOutOfChargesDeath()
    {
        
        playerState = PlayerState.PLAYER_DEATH_CHARGES_START;
        
        
    }


    public void ElectrifyInteract()
    {
        InteractWithObject();
        charges.UseLightCharges();
        PlayerObjectInteractions.playerObjectIInstance.SetInteracting(true);
    }


    public void StartJump()
    {
        if (playerGFX.GetFacingRight())
        {
            rb.velocity = new Vector2(jumpMoveSpeed, jumpForce);
        }
        else
        {
            rb.velocity = new Vector2(-jumpMoveSpeed, jumpForce);
        }
        //SetAnimationMovement(false);
        playerState = PlayerState.JUMPING;
        jumpFix = true;
        rb.isKinematic = false;
        //currentPlayerState = PlayerState.IDLE;
        //playerState = PlayerState.JUMP;
        rb.gravityScale = jumpGravity;
        onGround = false;
        FMODUnity.RuntimeManager.PlayOneShot(jumpSound, jumpSoundVolume);
    }

    public PlayerState GetPlayerState()
    {
        return playerState;
    }
    public void SetPlayerState(PlayerState state)
    {
        playerState = state;

        switch (playerState)
        {
            case PlayerState.IDLE:
            case PlayerState.KICK_BUCKET:
            case PlayerState.MOVING_OBJECT:
                rb.velocity = Vector2.zero;
                rb.isKinematic = true;
                //Debug.Log("Player State: " + rb.velocity);
                currentPlayerState = playerState;
                break;
        }
    }
    //used to make player stop when idle so they don't slide
    public void SetKinematic(bool state)
    {
        rb.isKinematic = state;
    }

    //used to change direction
    public void SetMovingRight(bool state)
    {
        if (state && !playerGFX.GetFacingRight())
        {
            playerGFX.SetFacingRight(true);
            transform.Rotate(0f, 180f, 0f);
            shooting.SetLastShootingLine(1);
        }
        else if (!state && playerGFX.GetFacingRight())
        {
            playerGFX.SetFacingRight(false);
            transform.Rotate(0f, 180f, 0f);
            shooting.SetLastShootingLine(-1);
        }

        if (playerGFX.GetFacingRight())
        {
            moveVelocity = moveSpeed;
        }
        else
        {
            moveVelocity = 0f - moveSpeed;
        }
    }

    public bool IsPlayerOnGround()
    {
        return onGround;
    }

    //private void OnDrawGizmosSelected()
    //{
    //    if (debugMode)
    //    {
    //        //Draw interactable circle
    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawWireSphere(transform.position, interactRadius);
    //    }
    //}

    //----------------------------------
    //Interact with Interactable objects
    //----------------------------------

    public void InteractWithObject()
    {
        GameObject comp = PlayerObjectInteractions.playerObjectIInstance.GetCurrentObject();
        if (comp != null)
        {
            comp.GetComponent<IInteractable>().Interact(); // call interact function
        }
    }

    //----------------------------
    // Animation related functions
    //----------------------------

    // returns if an automated animation is happening
    public bool GetAnimationMovement()
    {
        return animationMovement;
    }
    //set if an automated animation is starting or finishing
    public void SetAnimationMovement(bool state)
    {
        animationMovement = state;
        if (!state)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }
    }

   

    //----------------------
    //Moving Metal Functions
    //----------------------

    //used for when metal hits a door // need to add functionality for any wall
    public void SetMoveObjectStopped()
    {
        if (playerGFX.GetFacingRight())
        {
            playerState = PlayerState.MOVING_OBJECT_STOPPED_RIGHT;

        }
        else
        {
            playerState = PlayerState.MOVING_OBJECT_STOPPED_LEFT;
        }
    }

    //for if metal was hitting a door but isn't anymore so now the player can move the metal // need to add functionality for any wall
    public void SetMoveObject()
    {
        if (playerState == PlayerState.MOVING_OBJECT_STOPPED_LEFT ||
            playerState == PlayerState.MOVING_OBJECT_STOPPED_RIGHT ||
            playerState == PlayerState.MOVING_OBJECT_LEFT || playerState == PlayerState.MOVING_OBJECT_RIGHT)
        {
            playerState = PlayerState.MOVING_OBJECT;
        }
    }

    //Disconnect from metal// might be the problem with falling with metal issue 
    public void SetObjectDisconnected()
    {
        Debug.LogError("Metal Fixed");
        if (currentMovingObject != null)
        {
            currentMovingObject = null;
            if (playerState != PlayerState.FALLING && playerState != PlayerState.JUMP_FALLING && playerState != PlayerState.MOVING_OBJECT_START && playerState != PlayerState.MOVING_OBJECT_END)
            {
                Debug.LogError("Switch to walking");
                playerState = PlayerState.WALKING;
                if (movingMetal)
                {
                    SetAnimationMovement(false);
                    PlayerObjectInteractions.playerObjectIInstance.GetCurrentObject().GetComponent<Metal>().SetMoving(false);
                    movingMetal = false;
                }
            }
        }
    }

    public void SetMovingMetalStop()
    {
        movingMetal = false;
    }

    //return the metal being dragged
    public GameObject GetCurrentMovingObject()
    {
        return currentMovingObject;
    }
    // returns the script of current metal being dragged
    public Metal GetPlayerCurrentMetal()
    {
        return currentPlayerMetal;
    }
    //returns if player is dragging metal
    public bool GetMetalMoving()
    {
        return movingMetal;
    }

    public void SetPlayerMoveMetalOnPlatform()
    {
        SetOnDiagonalPlatform(false);
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
        SetCurrentPlatform(null);
        transform.localScale = new Vector3(1, 1, 1);
    }


    //sets everything to begin dragging metal
    public void StartMovingMetal(GameObject metal)
    {
        if (metal != null)
        {
            playerState = PlayerState.MOVING_OBJECT_START;
            metalMovingEvent.start();
            currentMovingObject = metal;
            currentPlayerMetal = currentMovingObject.GetComponent<Metal>();
            movingMetal = true;
            currentMovingObject.GetComponent<Metal>().SetPickUpMetalDirection();
        }
    }

    //used while player is dropping metal to check for SuspendedPlatform
    public bool IsDroppingMetal()
    {
        return droppingMetal;
    }

    //-----------------------------------------------------
    // Functions that deal with falling, jumping and moving 
    //-----------------------------------------------------

    //called from HopFix to handle the player hopping when moving metal objects
    //only called when the player lifts off the ground greater than the hopping fix distance 
    public void LeavingTheGround()
    {
        if (playerState == PlayerState.MOVING_OBJECT)
        {
            GameObject comp = PlayerObjectInteractions.playerObjectIInstance.GetCurrentObject();
            if (comp != null)
            {
                
                if (comp.GetComponent<Collider2D>().CompareTag("Metal"))
                {
                    //player was dragging metal and is now falling
                    if (comp.GetComponent<Metal>().IsMoving())
                    {
                        comp.GetComponent<Metal>().DisconnectMetalFromPlayer(); // disconnect metal from a fall
                        if (playerGFX.GetFacingRight())
                        {
                            SetMovingRight(false);
                        }
                        else
                        {
                            SetMovingRight(true);
                        }

                        animationMovement = false;
                        movingMetal = false;
                        onGround = false;
                        playerState = PlayerState.FALLING;
                        rb.isKinematic = false;
                        rb.gravityScale = fallGravity;
                        //fallFromMetal = true;
                    }
                }
            }
        }
        else
        {
            //Debug.Log("Jumping");
            onGround = false;

            if (playerState != PlayerState.JUMP && playerState != PlayerState.JUMPING)
            {
                rb.isKinematic = false;
                rb.gravityScale = fallGravity;

                playerState = PlayerState.FALLING;
                //make a 3rd gravity
            }
            else
            {
                //Debug.Log("Jumping 2");
                rb.gravityScale = jumpGravity;
            }
        }
    }


    //Set from PlayerCollision collider to set when the player is on the ground 
    public bool GetOnGround()
    {
        return onGround;
    }

    //called from PlayerCollision to set if player is on ground after a jump or fall
    //also set from Shooting
    public void SetOnGround(bool state)
    {
        //could be causing the falling with metal problem
        //if (!fallFromMetal)
        //{
        //rb.velocity = Vector2.zero;
        //rb.isKinematic = true;
        onGround = state;
        //player state when landing on ground is different depending on what the player state was before falling
        if (currentPlayerState == PlayerState.MOVING_OBJECT_IDLE || currentPlayerState == PlayerState.MOVING_OBJECT ||
            currentPlayerState == PlayerState.MOVING_OBJECT_END)
        {
            playerState = PlayerState.IDLE;
        }
        else
        {
            playerState = currentPlayerState;
        }
        rb.gravityScale = groundGravity;
        //}
    }
    //for when the player is jumping to non-moving diagonal platforms
    public void SetOnGroundJumpFix(bool state)
    {
        onGround = state;
    }
    
    public bool GetFalling()
    {
        if (playerState == PlayerState.FALLING || playerState == PlayerState.JUMP_FALLING)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //is the jump delay over for jumping to diagonal non moving platforms
    public bool GetJumpFix()
    {
        return jumpFix;
    }
    //called from SuspendedPlatform when it moves so the player can fall from the platform
    public void SetFallFix()
    {
        currentPlayerState = playerState;
        playerState = PlayerState.FALL_FIX;
        rb.isKinematic = false;
        fallFixTimer = 0;
        onGround = false;
    }

    //-------------
    // UI functions
    //-------------
    public void ClosePauseMenu() {
        if(pauseMenuUI != null)
        {
            pauseMenuUI.gameObject.SetActive(false);
        }
        PlayerControlsStatus(true);
    }
    public void ReferencePauseMenuUI(Canvas pauseMenu)
    {
        pauseMenuUI = pauseMenu;
    }
    public void ReferenceSetingsMenuUI(Canvas settingsMenu)
    {
        settingsMenuUI = settingsMenu;
        settingsMenuUI.gameObject.SetActive(false);
    }

    public void PlayerControlsStatus(bool status)
    {
        movementEnabled = status;
    }
    public void DestroyUI()
    {
        Destroy(pauseMenuUI.gameObject);
        Destroy(settingsMenuUI.gameObject);
    }

    public bool GetMovementEnabled()
    {
        return movementEnabled;
    }
}