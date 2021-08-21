using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shooting : MonoBehaviour
{
    [SerializeField] Projectile loadedProjectile = null; //projectile that is wating for shooting
    
    private Player player;
    private PlayerGFX playerGFX;


    [Header("Aiming")]
    [SerializeField] private AimLine aimLine = null; //player's aiming line
    Camera mainCam = null; // used for aiming
    [SerializeField] Vector3 shootingLine = new Vector3(1, 0, 0); //Direction for loaded projectile
    [SerializeField] Vector3 lastShootingLine = new Vector3(1, 0, 0);
    bool IsShootingLineInAngle = false;
    [SerializeField] LayerMask aimLineCollisionMask; 
    [SerializeField] Transform aimCone = null;

    [SerializeField] private bool firstAiming = true;

    //[SerializeField] private float shootFixTimer = .5f;

    public enum AimLineState
    {
        NOT_AIMED,
        AIMING
    };

    [SerializeField] private AimLineState aimLineState = AimLineState.NOT_AIMED;

    [Header("Shooting")]
    public float shootCoolTime = 0.5f; //Projectile shoot cool time
   
    [SerializeField] private bool canShoot = true; //Check if player can shoot projectile

    public int maxNumOfProjectile = 10; //Max number of projectile
    public GameObject projectilePrefab = null; //Prefab for projectile
    [SerializeField] private List<Projectile> listOfProjectile = null;

    [SerializeField] float projectileSpawnDistance = 2f; //How far is projectile spanwed from player?
    [SerializeField] private float lineXOffset; //offset of x dimension of start point for aiming line
    [SerializeField] private float lineYOffset; //offset of y dimension of start point for aiming line



    //variables to show lightning
    [SerializeField] private float angleBetween;
    [SerializeField] private Lightning lightning;
    private Vector2 lightningStartPos;
    private Vector2 lightningTargetPos;

    [SerializeField] GameObject lightningStartPosition;

    private Vector3 frontOfPlayer;
    private Vector3 forwardVector;

    private Charges charges;


    void Start()
    {
        player = GameObject.FindObjectOfType<Player>();
        playerGFX = GameObject.FindObjectOfType<PlayerGFX>();
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
            if (playerGFX.GetFacingRight())
            {
                forwardVector = new Vector3(1, 0, 0);
            }
            else
            {
                forwardVector = new Vector3(-1, 0, 0);
            }
            if (playerGFX.GetFacingRight())
            {
                frontOfPlayer = new Vector3(player.transform.position.x + lineXOffset,
                    player.transform.position.y + lineYOffset, player.transform.position.z);
            }
            else
            {
                frontOfPlayer = new Vector3(player.transform.position.x - lineXOffset,
                    player.transform.position.y + lineYOffset, player.transform.position.z);
            }
            //Calculate shooting line
            shootingLine = mousePos - frontOfPlayer;
            shootingLine.Normalize();

            //Get angle between forward vector and shooting line
            angleBetween = Vector2.Angle(forwardVector, shootingLine);

            //Set projectile based on shooting line
            if (angleBetween <= 45)
            {
                loadedProjectile.transform.position = frontOfPlayer /*+ (shootingLine * projectileSpawnDistance)*/;
                lastShootingLine = shootingLine;

                //SEt aim line
                lightningStartPos = frontOfPlayer + (shootingLine * projectileSpawnDistance);/*loadedProjectile.transform.position*/;
                aimLine.SetStartPoint(lightningStartPos);

                float rayDist = 50.0f;
                RaycastHit2D hit = Physics2D.Raycast(frontOfPlayer /*loadedProjectile.transform.position*/, shootingLine, rayDist,
                    aimLineCollisionMask);
                if (hit.collider != null)
                {
                    //SEt aim line
                    lightningTargetPos = hit.point;
                    aimLine.SetEndPoint(lightningTargetPos);
                }
                else
                {

                    //SEt aim line
                    lightningTargetPos = frontOfPlayer + (shootingLine * rayDist);
                    aimLine.SetEndPoint(lightningTargetPos);
                }
            }
            else
            {
                loadedProjectile.transform.position = frontOfPlayer;

                //SEt aim line
                lightningStartPos = frontOfPlayer + (lastShootingLine * projectileSpawnDistance);
                aimLine.SetStartPoint(lightningStartPos);


                float rayDist = 50.0f;
                RaycastHit2D hit = Physics2D.Raycast(frontOfPlayer, lastShootingLine, rayDist,
                    aimLineCollisionMask);
                if (hit.collider != null)
                {
                    lightningTargetPos = hit.point;
                    //SEt aim line
                    aimLine.SetEndPoint(lightningTargetPos);
                }
                else
                {
                    //Set aim line
                    lightningTargetPos = frontOfPlayer/*loadedProjectile.transform.position*/ + (lastShootingLine * rayDist);
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

                                if (playerGFX.GetFacingRight())
                                {
                                    frontOfPlayer = new Vector3(player.transform.position.x + lineXOffset,
                                        player.transform.position.y + lineYOffset, player.transform.position.z);
                                }
                                else
                                {
                                    frontOfPlayer = new Vector3(player.transform.position.x - lineXOffset,
                                        player.transform.position.y + lineYOffset, player.transform.position.z);
                                }

                                loadedProjectile.transform.position = frontOfPlayer /*+ (-player.transform.right * projectileSpawnDistance)*/;

                                loadedProjectile.GetComponent<Collider2D>().enabled = false;

                                //Turn on aimcone
                                aimCone.gameObject.SetActive(true);

                                //SEt aim line
                                aimLine.SetStartPoint(frontOfPlayer/*loadedProjectile.transform.position*/);
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
                        playerGFX.SetAnimation(16);
                        player.SetAnimationMovement(true);
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    //not aiming anymore
                    UnloadProjectile();
                    StopAiming();
                    playerGFX.SetAnimation(17);
                }
                else
                {
                    Aiming();
                }
            }
            //catchCursor = true;
        }
    }

    public void StartShooting()
    {
        loadedProjectile.SetProjectileDirection(lastShootingLine);
        loadedProjectile.GetComponent<Collider2D>().enabled = true;
        loadedProjectile.SetProjectileTarget(lightningTargetPos);
        StopAiming();
        canShoot = false;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {

            playerGFX.SetAnimation(1);
        }
        else
        {
            playerGFX.SetAnimation(0);
        }
        charges.UseLightCharges();

        lightning.SetStartPosition(lightningStartPosition.transform.position);
        lightning.SetTargetPosition(lightningTargetPos);
        lightning.SetShootLightning(true);
    }

    public void StopShooting()
    {
        aimLineState = AimLineState.NOT_AIMED;
    }


    public void StopAiming()
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
