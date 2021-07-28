using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
[DefaultExecutionOrder(-100)] //ensure this script runs before all other player scripts to prevent laggy input
public class Player : MonoBehaviour
{
    //to merge
    private CheckPointSystem checkPointDeathSystem = null;

    [SerializeField] private Charges charges;
    [SerializeField] private Shooting shooting;
    

    //CapsuleCollider2D capsuleCollider2D = null; //Player's capsule collider

    [Header("General")]
    //[SerializeField] private bool 
    Rigidbody2D rb = null; //player's rigid body

    [SerializeField] private PlayerGFX playerGFX;
    [SerializeField] private PlayerCollision playerCollision;

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
        INTERACT

    }

    [SerializeField] private PlayerState playerState = PlayerState.WALKING;
    public PlayerState PlayersState => playerState;
    [SerializeField] bool debugMode = false;

    [Header("Movement")]
    [SerializeField] private bool onGround = false; // keeps track if player is  on ground

    private Vector3 lastPosition; // used to store the players position each frame
    [SerializeField] private float moveSpeed = 4.0f; // regular speed of the player
    [SerializeField] private float jumpMoveSpeed = 8.0f; // jump speed of the player
    [SerializeField] private float moveObjectSpeed = 3.0f; // speed of the player moving while moving an object
    [SerializeField] private float moveVelocity = 0; //


    //changes in gravity
    [SerializeField] private float jumpGravity = 1f; // gravity while jumping
    [SerializeField] private float fallGravity = 5f; // gravity while falling
    [SerializeField] private float groundGravity = 10f; // gravity while the player is walking on a surface tagged ground 

    //variable for jump and fall from jump
    [SerializeField] private float jumpForce = 10f; //How strong does player jump
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float fallMultiplier = 4f;

    private bool jumpFix = false;
    private float jumpFixTimer = 0;
    private float jumpFixTime = .1f;



    //variables used for when player is on moving platforms to make the player fall and move with the platform
    [SerializeField] private PlayerState currentPlayerState;
    [SerializeField] private float fallFixTimer = 0;
    [SerializeField] private float fallFixMax = .1f;
    [SerializeField] private float fallFixStayMax = 2f;
    private bool fallFixSwitch = false;

    [SerializeField] private bool fallFromMetal = false;

    //ConstraintSource constraintSource;
    //ParentConstraint parentConstraint;

    private bool onPlatform = false;
    private bool onPlatformRotation = true;

    private float rotationTimer = 0;
    private float rotationTime = .5f;

    private bool droppingMetal = false;
    private float dropMetalTimer = 0;
    private float dropMetalTime = 2f;

    


    //FMOD Event Refs
    [FMODUnity.EventRef]
    public string jumpSound;

    [Header("Interact")]
    //stores object the player is currently moving 
    [SerializeField] private GameObject currentMovingObject;

    [SerializeField] private Metal currentPlayerMetal;


    //[SerializeField] float interactRadius = 5f;
    //[SerializeField] LayerMask interactLayer;

    //[Header("Kite")]
    //[SerializeField] private Kite kite;
    //public UnityEvent PlayerMovingHorizontallyEvent;
    //public UnityEvent PlayerMovingVerticallyEvent;
    //[SerializeField] private float playerDistMovedX;
    //[SerializeField] private float playerDistMovedY;

    [Header("Animation")] 
    private bool animationMovement = false;
    private bool animationMovingRight = false;

    [SerializeField] private bool movingMetal = false;

    [SerializeField] private bool movingMetalDirectionFix = false;
    //private bool animationSwitch = false;


    [SerializeField] private GameObject pickUpMetalPos;


    [Header("Menus")]
    [SerializeField] private Canvas pauseMenuUI = null;
    private Canvas settingsMenuUI = null;
    private bool movementEnabled = true;

    public void AddParentConstraint(GameObject gO)
    {
        //parentConstraint = gameObject.GetComponent<ParentConstraint>();
       //constraintSource.sourceTransform = gO.transform;
       //parentConstraint.AddSource(constraintSource);
       
        

        //mySource.sourceTransform = m_RealObjOnPlane.transform;

    }

    public bool GetJumpFix()
    {
        return jumpFix;
    }


    public void SetOnPlatform(bool state)
    {
        onPlatform = state;
    }

    public void SetRotationPlatformFix()
    {
        onPlatformRotation = true;
    }



    public void RemoveParentConstraint(GameObject gO)
    {
        //constraintSource.sourceTransform = null;
        //parentConstraint.RemoveSource(0);
    }

    public Vector2 GetMetalPickUpPos()
    {
        return pickUpMetalPos.transform.position;
    }


    public bool GetAnimationMovement()
    {
        return animationMovement;
    }

    //public bool GetAnimationMovingRight()
    //{
    //    return animationMovingRight;
    //}

    //public bool GetAnimationSwitch()
    //{
    //    return animationSwitch;
    //}

    //public void SetAnimationSwitch(bool state)
    //{
    //    animationSwitch = state;
    //}


    public void SetAnimationMovement(bool state)
    {
        //Debug.Log("Set Movement Animation: " + state);
        animationMovement = state;
        if (!state)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
            //Debug.Log("Player State: " + rb.velocity);
        }
        //else
        //{
        //    if (playerState == PlayerState.MOVING_OBJECT_START)
        //    {

        //    }
        //}
    }


    public bool GetMetalMoving()
    {
        //Debug.Log("Moving Metal: " + movingMetal);
        return movingMetal;
    }

    //public void MetalFacingFix(bool state)
    //{
        //if (state)
        //{
        //    if (playerGFX.GetFacingRight())
        //    {
        //        transform.Rotate(0f, 180f, 0f);
        //        shooting.SetLastShootingLine(-1);
        //    }
        //    else
        //    {
        //        transform.Rotate(0f, 180f, 0f);
        //        shooting.SetLastShootingLine(1);
        //    }
        //}
        //else
        //{
        //    if (playerGFX.GetFacingRight())
        //    {
        //        transform.Rotate(0f, 180f, 0f);
        //        shooting.SetLastShootingLine(1);
        //    }
        //    else
        //    {
        //        transform.Rotate(0f, 180f, 0f);
        //        shooting.SetLastShootingLine(-1);
        //    }
        //}
    //}

    //public void OtherMetalFacingFix()
    //{
    //    Debug.Log("Y Angle: " + transform.eulerAngles.y);
    //    if (transform.eulerAngles.y < 181 && transform.eulerAngles.y > 179)
    //    {
    //        shooting.SetLastShootingLine(1);
    //    }
    //    else
    //    {
    //        shooting.SetLastShootingLine(-1);
    //    }
    //}

    public void SetMovingRight(bool state)
    {
        //animationMovingRight = state;
        if (state && !playerGFX.GetFacingRight())
        {
            Debug.Log("Going Right");
            playerGFX.SetFacingRight(true);
            transform.Rotate(0f, 180f, 0f);
            shooting.SetLastShootingLine(1);
        }
        else if (!state && playerGFX.GetFacingRight())
        {
            Debug.Log("Going Left");
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


    public bool GetMovementEnabled()
    {
        return movementEnabled;
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
                Debug.Log("Player State: " + rb.velocity);
                currentPlayerState = playerState;
                break;
        }
    }

    public void SetKinematic(bool state)
    {
        rb.isKinematic = state;
    }

    public void DestroyUI()
    {
        Destroy(pauseMenuUI.gameObject);
        Destroy(settingsMenuUI.gameObject);
    }

    //public float GetDistPlayerMoveX()
    //{
    //    return playerDistMovedX;
    //}

    //public float GetDistPlayerMoveY()
    //{
    //    return playerDistMovedY;
    //}


    // Start is called before the first frame update
    void Start()
    {
        //reference checkpoint + death system script 
        checkPointDeathSystem = GameObject.Find("GlobalGameController").GetComponent<CheckPointSystem>();

        rb = GetComponent<Rigidbody2D>(); // player rigidbody

        shooting.SetAimLineState(Shooting.AimLineState.NOT_AIMED);
        playerGFX.SetFacingRight(true);

        // setting some generally player movement variables
        playerState = PlayerState.JUMPING;
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
        Debug.Log("Y Angle: " + transform.eulerAngles.y);
        //kite.SetKiteStartPosition(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        //player input
        HandleInput();

        //if there are no charges left then player has died
        //if (charges.GetLightCharges() == 0 && !shooting.GetLoadedProjectile() && !charges.GetLampOn() && shooting.GetCanShoot())
        //{

        //    FindObjectOfType<ObjectsCollision>().EmptyObjects();
        //    checkPointDeathSystem.PlayerDeath();
        //}
    }




    private void FixedUpdate()
    {
        if (animationMovement)
        {
            //Debug.Log("Animation Movement");
            if (playerState != PlayerState.MOVING_OBJECT)
            {
                rb.isKinematic = false;
            }

            switch (playerState)
            {
                case PlayerState.KICKING_BUCKET:
                    //Debug.Log("Animation: " + playerState);
                    rb.velocity = new Vector2(moveVelocity, rb.velocity.y);
                    break;
                case PlayerState.MOVING_OBJECT_START:
                    rb.velocity = new Vector2(moveVelocity, rb.velocity.y);
                    break;
            }
        }
        //else if (animationSwitch)
        //{
        //    if ( playerState != PlayerState.MOVING_OBJECT_SWITCH_START &&
        //        playerState != PlayerState.MOVING_OBJECT_SWITCH_END)
        //    {

        //        if (playerGFX.GetFacingRight())
        //        {
        //            moveVelocity = moveSpeed;
        //        }
        //        else
        //        {
        //            moveVelocity = 0f - moveSpeed;
        //        }

        //        //Debug.Log("switch move" + moveVelocity);
        //        rb.isKinematic = false;
        //        rb.velocity = new Vector2(moveVelocity, rb.velocity.y);
        //    }
        //}

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
                    if (playerGFX.GetFacingRight())
                    {
                        rb.velocity = new Vector2(jumpMoveSpeed, jumpForce);
                    }
                    else
                    {
                        rb.velocity = new Vector2(-jumpMoveSpeed, jumpForce);
                    }

                    //rb.velocity = jumpForce;
                    //rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier * 1) * Time.deltaTime;
                    //rb.velocity = new Vector2(jumpMoveSpeed, rb.velocity.y);
                    playerState = PlayerState.JUMPING;
                    break;
                //in the air of the jump
                case PlayerState.JUMPING:
                    if (rb.velocity.y > 0 /*&& !Input.GetButton("Jump")*/)
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

                            //rb.velocity = new Vector2(-jumpMoveSpeed, rb.velocity.y);
                        }
                    }
                    else
                    {
                        playerState = PlayerState.JUMP_FALLING;
                    }
                    break;
                //falling from a jump
                case PlayerState.JUMP_FALLING:
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
                case PlayerState.FALLING:
                    //falling but not from a jump

                    if (moveVelocity != 0)
                    {
                        rb.velocity = new Vector2(moveVelocity, rb.velocity.y);
                    }
                    else
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                    }

                    break;
                //state for when player is on a moving platform
                case PlayerState.FALL_FIX:
                    //Debug.Log("Movement Fix");
                    if (fallFixTimer >= fallFixStayMax)
                    {
                        rb.isKinematic = false;
                        playerState = currentPlayerState;
                        //Debug.Log("Walk problem");
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
            //playerDistMovedX = lastPosition.x - transform.position.x;
            //if ((Mathf.Abs(playerDistMovedX) > Mathf.Epsilon) && playerDistMovedX <= 1)
            //{

            //    PlayerMovingHorizontallyEvent.Invoke();
            //    //kite.MoveKiteWithPlayer(-distX);
            //}
            //playerDistMovedY = lastPosition.y - transform.position.y;
            //if ((Mathf.Abs(playerDistMovedY) > Mathf.Epsilon) && playerDistMovedY <= 1)
            //{
            //    PlayerMovingVerticallyEvent.Invoke();
            //    //kite.MoveKiteWithPlayer(-distX);
            //}


            lastPosition = transform.position;
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

    public bool IsDroppingMetal()
    {
        return droppingMetal;
    }

    void HandleInput()
    {
        if (droppingMetal)
        {
            dropMetalTimer += Time.deltaTime;
            if (dropMetalTimer >= dropMetalTime)
            {
                droppingMetal = false;
                dropMetalTimer = 0;
            }
        }

        if (jumpFix)
        {
            jumpFixTimer += Time.deltaTime;
            if (jumpFixTimer >= jumpFixTime)
            {
                jumpFix = false;
                jumpFixTimer = 0;
            }
        }

        

        //if (onPlatformRotation)
        //{
        //    Debug.Log("Rotation Problem");
        //    Vector3 fixRotation = transform.eulerAngles;
        //    fixRotation.z = 0;
        //    transform.eulerAngles = fixRotation;
        //    onPlatformRotation = false;
        //}

        //if (rotationTimer >= rotationTime)
        //{
        //    Debug.Log("Rotation Problem");
        //    Vector3 fixRotation = transform.eulerAngles;
        //    fixRotation.z = 0;
        //    transform.eulerAngles = fixRotation;
        //    onPlatformRotation = false;
        //    rotationTimer = 0;
        //}

        rotationTimer += Time.deltaTime;
        //player.transform.rotation = Quaternion.AngleAxis(0, fixRotation);

        //if (fallFixTimer >= fallFixMax)
        //{
        //    player.SetFallFix();
        //}
        //fallFixTimer += Time.deltaTime;

        if (movementEnabled && !animationMovement)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Pause");
                if (pauseMenuUI != null)
                {
                    Debug.Log("Pause 2");
                    pauseMenuUI.gameObject.SetActive(true);
                    //pauseMenuUI.gameObject.GetComponent<PauseMenu>().PauseMenuCalled();
                }
                PlayerControlsStatus(false);
            }
            //Horizontal move
            if (Input.GetKey(KeyCode.A))
            {
                fallFixSwitch = true;
                //Character flip
                if (playerGFX.GetFacingRight() && !movingMetal)
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
                            //animationMovement = true;
                            //animationSwitch = true;
                            
                            rb.isKinematic = true;
                            //playerGFX.PutMetalDownSwitch();
                        }

                        if (playerState == PlayerState.MOVING_OBJECT || playerState == PlayerState.MOVING_OBJECT_IDLE)
                        {
                            //MetalFacingFix(true);
                            movingMetal = true;
                        }
                        //Debug.Log("Falling Left" + playerState);
                        playerGFX.SetFacingRight(false); // facing left
                        transform.Rotate(0f, 180f, 0f); //rotate player and aiming to the left 
                        shooting.SetLastShootingLine(-1);
                    }
                    //if (playerState == PlayerState.MOVING_OBJECT || playerState == PlayerState.MOVING_OBJECT_IDLE)
                    //{
                    //    playerState = PlayerState.MOVING_OBJECT_SWITCH_START;
                    //    rb.velocity = Vector2.zero;
                    //    rb.isKinematic = true;
                    //    animationSwitch = true;
                    //    //animationMovement = true;
                    //    //rb.isKinematic = true;
                    //    //playerGFX.PutMetalDownSwitch();
                    //}
                }
                else
                {
                    if (playerState == PlayerState.MOVING_OBJECT_IDLE)
                    {
                        if (!playerGFX.GetFacingRight() && movingMetal)
                        {
                            //Debug.Log("Fall Problem");
                            playerState = PlayerState.MOVING_OBJECT;
                        }
                    }

                }

                //if (!animationSwitch)
                //{
                    //if (playerState != PlayerState.MOVING_OBJECT_END)
                    //{
                    //    rb.isKinematic = false;
                    //}
                //}

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
                                //if (movingMetalDirectionFix)
                                //{
                                //    MetalFacingFix(true);
                                //    movingMetalDirectionFix = false;
                                //}
                                
                                moveVelocity = 0f - moveSpeed;
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
                        // Debug.Log("Walk problem");
                        playerState = PlayerState.WALKING;
                    }
                    else
                    {
                        //Debug.Log("Fall Problem 3");
                        playerState = PlayerState.IDLE;
                    }
                }
            }
            else if (Input.GetKey(KeyCode.D))
            {
                fallFixSwitch = true;

                //Character flip
                if (!playerGFX.GetFacingRight() && !movingMetal)
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
                            // playerGFX.PutMetalDownSwitch();
                        }
                        if (playerState == PlayerState.MOVING_OBJECT || playerState == PlayerState.MOVING_OBJECT_IDLE)
                        {
                            //MetalFacingFix(true);
                            movingMetal = true;
                        }
                        //Debug.Log("Falling Right" + playerState);
                        playerGFX.SetFacingRight(true);
                        transform.Rotate(0f, 180f, 0f); //rotate player and aiming to the left
                        shooting.SetLastShootingLine(1);
                    }
                    //if (playerState == PlayerState.MOVING_OBJECT || playerState == PlayerState.MOVING_OBJECT_IDLE)
                    //{
                    //    playerState = PlayerState.MOVING_OBJECT_SWITCH_START;
                    //    rb.velocity = Vector2.zero;
                    //    rb.isKinematic = true;

                    //    animationSwitch = true;
                    //    //animationMovement = true;
                    //   //rb.isKinematic = true;
                    //    //playerGFX.PutMetalDownSwitch();
                    //}
                }
                else
                {
                    if (playerGFX.GetFacingRight() && movingMetal)
                    {
                        if (playerState == PlayerState.MOVING_OBJECT_IDLE)
                        {
                            //Debug.Log("Fall Problem");
                            playerState = PlayerState.MOVING_OBJECT;
                        }
                    }
                }

                //if (!animationSwitch)
                //{
                //    if (playerState != PlayerState.MOVING_OBJECT_END)
                //    {
                //        rb.isKinematic = false;
                //    }
                //}

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
                                //if (movingMetalDirectionFix)
                                //{
                                //    MetalFacingFix(true);
                                //    movingMetalDirectionFix = false;
                                //}
                                moveVelocity = moveSpeed;
                            }
                        }
                    }

                }
                //rb.constraints = RigidbodyConstraints2D.None;
                if (playerState == PlayerState.IDLE || playerState == PlayerState.WALKING || playerState == PlayerState.FALL_FIX)
                {
                    //only can move when not aiming
                    if (shooting.GetAimLineState() == Shooting.AimLineState.NOT_AIMED)
                    {
                        if (playerState == PlayerState.FALL_FIX)
                        {
                            onGround = true;
                        }
                        //Debug.Log("Walk problem");
                        playerState = PlayerState.WALKING;
                    }
                    else
                    {
                        //Debug.Log("Fall Problem");
                        playerState = PlayerState.IDLE;
                    }
                }
            }
            else
            {
                if (playerState == PlayerState.WALKING)
                {
                    //Debug.Log("Fall Problem 2");
                    playerState = PlayerState.IDLE;
                }
                else if (playerState == PlayerState.FALL_FIX)
                {
                    if (fallFixSwitch)
                    {
                        //Debug.Log("Fall Problem");
                        playerState = PlayerState.IDLE;
                        onGround = true;
                        rb.isKinematic = true;
                        fallFixSwitch = false;
                    }
                }
                else if (playerState == PlayerState.MOVING_OBJECT)
                {
                    //Debug.Log("Fall Problem");
                    playerState = PlayerState.MOVING_OBJECT_IDLE;
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
                        jumpFix = true;
                        rb.isKinematic = false;
                        //SoundManager.instance.PLaySE(JumpSound);
                        //shouldJump = true;
                        currentPlayerState = PlayerState.IDLE;
                        playerState = PlayerState.JUMP;
                        rb.gravityScale = jumpGravity;
                        onGround = false;
                        FMODUnity.RuntimeManager.PlayOneShot(jumpSound);
                        //falling = false;
                    }
                }
            }

            //if (!animationSwitch)
            //{
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
                                playerState = PlayerState.KICK_BUCKET_START;
                                comp.GetComponent<Bucket>().SetInKickingRange();
                            }

                            if (comp.GetComponent<Collider2D>().CompareTag("Lantern") ||
                                comp.GetComponent<Collider2D>().CompareTag("Switch"))
                            {
                                InteractWithObject();
                                charges.UseLightCharges();
                            }
                            
                            if (comp.GetComponent<Collider2D>().CompareTag("Metal"))
                            {
                                if (comp.GetComponent<Metal>().GetMovable())
                                {
                                    if (comp.GetComponent<Metal>().IsMoving())
                                    {
                                        //if(!movingMetal)
                                        //{
                                        //    movingMetalDirectionFix = true;
                                        //}
                                        Debug.Log("Was moving and now isn't moving");
                                        movingMetal = false;
                                        playerState = PlayerState.MOVING_OBJECT_END;
                                        rb.velocity = Vector2.zero;
                                        rb.isKinematic = true;
                                        currentMovingObject = null;
                                        //currentPlayerMetal = null;
                                        droppingMetal = true;


                                        //if (pickUpMetalPos.transform.position.x > transform.position.x)
                                        //{
                                        //    playerGFX.SetFacingRight(true);
                                        //}
                                        //else 
                                        //{
                                        //    playerGFX.SetFacingRight(false);
                                        //}
                                        //rb.gravityScale = groundGravity;
                                    }
                                    else
                                    {
                                        Debug.Log("Wasn't moving and now is moving");
                                        StartMovingMetal(comp);
                                        //rb.gravityScale = jumpGravity;

                                    }
                                    InteractWithObject();
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
            //}
        }
    }

    public void StartMovingMetal(GameObject metal)
    {
        playerState = PlayerState.MOVING_OBJECT_START;
        currentMovingObject = metal;
        currentPlayerMetal = currentMovingObject.GetComponent<Metal>();
        movingMetal = true;
        currentMovingObject.GetComponent<Metal>().SetPickUpMetalDirection();
    }
    //public void PutDownMetal(GameObject metal)
    //{
    //    Debug.Log("Metal Move1");
    //    playerState = PlayerState.MOVING_OBJECT_START;
    //    currentMovingObject = metal;
    //    currentMovingObject.GetComponent<Metal>().SetPickUpMetalDirection();
    //}


    public void InteractWithObject()
    {
        GameObject comp = PlayerObjectInteractions.playerObjectIInstance.GetCurrentObject(); 
        comp.GetComponent<IInteractable>().Interact(); // call interact function
    }


    //Set from PlayerCollision collider to set when the player is on the ground 
    public bool GetOnGround()
    {
        return onGround;
    }

    public void SetOnGround(bool state)
    {
        //Debug.Log("ON GROUND");
        //if (!fallFromMetal)
        //{
        onGround = state;

        playerState = currentPlayerState;
        rb.gravityScale = groundGravity;
        //}
        //capsuleCollider2D.sharedMaterial.friction = onGroundFriction;
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

    //called from MovingObjectsCollision to handle the player hopping when moving metal objects
    public void LeavingTheGround()
    {
        if (playerState == PlayerState.MOVING_OBJECT)
        {
            //playerState = PlayerState.WALKING;
            GameObject comp = PlayerObjectInteractions.playerObjectIInstance.GetCurrentObject();
            if (comp != null)
            {
                if (comp.GetComponent<Collider2D>().CompareTag("Metal"))
                {
                    if (comp.GetComponent<Metal>().IsMoving())
                    {
                        Debug.Log("Falling off touching an object");
                        //comp.GetComponent<IInteractable>().Interact();
                        comp.GetComponent<Metal>().DisconnectMetalFromPlayer();
                        if (playerGFX.GetFacingRight())
                        {
                            Debug.Log("right to left");
                            SetMovingRight(false);
                        }
                        else
                        {
                            Debug.Log("left to right");
                            SetMovingRight(true);
                        }

                        animationMovement = false;
                        movingMetal = false;
                        //MetalFacingFix(true);
                        onGround = false;
                        playerState = PlayerState.FALLING;
                        rb.isKinematic = false;
                        rb.gravityScale = fallGravity;
                        //capsuleCollider2D.sharedMaterial.friction = inAirFriction;
                        fallFromMetal = true;
                        //playerCollision.SetFallWait();
                        //Vector3 fixRotation = transform.eulerAngles;
                        //fixRotation.z = 0;
                        //transform.eulerAngles = fixRotation;


                    }
                }
            }
        }
        else
        {
            Debug.Log("Jumping");
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
                Debug.Log("Jumping 2");
                rb.gravityScale = jumpGravity;
            }
            //capsuleCollider2D.sharedMaterial.friction = inAirFriction;
        }
    }

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

    public void SetMoveObject()
    {
        if (playerState == PlayerState.MOVING_OBJECT_STOPPED_LEFT ||
            playerState == PlayerState.MOVING_OBJECT_STOPPED_RIGHT ||
            playerState == PlayerState.MOVING_OBJECT_LEFT || playerState == PlayerState.MOVING_OBJECT_RIGHT)
        {
            playerState = PlayerState.MOVING_OBJECT;
        }
    }

    public void SetObjectDisconnected()
    {
        //Debug.Log("Metal Exiting");
        if (currentMovingObject != null)
        {
            currentMovingObject = null;
            if (playerState != PlayerState.FALLING && playerState != PlayerState.JUMP_FALLING)
            {
                //Debug.Log("Walk problem");
                playerState = PlayerState.WALKING;
            }
        }
    }
    public GameObject GetCurrentMovingObject()
    {
        return currentMovingObject;
    }

    public Metal GetPlayerCurrentMetal()
    {
        return currentPlayerMetal;
    }


    public void SetFallFix()
    {
        //Debug.Log("FALL FIX 2" + playerState);
        currentPlayerState = playerState;
        playerState = PlayerState.FALL_FIX;
        rb.isKinematic = false;
        fallFixTimer = 0;
        onGround = false;
    }

    //public PlayerState GetPlayerState()
    //{
    //    return playerState;
    //}
    //public void SetPlayerState(PlayerState state)
    //{
    //    playerState = state;
    //}

    //public AimLineState GetAimLineState()
    //{
    //    return aimLineState;
    //}

    //public void SetKinematic(bool state)
    //{
    //    rb.isKinematic = false;
    //    rb.gravityScale = groundGravity;
    //}

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
        //pauseMenuUI.gameObject.SetActive(true);
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
}