using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shooting : MonoBehaviour
{
    [SerializeField] Projectile loadedProjectile = null; //projectile that is wating for shooting
    
    private Player player; 


    [Header("Aiming")]
    [SerializeField] private AimLine aimLine = null; //player's aiming line
    Camera mainCam = null; // used for aiming
    Vector3 shootingLine = new Vector3(1, 0, 0); //Direction for loaded projectile
    Vector3 lastShootingLine = new Vector3(1, 0, 0);
    bool IsShootingLineInAngle = false;
    [SerializeField] LayerMask aimLineCollisionMask; //should be tile or obstacle
    [SerializeField] Transform aimCone = null;

    [SerializeField] private bool firstAiming = true;

    //variable for aim line disappearing and appearing with mouse movement
    private bool mouseClick;
    float timeLeft = 1;
    float visibleCursorTimer = .5f;
    float cursorPosition;
    bool catchCursor = true;

    [SerializeField] private float shootFixTimer = .5f;



    //public enum AimLineState
    //{
    //    NOT_AIMED,
    //    AIMING
    //};

    //[SerializeField] private AimLineState aimLineState = AimLineState.NOT_AIMED;

    [Header("Shooting")]
    public float shootCoolTime = 0.5f; //Projectile shoot cool time
   
    [SerializeField] private bool canShoot = true; //Check if player can shoot projectile

    public int maxNumOfProjectile = 10; //Max number of projectile
    public GameObject projectilePrefab = null; //Prefab for projectile
    [SerializeField] private List<Projectile> listOfProjectile = null;

    [SerializeField] float projectileSpawnDistance = 1f; //How far is projectile spanwed from player?

    //variables to show lightning
    [SerializeField] private float angleBetween;
    [SerializeField] private Lightning lightning;
    private Vector2 lightningStartPos;
    private Vector2 lightningTargetPos;

    private Charges charges;





    void Start()
    {
        player = GameObject.FindObjectOfType<Player>();
        charges = GameObject.FindObjectOfType<Charges>();
        //Create list for projectile pool
        listOfProjectile = new List<Projectile>();
        for (int i = 0; i < maxNumOfProjectile; ++i)
        {
            GameObject projectile = Instantiate(projectilePrefab);

            //Set owner of this projectile
            //projectile.GetComponent<Projectile>().owner = FindObjectOfType<Shooting>().gameObject;

            //Add to pool
            listOfProjectile.Add(projectile.GetComponent<Projectile>());
        }
        //aiming
        mainCam = Camera.main;
        //aimLine = GetComponentInChildren<AimLine>();

    }


    public enum AimLineState
    {
        NOT_AIMED,
        AIMING
    };

    [SerializeField] private AimLineState aimLineState = AimLineState.NOT_AIMED;


    public AimLineState GetAimLineState()
    {
        return aimLineState;
    }
    public void SetAimLineState(AimLineState state)
    {
        aimLineState = state;
    }

    public Projectile GetProjectile()
    {
        return loadedProjectile;
    }


    public bool GetCanShoot()
    {
        return canShoot;
    }


    public void SetLastShootingLine(int xValue)
    {
        lastShootingLine.x = xValue;
    }


    public bool GetLoadedProjectile()
    {
        if (loadedProjectile != null)
        {
            return true;
        }
        else
        {
            return false;
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
            Vector3 forwardVector = -player.transform.right;

            //Calculate shooting line
            shootingLine = mousePos - player.transform.position;
            shootingLine.Normalize();

            //Get angle between forward vector and shooting line
            angleBetween = Vector2.Angle(forwardVector, shootingLine);

            //Set projectile based on shooting line
            if (angleBetween <= 45)
            {
                loadedProjectile.transform.position = player.transform.position + (shootingLine * projectileSpawnDistance);
                lastShootingLine = shootingLine;

                //SEt aim line
                lightningStartPos = loadedProjectile.transform.position;
                aimLine.SetStartPoint(lightningStartPos);

                //if (debugMode)
                //{
                //    //Draw shooting line
                //    Debug.DrawLine(loadedProjectile.transform.position, mousePos, Color.red);
                //}

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
                loadedProjectile.transform.position = player.transform.position + (lastShootingLine * projectileSpawnDistance);

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


    public void MouseInputHandle()
    {
        if (player.GetMovementEnabled())
        {
            if (aimLineState == AimLineState.NOT_AIMED)
            {
                //Can only shoot when player is on ground and if there is any lightCharges left
                if (canShoot == true && charges.GetLightCharges() != 0 && !firstAiming)
                {
                    if (player.GetPlayerState() == Player.PlayerState.FALL_FIX)
                    {
                        player.SetOnGround(true);
                    }
                    if (player.GetOnGround())
                    {
                        if (Input.GetMouseButtonDown(0))
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
                                    player.transform.position + (-player.transform.right * projectileSpawnDistance);

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
                }
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
                            charges.UseLightCharges();

                            lightning.SetStartPosition(lightningStartPos);
                            lightning.SetTargetPosition(lightningTargetPos);
                            lightning.SetShootLightning(true);
                            //Set projectile's parent to player
                            //loadedProjectile.transform.SetParent(null);
                            //loadedProjectile.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

                        }
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    //not aiming anymore
                    aimLineState = AimLineState.NOT_AIMED;
                    UnloadProjectile();
                    StopAiming();
                    //mouseGlitchFix = false;
                    //timeLeft = visibleCursorTimer;
                    //cursorSpriteRenderer.sprite = null;
                    //catchCursor = true;
                    //visibleCursor = false;
                }
                else
                {
                    Aiming();
                }
            }
            catchCursor = true;
        }
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


}
