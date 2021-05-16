using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
[DefaultExecutionOrder(-100)] //ensure this script runs before all other player scripts to prevent laggy input
public class Player : MonoBehaviour
{
    Rigidbody2D rb = null; //player's rigid body
    CapsuleCollider2D capsuleCollider2D = null; //Player's capsule collider
    AimLine aimLine = null; //player's aiming line

    [SerializeField] bool debugMode = false;

    [Header("Attribute")]
    public float moveSpeed = 3.0f;
    public float jumpForce = 10f; //How strong does player jump
    public float shootCoolTime = 0.5f; //Projectile shoot cool time
    public int hp = 10;
    public int lightCharges = 3; //how many lighting can character use?
    public int maxLightCharges = 3; //how many lighting can character use?

    Vector2 moveDir = Vector2.zero; //player's movement direction

    bool shouldJump = false; //Check if player should jump
    bool canShoot = true; //Check if player can shoot projectile

    [Header("Projectile")]
    public int maxNumOfProjectile = 10; //Max number of projectile
    public GameObject projectilePrefab = null; //Prefab for projectile
    Queue<Projectile> listOfProjectile = null; //Queue for projectile pool
    [SerializeField] float projectileSpawnDistance = 1f; //How far is projectile spanwed from player?
    Projectile loadedProjectile = null; //projectile that is wating for shooting
    Vector3 shootingLine = new Vector3(1, 0, 0); //Direction for loaded projectile
    Vector3 lastShootingLine = new Vector3(1, 0, 0);
    bool IsShootingLineInAngle = false;
    [SerializeField] LayerMask aimLineCollisionMask; //should be tile or obstacle

    bool isFacingRight = false; //Is character facing right side? for Characte flip

    [SerializeField] LayerMask tileLayerMask; //Used to check if player is on ground

    // to Get SFX sound name 
    [SerializeField]
    private string ShootSound;
    [SerializeField]
    private string JumpSound;

    private Animator animator;

    [Header("Interact")]
    [SerializeField] float interactRadius = 5f;
    [SerializeField] LayerMask interactLayer;

    Camera mainCam = null;

    [SerializeField] Transform aimCone = null;

    //current charges, max charges
    public UnityEvent<int, int> onLightChargesChanged; //DarkBOrder will subscribe, charges text ui will subscribe this

    
    private enum AimLineState 
    {
        NOT_AIMED,
        AIMING
    };

    [SerializeField] private AimLineState aimLineState = AimLineState.NOT_AIMED;

    private bool mouseClick;
    //public static bool visibleCursor;
    float timeLeft = 1;
    float visibleCursorTimer = 2f;
    float cursorPosition;
    float cursorPosY;
    //Vector2 cursorSensitivity;
    bool catchCursor = true;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        //animator = GetComponentInChildren<Animator>();

        //Create queue for projectile pool
        listOfProjectile = new Queue<Projectile>();
        for (int i = 0; i < maxNumOfProjectile; ++i)
        {
            GameObject projectile = Instantiate(projectilePrefab);

            //Set owner of this projectile
            projectile.GetComponent<Projectile>().owner = this;

            //Add to pool
            listOfProjectile.Enqueue(projectile.GetComponent<Projectile>());
        }

        mainCam = Camera.main;

        aimLine = GetComponentInChildren<AimLine>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        //Jump
        if (shouldJump == true)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            shouldJump = false;
        }

        //Change player's velocity
        //only can move when not aiming
        if (loadedProjectile == null)
        {
            Vector2 tempVel = rb.velocity;
            tempVel.x = moveDir.x * moveSpeed;
            rb.velocity = tempVel;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
        //animator.SetInteger("Direction", (int)moveDir.x);
    }

    void HandleInput()
    {
        //Horizontal move
        if (Input.GetKey(KeyCode.A))
        {
            moveDir.x = -1;

            //Characte flip
            if (isFacingRight == true)
            {
                
                isFacingRight = false;
                transform.Rotate(0f, 180f, 0f);
                lastShootingLine.x = -1;
                //transform.localScale = new Vector3(1, 1, 1);
            }

            if (aimLineState == AimLineState.AIMING)
            {
                aimLineState = AimLineState.NOT_AIMED;
                UnloadProjectile();
                StopAiming();
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveDir.x = 1;

            //Characte flip
            if (isFacingRight == false)
            {
                
                isFacingRight = true;
                transform.Rotate(0f, 180f, 0f);
                lastShootingLine.x = 1;
                //transform.localScale = new Vector3(-1, 1, 1);
            }
            if (aimLineState == AimLineState.AIMING)
            {
                aimLineState = AimLineState.NOT_AIMED;
                UnloadProjectile();
                StopAiming();
            }
        }
        else
        {
            moveDir.x = 0f;
            MouseInputHandle();
        }


        //Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Only can jump if player is on ground and not loaded projectile
            if (IsPlayerOnGround() == true && loadedProjectile == null)
            {
                //SoundManager.instance.PLaySE(JumpSound);
                shouldJump = true;
            }
        }

        //Interact
        if(Input.GetKeyDown(KeyCode.E))
        {
            if (lightCharges != 0)
            {
                //Find any interactable object within circle
                Collider2D result = Physics2D.OverlapCircle(transform.position, interactRadius, interactLayer);
                if (result != null)
                {
                    //Call interact interface function
                    IInteractable comp = result.gameObject.GetComponent<IInteractable>();
                    if (comp != null)
                    {
                        comp.Interact();
                        if (result.CompareTag("Lantern"))
                        {
                            UseLightCharges();
                        }
                    }
                }
            }                          
        }

        
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
        aimLineState = AimLineState.NOT_AIMED;
    }

    //Return projectile to pool
    public void ReturnProjectile(Projectile projectile)
    {
        listOfProjectile.Enqueue(projectile);
    }

    bool IsPlayerOnGround()
    {
        //Do capsule cast to downward of player so that it checks if player is on ground
        RaycastHit2D result = Physics2D.CapsuleCast(capsuleCollider2D.bounds.center, capsuleCollider2D.bounds.size, CapsuleDirection2D.Vertical, 0f, Vector2.down, 0.1f, tileLayerMask);

        //Debug.Log(result.collider);

        return (result.collider != null);
    } 

    private void OnDrawGizmosSelected()
    {
        if (debugMode)
        {
            //Draw interactable circle
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactRadius);
        }
    }

    private void MouseInputHandle()
    {
        if (catchCursor)
        {
            catchCursor = false;

            //Vector2 mouseMovement = new Vector2(Input.GetAxisRaw("Mouse X") * cursorSensitivity.x,
            //                    Input.GetAxisRaw("Mouse Y") * cursorSensitivity.y);
            cursorPosition = Input.GetAxis("Mouse X");

            //cursorCollider.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetAxis("Mouse X") == cursorPosition)
        {
            if (aimLineState == AimLineState.AIMING)
            {
                timeLeft -= Time.deltaTime;

                if (timeLeft < 0)
                {
                    aimLineState = AimLineState.NOT_AIMED;
                    UnloadProjectile();
                    StopAiming();
                    //mouseGlitchFix = false;
                    timeLeft = visibleCursorTimer;
                    //cursorSpriteRenderer.sprite = null;

                    //visibleCursor = false;
                    catchCursor = true;
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
            if (aimLineState == AimLineState.NOT_AIMED)
            {
                //Debug.Log("Not Aiming");
                //Can only shoot when player is on ground and if there is any lightCharges left
                if (canShoot == true && IsPlayerOnGround() && lightCharges != 0)
                {
                    aimLineState = AimLineState.AIMING;
                    //Get projectile from list
                    if (listOfProjectile.Count != 0)
                    {
                        if (loadedProjectile != null)
                        {
                            UnloadProjectile();
                            loadedProjectile = null;
                        }

                        loadedProjectile = listOfProjectile.Dequeue();

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
                catchCursor = true;
            }
            else if (aimLineState == AimLineState.AIMING)
            {
                //Debug.Log("Aiming");
                if (Input.GetMouseButtonDown(0))
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
                else
                {
                    Aiming();
                }
                catchCursor = true;
            }
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
        //Turn off aimcone
        aimCone.gameObject.SetActive(false);
        //Turn off aimline
        aimLine.gameObject.SetActive(false);
        Invoke("ResetShootCoolDown", shootCoolTime);
        loadedProjectile = null;
    }

    private void UnloadProjectile()
    {
        if (loadedProjectile != null)
        {
            loadedProjectile.gameObject.SetActive(false);
        }
        foreach(Projectile shot in listOfProjectile)
        {
            
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
                RaycastHit2D hit = Physics2D.Raycast(loadedProjectile.transform.position, shootingLine, rayDist, aimLineCollisionMask);
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
                RaycastHit2D hit = Physics2D.Raycast(loadedProjectile.transform.position, lastShootingLine, rayDist, aimLineCollisionMask);
                if (hit.collider != null)
                {
                    //Debug.Log(hit.collider.name);

                    //SEt aim line
                    aimLine.SetEndPoint(hit.point);
                }
                else
                {

                    //SEt aim line
                    aimLine.SetEndPoint(loadedProjectile.transform.position + (lastShootingLine * rayDist));
                }
            }
        }
    }


    void UseLightCharges()
    {
        lightCharges -= 1;

        if (onLightChargesChanged != null)
        {
            onLightChargesChanged.Invoke(lightCharges, maxLightCharges);
        }
    }
}
