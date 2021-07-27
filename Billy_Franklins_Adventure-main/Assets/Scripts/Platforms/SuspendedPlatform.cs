using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;


public class SuspendedPlatform : MonoBehaviour
{
    // attributes to adjust how the platform moves randomly like there is wind
    [SerializeField] private float minTime = 2.0f;
    [SerializeField] private float maxTime = 5.0f;
    float platformTimer;

    [SerializeField] private float minMoveTime = 0.0f;
    [SerializeField] private float maxMoveTime = 1.0f;
    float moveTimer;

    [SerializeField] private float minSidewaysTime = 2.0f;
    [SerializeField] private float maxSidewaysTime = 5.0f;
    float platformSidewaysTimer;

    [SerializeField] private float minSidewaysMoveTime = 0.0f;
    [SerializeField] private float maxSidewaysMoveTime = 1.0f;
    float moveSidewaysTimer;



    [SerializeField] private float minUpMoveSpeedLight = 0.0f;
    [SerializeField] private float maxUpMoveSpeedLight = 10.0f;
    [SerializeField] private float minUpMoveSpeedMedium = 0.0f;
    [SerializeField] private float maxUpMoveSpeedMedium = 30.0f;
    [SerializeField] private float minUpMoveSpeedHeavy = 0.0f;
    [SerializeField] private float maxUpMoveSpeedHeavy = 50.0f;

    [SerializeField] private float minSidewaysMoveSpeedLight = 0.0f;
    [SerializeField] private float maxSidewaysMoveSpeedLight = 10.0f;
    [SerializeField] private float minSidewaysMoveSpeedMedium = 0.0f;
    [SerializeField] private float maxSidewaysMoveSpeedMedium = 30.0f;
    [SerializeField] private float minSidewaysMoveSpeedHeavy = 0.0f;
    [SerializeField] private float maxSidewaysMoveSpeedHeavy = 50.0f;




    [SerializeField] private float movementSpeed = 20.0f;
    [SerializeField] private float sideWaysMoveSpeed = 20.0f;

    public Rigidbody2D platformRigidbody { get; private set; }
    private bool moving = false;
    private bool movingSideways = false;
    private bool grounded = false;

    [SerializeField] private bool upAndDownPlatform = true; 

    private Player player;

    [SerializeField] private bool playerTouching = false;

    [SerializeField] private float fallFixTimer = 0;
    [SerializeField] private float fallFixMax = .1f;

    [SerializeField] private bool windBlowingRight = false;
    [SerializeField] private float lastRotation;


    public enum PlatformWindState
    {
        LIGHT,
        MEDIUM,
        HEAVY
    }

    [SerializeField] private PlatformWindState platformWindState = PlatformWindState.LIGHT;

    // Start is called before the first frame update
    void Start()
    {
        platformTimer = Random.Range(minTime, maxTime);
        platformRigidbody = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.FindObjectOfType<Player>().GetComponent<Player>();
        WindController.instance.BlowWindEvent.AddListener(SetWindPlatform);
        WindController.instance.CalmWindEvent.AddListener(SetWindPlatform);
    }

    // Update is called once per frame
    void Update()
    {
        if (!grounded)
        {
            MovePlatform();
            if (playerTouching && moving)
            {
                //player.transform.rotation = Quaternion.Euler(0.0f, player.transform.rotation.y, this.transform.rotation.z * -1.0f);

                //Vector3 fixRotation = player.transform.localEulerAngles;
                //fixRotation.z = 0;

                //player.transform.localEulerAngles = fixRotation;
                //player.transform.rotation = Quaternion.AngleAxis(0, fixRotation);

                //if (fallFixTimer >= fallFixMax)
                //{
                //    player.SetFallFix();
                //}
                //fallFixTimer += Time.deltaTime;
            }
        }
    }

   

    //set so object doesn't move once it is on the ground
    public void SetGrounded()
    {
        grounded = true;
    }

    public void SetKinematic()
    {
        platformRigidbody.isKinematic = true;
        platformRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void TurnOffConstraints()
    {
        platformRigidbody.constraints = RigidbodyConstraints2D.None;
    }

    void MovePlatform()
    {
        MoveUpAndDown();
        if (!upAndDownPlatform)
        {
            MoveSideways();
        }

        if (playerTouching)
        {
            if (Mathf.Abs(transform.eulerAngles.z - lastRotation) >= 0.05f)
            {
                player.SetFallFix();
                //Debug.Log("Platform");
                //player.SetRotationPlatformFix();
            }

            //lastRotation = transform.eulerAngles.z;
        }
    }

    private void MoveUpAndDown()
    {
        //moves the platform up and down
        if (moving)
        {
            if (moveTimer <= 0)
            {
                platformTimer = Random.Range(minTime, maxTime);
                moving = false;
            }

            platformRigidbody.AddForce(transform.up * movementSpeed);

            moveTimer -= Time.deltaTime;
        }
        //timer until next time the platform moves
        else
        {
            if (platformTimer <= 0)
            {
                switch (platformWindState)
                {
                    case PlatformWindState.LIGHT:
                        movementSpeed = Random.Range(minUpMoveSpeedLight, maxUpMoveSpeedLight);
                        break;
                    case PlatformWindState.MEDIUM:
                        movementSpeed = Random.Range(minUpMoveSpeedMedium, maxUpMoveSpeedMedium);
                        break;
                    case PlatformWindState.HEAVY:
                        movementSpeed = Random.Range(minUpMoveSpeedHeavy, maxUpMoveSpeedHeavy);
                        break;
                }
                moveTimer = Random.Range(minMoveTime, maxMoveTime);
                moving = true;
            }

            platformTimer -= Time.deltaTime;
        }
    }

    private void MoveSideways()
    {
        //moves the platform up and down
        if (movingSideways)
        {
            if (moveSidewaysTimer <= 0)
            {
                platformSidewaysTimer = Random.Range(minSidewaysTime, maxSidewaysTime);
                movingSideways = false;
            }

            if (windBlowingRight)
            {
                platformRigidbody.AddForce(transform.right * sideWaysMoveSpeed);
            }
            else
            {
                platformRigidbody.AddForce(transform.right * -sideWaysMoveSpeed);
            }

            moveSidewaysTimer -= Time.deltaTime;
        }
        //timer until next time the platform moves
        else
        {
            if (platformSidewaysTimer <= 0)
            {
                switch (platformWindState)
                {
                    case PlatformWindState.LIGHT:
                        sideWaysMoveSpeed = Random.Range(minSidewaysMoveSpeedLight, maxSidewaysMoveSpeedLight);
                        break;
                    case PlatformWindState.MEDIUM:
                        sideWaysMoveSpeed = Random.Range(minSidewaysMoveSpeedMedium, maxSidewaysMoveSpeedMedium);
                        break;
                    case PlatformWindState.HEAVY:
                        sideWaysMoveSpeed = Random.Range(minSidewaysMoveSpeedHeavy, maxSidewaysMoveSpeedHeavy);
                        break;
                }
                moveSidewaysTimer = Random.Range(minSidewaysMoveTime, maxSidewaysMoveTime);
                sideWaysMoveSpeed = Random.Range(minSidewaysMoveSpeedHeavy, maxSidewaysMoveSpeedHeavy);
                movingSideways = true;
            }
            platformSidewaysTimer -= Time.deltaTime;
        }
    }

    private void ConnectPlatformToPlayer()
    {
        if (!grounded)
        {
            //player.transform.parent = transform;
            playerTouching = true;
        }
    }

    public void DisconnectPlatformFromPlayer()
    {
        
        //if (player.transform.parent == transform)
        //{
            //player.transform.parent = null;
            //DontDestroyOnLoad(player.gameObject);
            Debug.Log("Disconnect");
        playerTouching = false;
        //}
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ConnectPlatformToPlayer();
            
            //player.AddParentConstraint(gameObject);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //playerTouching = false;
            DisconnectPlatformFromPlayer();
            //player.RemoveParentConstraint(gameObject);
        }
    }

    private void SetWindPlatform()
    {
        float blowSpeed = WindController.instance.GetWindSpeed();
        if (blowSpeed > 0)
        {
            windBlowingRight = true;
        }
        else
        {
            windBlowingRight = false;
        }
        if (blowSpeed < -40 || blowSpeed > 40)
        {
            platformWindState = PlatformWindState.HEAVY;
        }
        else if (blowSpeed < -20 || blowSpeed > 20)
        {
            platformWindState = PlatformWindState.MEDIUM;
        }
        else
        {
            platformWindState = PlatformWindState.LIGHT;
        }
    }
}
