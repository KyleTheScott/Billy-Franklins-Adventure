using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.UI;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
[DefaultExecutionOrder(-100)] //ensure this script runs before all other player scripts to prevent laggy input
public class Player : MonoBehaviour
{
    private CheckPointDeathSystem checkPointDeathSystem = null;

    Rigidbody2D rb = null; //player's rigid body
    CapsuleCollider2D capsuleCollider2D = null; //Player's capsule collider
    AimLine aimLine = null; //player's aiming line

    [SerializeField] bool debugMode = false;

    [Header("Attribute")] [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float jumpForce = 10f; //How strong does player jump
    [SerializeField] private float forwardJumpForce = 10f;
    [SerializeField] private float moveVelocity;
    public float shootCoolTime = 0.5f; //Projectile shoot cool time
    public int hp = 10;
    public int lightCharges = 3; //how many lighting can character use?
    public int maxLightCharges = 3; //how many lighting can character use?

    Vector2 moveDir = Vector2.zero; //player's movement direction

    bool shouldJump = false; //Check if player should jump
    [SerializeField] private bool canShoot = true; //Check if player can shoot projectile

    private bool moving;

    [SerializeField] private bool onGround;
    [SerializeField] private bool falling;
    //[SerializeField] private bool 

    [SerializeField] private SpriteRenderer playerSprite;

    private bool movingObject;

    private enum PlayerState
    {
        IDLE,
        JUMP,
        JUMPING,
        FALLING,
        WALKING,
        MOVING_OBJECT

    }

    [SerializeField] private PlayerState playerState = PlayerState.WALKING;

    private Vector3 lastPosition;

    [SerializeField] private float inAirFriction = 0f;
    [SerializeField] private float onGroundFriction = 0f;

    //changes in gravity
    [SerializeField] private float jumpGravity = 1f;
    [SerializeField] private float fallGravity = 5f;
    [SerializeField] private float groundGravity = 10f;

    //[Range(1, 10)] [SerializeField] private float jumpVelocity = 2;

    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float fallMultiplier = 4f;


    [Header("Projectile")] public int maxNumOfProjectile = 10; //Max number of projectile

    public GameObject projectilePrefab = null; //Prefab for projectile

    //Queue<Projectile> listOfProjectile = null; //Queue for projectile pool
    [SerializeField] private List<Projectile> listOfProjectile = null;

    [SerializeField] float projectileSpawnDistance = 1f; //How far is projectile spanwed from player?
    Projectile loadedProjectile = null; //projectile that is wating for shooting
    Vector3 shootingLine = new Vector3(1, 0, 0); //Direction for loaded projectile
    Vector3 lastShootingLine = new Vector3(1, 0, 0);
    bool IsShootingLineInAngle = false;
    [SerializeField] LayerMask aimLineCollisionMask; //should be tile or obstacle

    bool isFacingRight = false; //Is character facing right side? for Characte flip

    [SerializeField] LayerMask tileLayerMask; //Used to check if player is on ground

    // to Get SFX sound name 
    [SerializeField] private string ShootSound;
    [SerializeField] private string JumpSound;

    private Animator animator;

    [Header("Interact")] [SerializeField] float interactRadius = 5f;
    [SerializeField] LayerMask interactLayer;

    Camera mainCam = null;

    [SerializeField] Transform aimCone = null;

    //current charges, max charges
    public UnityEvent<int, int> onLightChargesChanged; //DarkBOrder will subscribe, charges text ui will subscribe this



    [SerializeField] private bool firstAiming = true;

    private enum AimLineState
    {
        NOT_AIMED,
        AIMING
    };

    [SerializeField] private AimLineState aimLineState = AimLineState.NOT_AIMED;

    private bool mouseClick;

    //public static bool visibleCursor;
    float timeLeft = 1;
    float visibleCursorTimer = .5f;

    float cursorPosition;

    //Vector2 cursorSensitivity;
    bool catchCursor = true;

    [SerializeField] private float shootFixTimer = .5f;


    [SerializeField] private MovingObjectsCollision movingObjectsCollision;




    private void Awake()
    {
        //reference checkpoint + death system script 
        checkPointDeathSystem = GameObject.Find("GlobalGameController").GetComponent<CheckPointDeathSystem>();
    }


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        //animator = GetComponentInChildren<Animator>();

        //Create queue for projectile pool
        listOfProjectile = new List<Projectile>();
        for (int i = 0; i < maxNumOfProjectile; ++i)
        {
            GameObject projectile = Instantiate(projectilePrefab);

            //Set owner of this projectile
            projectile.GetComponent<Projectile>().owner = this;

            //Add to pool
            listOfProjectile.Add(projectile.GetComponent<Projectile>());
        }

        mainCam = Camera.main;
        isFacingRight = true;
        playerState = PlayerState.JUMPING;
        transform.Rotate(0f, 180f, 0f);

        aimLine = GetComponentInChildren<AimLine>();
        aimLineState = AimLineState.NOT_AIMED;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();

        //if there are no charges left then player has died
        if (lightCharges == 0 && loadedProjectile == null)
        {
            checkPointDeathSystem.PlayerDeath();
        }
    }

    private void FixedUpdate()
    {
        switch (playerState)
        {
            case PlayerState.IDLE:
                if (lastPosition != transform.position)
                {
                    //rb.constraints = RigidbodyConstraints2D.FreezePositionX;
                    rb.velocity = Vector2.zero;
                    rb.isKinematic = true;
                }
                break;
            case PlayerState.JUMP:
                
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                
                playerState = PlayerState.JUMPING;
                break;
            case PlayerState.JUMPING:
               

                if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
                {
                    //rb.velocity = Vector2.up * jumpVelocity;
                    rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier * 1) * Time.deltaTime;
                    rb.velocity = new Vector2(moveVelocity, rb.velocity.y);
                    //if (isFacingRight)
                    //{
                    //    rb.velocity = Vector2.right * moveVelocity;
                    //}
                    //else
                    //{
                    //    rb.velocity = Vector2.left * moveVelocity;
                    //}

                }
                else
                {
                    falling = true;
                    //rb.velocity = new Vector2(moveVelocity, transform.position.y);
                    rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier * 1) * Time.deltaTime;
                    rb.velocity = new Vector2(moveVelocity, rb.velocity.y);
                    //if (isFacingRight)
                    //{
                    //    rb.velocity = Vector2.right * moveVelocity;
                    //}
                    //else
                    //{
                    //    rb.velocity = Vector2.left * moveVelocity;
                    //}
                }
                
                //if (onGround)
                //{
                //    playerState = PlayerState.WALKING;
                //}
                break;
            case PlayerState.FALLING:
                falling = true;
                rb.velocity = new Vector2(moveVelocity, rb.velocity.y);
                break;
            case PlayerState.WALKING:
                //moveVelocity = 0;
                ////Change player's velocity
                ////only can move when not aiming
                //if (isFacingRight)
                //{
                //    moveVelocity = moveSpeed;
                //}
                //else
                //{
                //    moveVelocity = 0f - moveSpeed;
                //}
                rb.velocity = new Vector2(moveVelocity, rb.velocity.y);
                break;
            case PlayerState.MOVING_OBJECT:
                rb.velocity = new Vector2(moveVelocity, rb.velocity.y);
                break;
        }
        lastPosition = transform.position;

        //animator.SetInteger("Direction", (int)moveDir.x);
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
            //Character flip
            if (isFacingRight)
            {
                if (playerState != PlayerState.JUMP && playerState != PlayerState.JUMPING &&
                    playerState != PlayerState.FALLING)
                {
                    isFacingRight = false;
                    transform.Rotate(0f, 180f, 0f);
                    lastShootingLine.x = -1;
                }
            }
            rb.isKinematic = false;
            if (playerState != PlayerState.JUMP && playerState != PlayerState.JUMPING &&
                playerState != PlayerState.FALLING)
            {
                moveVelocity = 0f - moveSpeed;
            }

            falling = false;
            //rb.constraints = RigidbodyConstraints2D.None;

            //shouldJump = false;

            //Change player's velocity
            //only can move when not aiming
            if (playerState != PlayerState.JUMP && playerState != PlayerState.JUMPING && 
                playerState != PlayerState.FALLING && playerState != PlayerState.MOVING_OBJECT)
            {
                if (aimLineState == AimLineState.NOT_AIMED)
                {

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
            //Character flip
            if (isFacingRight == false)
            {
                if (playerState != PlayerState.JUMP && playerState != PlayerState.JUMPING &&
                    playerState != PlayerState.FALLING)
                {
                    isFacingRight = true;
                    transform.Rotate(0f, 180f, 0f);
                    lastShootingLine.x = 1;
                }
            }
            rb.isKinematic = false;
            if (playerState != PlayerState.JUMP && playerState != PlayerState.JUMPING &&
                playerState != PlayerState.FALLING)
            {
                moveVelocity = moveSpeed;
            }

            falling = false;
            //rb.constraints = RigidbodyConstraints2D.None;
            if (playerState != PlayerState.JUMP && playerState != PlayerState.JUMPING &&
                playerState != PlayerState.FALLING && playerState != PlayerState.MOVING_OBJECT)
            {
                if (aimLineState == AimLineState.NOT_AIMED)
                {
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
            if (playerState != PlayerState.MOVING_OBJECT && playerState != PlayerState.JUMP && 
                playerState != PlayerState.JUMPING && playerState != PlayerState.FALLING)
            {
                playerState = PlayerState.IDLE;
            }
            falling = false;
            moveVelocity = 0;
        }

        if (playerState != PlayerState.MOVING_OBJECT && playerState != PlayerState.JUMP && 
            playerState != PlayerState.JUMPING && playerState != PlayerState.FALLING)
        {
            MouseInputHandle();
        }

        //Jump
        if (Input.GetButtonDown("Jump"))
        {
            //Only can jump if player is on ground and not loaded projectile
            if (onGround && loadedProjectile == null && (playerState == PlayerState.WALKING || playerState == PlayerState.IDLE))
            {
                rb.isKinematic = false;
                //SoundManager.instance.PLaySE(JumpSound);
                //shouldJump = true;
                playerState = PlayerState.JUMP;
                rb.gravityScale = jumpGravity;
                onGround = false;
                falling = false;
            }
        }

        //Interact
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (lightCharges != 0 && onGround)
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
                    comp.GetComponent<IInteractable>().Interact();
                    if (comp.GetComponent<Collider2D>().CompareTag("Lantern"))
                    {
                        UseLightCharges();
                    }

                    if (comp.GetComponent<Collider2D>().CompareTag("Metal"))
                    {
                        if (comp.GetComponent<Metal>().IsMoving())
                        {
                            playerState = PlayerState.MOVING_OBJECT;
                        }
                        else
                        {
                            playerState = PlayerState.WALKING;
                        }
                    }
                }

                //}
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
                if (canShoot == true && onGround && lightCharges != 0 && !firstAiming)
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
            float angleBetween = Vector2.Angle(forwardVector, shootingLine);

            //Set projectile based on shooting line
            if (angleBetween <= 45)
            {
                loadedProjectile.transform.position = transform.position + (shootingLine * projectileSpawnDistance);
                lastShootingLine = shootingLine;

                //SEt aim line
                aimLine.SetStartPoint(loadedProjectile.transform.position);

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
                    aimLine.SetEndPoint(hit.point);
                }
                else
                {

                    //SEt aim line
                    aimLine.SetEndPoint(loadedProjectile.transform.position + (shootingLine * rayDist));
                }
            }
            else
            {
                loadedProjectile.transform.position = transform.position + (lastShootingLine * projectileSpawnDistance);

                //SEt aim line
                aimLine.SetStartPoint(loadedProjectile.transform.position);


                float rayDist = 50.0f;
                RaycastHit2D hit = Physics2D.Raycast(loadedProjectile.transform.position, lastShootingLine, rayDist,
                    aimLineCollisionMask);
                if (hit.collider != null)
                {
                    //Debug.Log(hit.collider.name);

                    //SEt aim line
                    aimLine.SetEndPoint(hit.point);
                }
                else
                {
                    //Set aim line
                    aimLine.SetEndPoint(loadedProjectile.transform.position + (lastShootingLine * rayDist));
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
        capsuleCollider2D.sharedMaterial.friction = onGroundFriction;
    }

    public bool GetFalling()
    {
        return falling;
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
                        comp.GetComponent<IInteractable>().Interact();
                        onGround = false;
                        playerState = PlayerState.FALLING;
                        rb.gravityScale = fallGravity;
                        capsuleCollider2D.sharedMaterial.friction = inAirFriction;
                    }
                }
            }
        }
        else
        {
            onGround = false;
            
            if (playerState != PlayerState.JUMP && playerState != PlayerState.JUMPING)
            {
                playerState = PlayerState.FALLING;
                //make a 3rd gravity
            }
            else
            {
                rb.gravityScale = jumpGravity;
            }
            capsuleCollider2D.sharedMaterial.friction = inAirFriction;
        }
    }
}