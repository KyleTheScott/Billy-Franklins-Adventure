using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.UI;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
[DefaultExecutionOrder(-100)] //ensure this script runs before all other player scripts to prevent laggy input
public class Player : MonoBehaviour
{

    private CheckPointSystem checkPointDeathSystem = null;



    //CapsuleCollider2D capsuleCollider2D = null; //Player's capsule collider

    [Header("General")]
    //[SerializeField] private bool 
    Rigidbody2D rb = null; //player's rigid body
    [SerializeField] private SpriteRenderer playerSprite;
    private Animator animator;
    bool isFacingRight = false; //Is character facing right side? for Characte flip
    public enum PlayerState
    {
        IDLE,
        JUMP,
        JUMPING,
        JUMP_FALLING,
        FALLING,
        FALL_FIX,
        WALKING,
        MOVING_OBJECT,
        MOVING_OBJECT_IDLE,
        MOVING_OBJECT_STOPPED_LEFT,
        MOVING_OBJECT_LEFT,
        MOVING_OBJECT_STOPPED_RIGHT,
        MOVING_OBJECT_RIGHT

    }

    [SerializeField] private PlayerState playerState = PlayerState.WALKING;
    [SerializeField] bool debugMode = false;

    [Header("Movement")]
    [SerializeField] private bool onGround = false; // keeps track if player is  on ground
    private Vector3 lastPosition; // used to store the players position each frame
    [SerializeField] private float moveSpeed = 4.0f; // regular speed of the player
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

    //variables used for when player is on moving platforms to make the player fall and move with the platform
    [SerializeField] private PlayerState currentPlayerState;
    [SerializeField] private float fallFixTimer = 0;
    [SerializeField] private float fallFixMax = .1f;
    [SerializeField] private float fallFixStayMax = 2f;
    private bool fallFixSwitch = false;

    [Header("Shooting")]
    public float shootCoolTime = 0.5f; //Projectile shoot cool time
    public int hp = 10;
    public int lightCharges = 3; //how many lighting can character use?
    public int maxLightCharges = 3; //how many lighting can character use?
    [SerializeField] private bool canShoot = true; //Check if player can shoot projectile
   
    public int maxNumOfProjectile = 10; //Max number of projectile
    public GameObject projectilePrefab = null; //Prefab for projectile
    [SerializeField] private List<Projectile> listOfProjectile = null;
    
    [SerializeField] float projectileSpawnDistance = 1f; //How far is projectile spanwed from player?
    Projectile loadedProjectile = null; //projectile that is wating for shooting
    //current charges, max charges
    public UnityEvent<int, int> onLightChargesChanged; //DarkBOrder will subscribe, charges text ui will subscribe this

    //[SerializeField] LayerMask tileLayerMask; //Used to check if player is on ground

    // to Get SFX sound name 
    [SerializeField] private string ShootSound;
    [SerializeField] private string JumpSound;


    //variables to show lightning
    [SerializeField] private float angleBetween;
    [SerializeField] private Lightning lightning;
    private Vector2 lightningStartPos;
    private Vector2 lightningTargetPos;

    [SerializeField] private bool lampOn = false; 


    [Header("Aiming")]
   AimLine aimLine = null; //player's aiming line
   Camera mainCam = null; // used for aiming
   Vector3 shootingLine = new Vector3(1, 0, 0); //Direction for loaded projectile
   Vector3 lastShootingLine = new Vector3(1, 0, 0);
   bool IsShootingLineInAngle = false;
   [SerializeField] LayerMask aimLineCollisionMask; //should be tile or obstacle
   [SerializeField] Transform aimCone = null;

    [SerializeField] private bool firstAiming = true;

    public enum AimLineState
    {
        NOT_AIMED,
        AIMING
    };

    [SerializeField] private AimLineState aimLineState = AimLineState.NOT_AIMED;



    //variable for aim line disappearing and appearing with mouse movement
    private bool mouseClick;
    float timeLeft = 1;
    float visibleCursorTimer = .5f;
    float cursorPosition;
    bool catchCursor = true;

    [SerializeField] private float shootFixTimer = .5f;

   



    [Header("Interact")]
    //stores object the player is currently moving 
    [SerializeField] private GameObject currentMovingObject;
    //[SerializeField] float interactRadius = 5f;
    //[SerializeField] LayerMask interactLayer;


    public void SetLampOn(bool state)
    {
        lampOn = state;
    }




    // Start is called before the first frame update
    void Start()
    {
        //reference checkpoint + death system script 
        checkPointDeathSystem = GameObject.Find("GlobalGameController").GetComponent<CheckPointSystem>();

        rb = GetComponent<Rigidbody2D>(); // player rigidbody
        //capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        //animator = GetComponentInChildren<Animator>();

        //Create list for projectile pool
        listOfProjectile = new List<Projectile>();
        for (int i = 0; i < maxNumOfProjectile; ++i)
        {
            GameObject projectile = Instantiate(projectilePrefab);

            //Set owner of this projectile
            projectile.GetComponent<Projectile>().owner = this;

            //Add to pool
            listOfProjectile.Add(projectile.GetComponent<Projectile>());
        }
        //aiming
        mainCam = Camera.main;
        aimLine = GetComponentInChildren<AimLine>();
        aimLineState = AimLineState.NOT_AIMED;

        // setting some generally player movement variables
        isFacingRight = true;
        playerState = PlayerState.JUMPING;
        transform.Rotate(0f, 180f, 0f);

    }

    // Update is called once per frame
    void Update()
    {
        //player input
        HandleInput();

        //if there are no charges left then player has died
        if (lightCharges == 0 && loadedProjectile == null && !lampOn && canShoot)
        {
            
            checkPointDeathSystem.PlayerDeath();
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

    private void FixedUpdate()
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
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                playerState = PlayerState.JUMPING;
                break;
            //in the air of the jump
            case PlayerState.JUMPING:
                if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
                {
                    rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier * 1) * Time.deltaTime;
                    rb.velocity = new Vector2(moveVelocity, rb.velocity.y);
                }
                else
                {
                    playerState = PlayerState.JUMP_FALLING;
                }
                break;
            //falling from a jump
            case PlayerState.JUMP_FALLING:
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier * 1) * Time.deltaTime;
                rb.velocity = new Vector2(moveVelocity, rb.velocity.y);
                break;
            //falling but not from a jump
            case PlayerState.FALLING:
                rb.velocity = new Vector2(moveVelocity, rb.velocity.y);
                break;
            //state for when player is on a moving platform
            case PlayerState.FALL_FIX:
                if (fallFixTimer >= fallFixStayMax)
                {
                    rb.isKinematic = false;
                    playerState = PlayerState.WALKING;
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
        lastPosition = transform.position;
    }


    void ShootProjectile()
    {
        ////Get projectile from list
        //if (listOfProjectile.Count != 0)
        //{
        //    Projectile projectile = listOfProjectile.Dequeue();

        //    //Activate projectile
        //    projectile.gameObject.SetActive(true);

        //    //Set projectile in front of player
        //    Vector3 forwardVec = -transform.right;
        //    Vector3 upwardVec = transform.up;
        //    projectile.transform.position = transform.position + (forwardVec * projectileSpawnXOffset) + (upwardVec * projectileSpawnYOffset);

        //    //Set projectile move direction
        //    projectile.SetProjectileDirection(forwardVec);

        //    //Can't shoot projectile continousely
        //    canShoot = false;
        //    Invoke("ResetShootCoolDown", shootCoolTime);
        //}
    }

    void ResetShootCoolDown()
    {
        canShoot = true;
    }

    //Return projectile to pool
    public void ReturnProjectile(Projectile projectile)
    {
        listOfProjectile.Add(projectile);
    }

    public bool IsPlayerOnGround()
    {
        return onGround;
        ////Do capsule cast to downward of player so that it checks if player is on ground
        //RaycastHit2D result = Physics2D.CapsuleCast(capsuleCollider2D.bounds.center, capsuleCollider2D.bounds.size,
        //    CapsuleDirection2D.Vertical, 0f, Vector2.down, 0.1f, tileLayerMask);

        ////Debug.Log(result.collider);

        //return (result.collider != null);
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


    void HandleInput()
    {
        //Horizontal move
        if (Input.GetKey(KeyCode.A))
        {
            fallFixSwitch = true;
            //Character flip
            if (isFacingRight)
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
                    }
                    
                    isFacingRight = false; // facing left
                    transform.Rotate(0f, 180f, 0f); //rotate player and aiming to the left 
                    lastShootingLine.x = -1;
                }
            }
            if (playerState == PlayerState.MOVING_OBJECT_IDLE)
            {
                playerState = PlayerState.MOVING_OBJECT;
            }
            rb.isKinematic = false;
            if (playerState != PlayerState.JUMP && playerState != PlayerState.JUMPING &&
                playerState != PlayerState.FALLING && playerState != PlayerState.JUMP_FALLING && 
                playerState != PlayerState.MOVING_OBJECT_STOPPED_LEFT)
            {
                if (aimLineState == AimLineState.NOT_AIMED)
                {
                    //changes player speed depending if the player is walking or moving an object
                    if (playerState == PlayerState.MOVING_OBJECT)
                    {
                        moveVelocity = 0f - moveObjectSpeed;
                    }
                    else
                    {
                        moveVelocity = 0f - moveSpeed;
                    }
                }
            }
            
            
            if (playerState == PlayerState.IDLE || playerState == PlayerState.WALKING || playerState == PlayerState.FALL_FIX)
            {
                //only can move when not aiming
                if (aimLineState == AimLineState.NOT_AIMED)
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
            if (isFacingRight == false)
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
                    }
                    
                    isFacingRight = true;
                    transform.Rotate(0f, 180f, 0f); //rotate player and aiming to the left
                    lastShootingLine.x = 1;
                }
            }
            if (playerState == PlayerState.MOVING_OBJECT_IDLE)
            {
                playerState = PlayerState.MOVING_OBJECT;
            }
            rb.isKinematic = false;
            if (playerState != PlayerState.JUMP && playerState != PlayerState.JUMPING &&
                playerState != PlayerState.FALLING && playerState != PlayerState.JUMP_FALLING && 
                playerState != PlayerState.MOVING_OBJECT_STOPPED_RIGHT)
            {
                if (aimLineState == AimLineState.NOT_AIMED)
                {
                    //changes player speed depending if the player is walking or moving an object
                    if (playerState == PlayerState.MOVING_OBJECT)
                    {
                        moveVelocity = moveObjectSpeed;
                    }
                    else
                    {
                        moveVelocity = moveSpeed;
                    }
                }

            }
            //rb.constraints = RigidbodyConstraints2D.None;
            if (playerState == PlayerState.IDLE || playerState == PlayerState.WALKING || playerState == PlayerState.FALL_FIX)
            {
                //only can move when not aiming
                if (aimLineState == AimLineState.NOT_AIMED)
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
            if (playerState == PlayerState.WALKING )
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
            }
            moveVelocity = 0;
        }

        if (playerState != PlayerState.MOVING_OBJECT && playerState != PlayerState.MOVING_OBJECT_IDLE && 
            playerState != PlayerState.JUMP && playerState != PlayerState.JUMPING && 
             playerState != PlayerState.JUMP_FALLING)
        {
            MouseInputHandle();
        }

        //Jump
        if (Input.GetButtonDown("Jump"))
        {
            //Only can jump if player is on ground and not loaded projectile
            if (loadedProjectile == null && (
                playerState == PlayerState.WALKING || playerState == PlayerState.IDLE || playerState == PlayerState.FALL_FIX))
            {
                if (playerState == PlayerState.FALL_FIX)
                {
                    onGround = true;
                }

                if (onGround)
                {
                    Debug.Log("Jumping");
                    rb.isKinematic = false;
                    //SoundManager.instance.PLaySE(JumpSound);
                    //shouldJump = true;
                    playerState = PlayerState.JUMP;
                    rb.gravityScale = jumpGravity;
                    onGround = false;
                    //falling = false;
                }
            }
        }

        //Interact
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (lightCharges != 0)
            {
                if (playerState == PlayerState.FALL_FIX)
                {
                    onGround = true;
                }
                if (onGround)
                {
                    ////Find any interactable object within circle
                    //Collider2D result = Physics2D.OverlapCircle(transform.position, interactRadius, interactLayer);
                    //if (result != null)
                    //{
                    //    //Call interact interface function
                    //    IInteractable comp = result.gameObject.GetComponent<IInteractable>();
                    GameObject comp = PlayerObjectInteractions.playerObjectIInstance.GetCurrentObject();
                    if (comp != null)
                    {
                        comp.GetComponent<IInteractable>().Interact(); // call interact function
                        if (comp.GetComponent<Collider2D>().CompareTag("Lantern"))
                        {
                            UseLightCharges();
                        }

                        if (comp.GetComponent<Collider2D>().CompareTag("Metal"))
                        {
                            if (comp.GetComponent<Metal>().IsMoving())
                            {
                                playerState = PlayerState.MOVING_OBJECT;
                                currentMovingObject = comp;
                                //rb.gravityScale = jumpGravity;
                            }
                            else
                            {
                                playerState = PlayerState.WALKING;
                                currentMovingObject = null;
                                //rb.gravityScale = groundGravity;
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

    private void MouseInputHandle()
    {
        //checks mouse position
        if (catchCursor)
        {
            catchCursor = false;

            cursorPosition = Input.GetAxis("Mouse X");
        }

        //checks if mouse is stopped
        if (Input.GetAxis("Mouse X") == cursorPosition)
        {
            shootFixTimer = 0.5f;
            //aiming but not touching mouse
            if (aimLineState == AimLineState.AIMING)
            {
                timeLeft -= Time.deltaTime;
                //if time runs out
                if (timeLeft < 0)
                {
                    //not aiming anymore
                    aimLineState = AimLineState.NOT_AIMED;
                    UnloadProjectile();
                    StopAiming();
                    //mouseGlitchFix = false;
                    timeLeft = visibleCursorTimer;
                    //cursorSpriteRenderer.sprite = null;
                    catchCursor = true;
                    //visibleCursor = false;
                }
                else
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        //Debug.Log("Shoot");
                        aimLineState = AimLineState.NOT_AIMED;
                        if (loadedProjectile != null)
                        {
                            loadedProjectile.SetProjectileDirection(lastShootingLine);
                            loadedProjectile.GetComponent<Collider2D>().enabled = true;
                            StopAiming();
                            lightning.SetStartPosition(lightningStartPos);
                            lightning.SetTargetPosition(lightningTargetPos);
                            lightning.SetShootLightning(true);
                            //Can't shoot projectile continously
                            canShoot = false;
                            UseLightCharges();
                            //Set projectile's parent to player
                            //loadedProjectile.transform.SetParent(null);
                            //loadedProjectile.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

                        }
                    }
                    else
                    {
                        Aiming();
                    }
                }

            }
        }
        else
        {
            timeLeft = visibleCursorTimer;
            if (aimLineState == AimLineState.NOT_AIMED)
            {
                //if (!canShoot)
                //{
                //    shootFixTimer--;
                //    if (shootFixTimer <= 0)
                //    {
                //        shootFixTimer = 0.5f;
                //        canShoot = true;
                //    }
                //}
                //Can only shoot when player is on ground and if there is any lightCharges left


                //onGround = IsPlayerOnGround();
                if (canShoot == true && lightCharges != 0 && !firstAiming)
                {
                    if (playerState == PlayerState.FALL_FIX)
                    {
                        onGround = true;
                    }
                    if (onGround)
                    {
                        aimLineState = AimLineState.AIMING;
                        //Get projectile from list
                        if (listOfProjectile.Count != 0)
                        {

                            loadedProjectile = listOfProjectile[listOfProjectile.Count - 1];
                            if (listOfProjectile.Count > 10)
                            {
                                listOfProjectile.RemoveAt(0);
                            }

                            //Activate projectile
                            loadedProjectile.gameObject.SetActive(true);

                            loadedProjectile.transform.position =
                                transform.position + (-transform.right * projectileSpawnDistance);

                            loadedProjectile.GetComponent<Collider2D>().enabled = false;

                            //Set projectile's parent to player
                            //loadedProjectile.transform.SetParent(transform);
                            //loadedProjectile.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

                            //Turn on aimcone
                            aimCone.gameObject.SetActive(true);

                            //SEt aim line
                            aimLine.SetStartPoint(loadedProjectile.transform.position);
                            aimLine.gameObject.SetActive(true);
                        }
                    }
                }

                //cursorSpriteRenderer.sprite = cursorSprite;
                timeLeft = visibleCursorTimer;
                //Cursor.visible = true;
                //visibleCursor = true;
                if (firstAiming)
                {
                    firstAiming = false;
                }
            }
            else if (aimLineState == AimLineState.AIMING)
            {
                //Debug.Log("Aiming");
                if (Input.GetMouseButtonDown(0))
                {
                    if (loadedProjectile != null)
                    {
                        //Debug.Log("Shoot");
                        aimLineState = AimLineState.NOT_AIMED;
                        if (loadedProjectile != null)
                        {
                            loadedProjectile.SetProjectileDirection(lastShootingLine);
                            loadedProjectile.GetComponent<Collider2D>().enabled = true;
                            StopAiming();

                            //Can't shoot projectile continousely
                            canShoot = false;
                            UseLightCharges();

                            lightning.SetStartPosition(lightningStartPos);
                            lightning.SetTargetPosition(lightningTargetPos);
                            lightning.SetShootLightning(true);
                            //Set projectile's parent to player
                            //loadedProjectile.transform.SetParent(null);
                            //loadedProjectile.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

                        }
                    }
                }
                else
                {
                    Aiming();

                }
            }

            catchCursor = true;
            //else if (aimLineState == AimLineState.SHOOTING)
            //{
            //    loadedProjectile.SetProjectileDirection(lastShootingLine);
            //}

        }

        ////Left mouse down for spawn projectile
        //if (Input.GetMouseButtonDown(0))
        //{
        //    //Can only shoot when player is on ground and if there is nay lightCharges left
        //    if (canShoot == true && IsPlayerOnGround() && lightCharges != 0)
        //    {
        //        //Get projectile from list
        //        if (listOfProjectile.Count != 0)
        //        {
        //            loadedProjectile = listOfProjectile.Dequeue();

        //            //Activate projectile
        //            loadedProjectile.gameObject.SetActive(true);

        //            loadedProjectile.transform.position = transform.position + (-transform.right * projectileSpawnDistance);

        //            loadedProjectile.GetComponent<Collider2D>().enabled = false;

        //            //Set projectile's parent to player
        //            //loadedProjectile.transform.SetParent(transform);
        //            //loadedProjectile.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

        //            //Turn on aimcone
        //            aimCone.gameObject.SetActive(true);

        //            //SEt aim line
        //            aimLine.SetStartPoint(loadedProjectile.transform.position);
        //            aimLine.gameObject.SetActive(true);
        //        }
        //    }
        //}

        ////If holding mouse left button, calculate loaded projectile's position
        //if (Input.GetMouseButton(0))
        //{
        //    if (loadedProjectile != null)
        //    {
        //        //GEt mouse position of world
        //        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        //        mousePos.z = 0f;

        //        //Get forward vector of player
        //        Vector3 forwardVector = -transform.right;

        //        //Calculate shooting line
        //        shootingLine = mousePos - transform.position;
        //        shootingLine.Normalize();

        //        //Get angle between forward vector and shooting line
        //        float angleBetween = Vector2.Angle(forwardVector, shootingLine);

        //        //Set projectile based on shooting line
        //        if (angleBetween <= 45)
        //        {
        //            loadedProjectile.transform.position = transform.position + (shootingLine * projectileSpawnDistance);
        //            lastShootingLine = shootingLine;

        //            //SEt aim line
        //            aimLine.SetStartPoint(loadedProjectile.transform.position);

        //            if (debugMode)
        //            {
        //                //Draw shooting line
        //                Debug.DrawLine(loadedProjectile.transform.position, mousePos, Color.red);
        //            }

        //            //float rayDist = (mousePos - loadedProjectile.transform.position).magnitude;
        //            float rayDist = 50.0f;
        //            RaycastHit2D hit = Physics2D.Raycast(loadedProjectile.transform.position, shootingLine, rayDist, aimLineCollisionMask);
        //            if (hit.collider != null)
        //            {
        //                //Debug.Log(hit.collider.name);

        //                //SEt aim line
        //                aimLine.SetEndPoint(hit.point);
        //            }
        //            else
        //            {

        //                //SEt aim line
        //                aimLine.SetEndPoint(loadedProjectile.transform.position + (shootingLine * rayDist));
        //            }
        //        }
        //        else
        //        {
        //            loadedProjectile.transform.position = transform.position + (lastShootingLine * projectileSpawnDistance);

        //            //SEt aim line
        //            aimLine.SetStartPoint(loadedProjectile.transform.position);


        //            float rayDist = 50.0f;
        //            RaycastHit2D hit = Physics2D.Raycast(loadedProjectile.transform.position, lastShootingLine, rayDist, aimLineCollisionMask);
        //            if (hit.collider != null)
        //            {
        //                //Debug.Log(hit.collider.name);

        //                //SEt aim line
        //                aimLine.SetEndPoint(hit.point);
        //            }
        //            else
        //            {

        //                //SEt aim line
        //                aimLine.SetEndPoint(loadedProjectile.transform.position + (lastShootingLine * rayDist));
        //            }
        //        }
        //    }
        //}

        ////If release mouse left button, shoot loaded projectile
        //if (Input.GetMouseButtonUp(0))
        //{
        //    if (loadedProjectile != null)
        //    {
        //        Debug.Log("Shooting");
        //        loadedProjectile.SetProjectileDirection(lastShootingLine);
        //        loadedProjectile.GetComponent<Collider2D>().enabled = true;

        //        //Set projectile's parent to player
        //        //loadedProjectile.transform.SetParent(null);
        //        //loadedProjectile.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

        //        loadedProjectile = null;

        //        //Can't shoot projectile continousely
        //        canShoot = false;

        //        UseLightCharges();

        //        //Turn off aimcone
        //        aimCone.gameObject.SetActive(false);

        //        //Turn off aimline
        //        aimLine.gameObject.SetActive(false);

        //        Invoke("ResetShootCoolDown", shootCoolTime);
        //    }
        //}
    }

    private void StopAiming()
    {
        loadedProjectile = null;
        canShoot = false;
        //Turn off aimcone
        aimCone.gameObject.SetActive(false);
        //Turn off aimline
        aimLine.gameObject.SetActive(false);
        Invoke("ResetShootCoolDown", shootCoolTime);

    }
    
    private void UnloadProjectile()
    {
        if (loadedProjectile != null)
        {
            loadedProjectile.gameObject.SetActive(false);
        }
    }

    private void Aiming()
    {
        if (loadedProjectile != null)
        {
            //GEt mouse position of world
            Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            //Get forward vector of player
            Vector3 forwardVector = -transform.right;

            //Calculate shooting line
            shootingLine = mousePos - transform.position;
            shootingLine.Normalize();

            //Get angle between forward vector and shooting line
            angleBetween = Vector2.Angle(forwardVector, shootingLine);

            //Set projectile based on shooting line
            if (angleBetween <= 45)
            {
                loadedProjectile.transform.position = transform.position + (shootingLine * projectileSpawnDistance);
                lastShootingLine = shootingLine;

                //SEt aim line
                lightningStartPos = loadedProjectile.transform.position;
                aimLine.SetStartPoint(lightningStartPos);

                if (debugMode)
                {
                    //Draw shooting line
                    Debug.DrawLine(loadedProjectile.transform.position, mousePos, Color.red);
                }

                //float rayDist = (mousePos - loadedProjectile.transform.position).magnitude;
                float rayDist = 50.0f;
                RaycastHit2D hit = Physics2D.Raycast(loadedProjectile.transform.position, shootingLine, rayDist,
                    aimLineCollisionMask);
                if (hit.collider != null)
                {
                    //Debug.Log(hit.collider.name);

                    //SEt aim line
                    lightningTargetPos = hit.point;
                    aimLine.SetEndPoint(lightningTargetPos);
                }
                else
                {

                    //SEt aim line
                    lightningTargetPos = loadedProjectile.transform.position + (shootingLine * rayDist);
                    aimLine.SetEndPoint(lightningTargetPos);
                }
            }
            else
            {
                loadedProjectile.transform.position = transform.position + (lastShootingLine * projectileSpawnDistance);

                //SEt aim line
                lightningStartPos = loadedProjectile.transform.position;
                aimLine.SetStartPoint(lightningStartPos);


                float rayDist = 50.0f;
                RaycastHit2D hit = Physics2D.Raycast(loadedProjectile.transform.position, lastShootingLine, rayDist,
                    aimLineCollisionMask);
                if (hit.collider != null)
                {
                    //Debug.Log(hit.collider.name);
                    lightningTargetPos = hit.point;
                    //SEt aim line
                    aimLine.SetEndPoint(lightningTargetPos);
                }
                else
                {
                    //Set aim line
                    lightningTargetPos = loadedProjectile.transform.position + (lastShootingLine * rayDist);
                    aimLine.SetEndPoint(lightningTargetPos);
                }
            }
        }
    }


    public void UseLightCharges()
    {
        lightCharges -= 1;

        if (onLightChargesChanged != null)
        {
            onLightChargesChanged.Invoke(lightCharges, maxLightCharges);
        }
    }

    //public void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.collider.CompareTag("Ground"))
    //    {
    //        Debug.Log("Ground");
            

    //        //onGround = true;
            
    //    }
    //}
    //Set from PlayerCollision collider to set when the player is on the ground 
    public void SetOnGround(bool state)
    {
        onGround = state;
        playerState = PlayerState.WALKING;
        rb.gravityScale = groundGravity;
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

    //public void OnCollisionExit2D(Collision2D collision)
        //{
        //    if (collision.collider.CompareTag("Ground"))
        //    {
        //        if (!movingObjectsCollision.GetComponent<MovingObjectsCollision>().IsGrounded())
        //        {
        //            GameObject comp = PlayerObjectInteractions.playerObjectIInstance.GetCurrentObject();
        //            if (comp != null)
        //            {
        //                comp.GetComponent<IInteractable>().Interact();
        //                if (comp.GetComponent<Collider2D>().CompareTag("Lantern"))
        //                {
        //                    UseLightCharges();
        //                }

        //                if (comp.GetComponent<Collider2D>().CompareTag("Metal"))
        //                {
        //                    if (comp.GetComponent<Metal>().IsMoving())
        //                    {
        //                        playerState = PlayerState.MOVING_OBJECT;
        //                    }
        //                    else
        //                    {
        //                        playerState = PlayerState.WALKING;
        //                    }
        //                }
        //            }

        //            onGround = false;
        //        }
        //    }
        //}

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
                        comp.GetComponent<IInteractable>().Interact();
                        onGround = false;
                        playerState = PlayerState.FALLING;
                        rb.isKinematic = false;
                        rb.gravityScale = fallGravity;
                        //capsuleCollider2D.sharedMaterial.friction = inAirFriction;
                    }
                }
            }
        }
        else
        {
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
                rb.gravityScale = jumpGravity;
            }
            //capsuleCollider2D.sharedMaterial.friction = inAirFriction;
        }
    }

    public void SetMoveObjectStopped()
    {
        if (isFacingRight)
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
        Debug.Log("Metal Exiting");
        if (currentMovingObject != null)
        {
            currentMovingObject = null;
            if (playerState != PlayerState.FALLING && playerState != PlayerState.JUMP_FALLING)
            {
                playerState = PlayerState.WALKING;
            }
        }
    }
    public GameObject GetCurrentMovingObject()
    {
        return currentMovingObject;
    }

    public void SetFallFix()
    {
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

}