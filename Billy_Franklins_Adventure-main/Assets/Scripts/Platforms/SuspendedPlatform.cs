using System;
using System.IO.MemoryMappedFiles;
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



    [SerializeField] private float minUpMoveSpeedLight = 0.0f;
    [SerializeField] private float maxUpMoveSpeedLight = 10.0f;
    [SerializeField] private float minUpMoveSpeedMedium = 10.0f;
    [SerializeField] private float maxUpMoveSpeedMedium = 30.0f;
    [SerializeField] private float minUpMoveSpeedHeavy = 30.0f;
    [SerializeField] private float maxUpMoveSpeedHeavy = 50.0f;



    [SerializeField] private float movementSpeed = 20.0f;

    Rigidbody2D platformRigidbody;
    private bool moving = false;
    private bool grounded = false;

    private bool upAndDownPlatform = true; 

    private Player player;

    [SerializeField] private bool playerTouching;

    [SerializeField] private float fallFixTimer = 0;
    [SerializeField] private float fallFixMax = .1f;

    [SerializeField] private bool windBlowingRight = false;

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
        player = GameObject.Find("Player").GetComponent<Player>();
        WindController.instance.BlowWindEvent.AddListener(BlowWindPlatform);
        WindController.instance.CalmWindEvent.AddListener(CalmWindPlatform);
    }

    // Update is called once per frame
    void Update()
    {
        if (!grounded)
        {
            MovePlatform();
            if (playerTouching && moving)
            {
                if (fallFixTimer >= fallFixMax)
                {
                    player.SetFallFix();
                }
                fallFixTimer += Time.deltaTime;
            }
        }
    }

    public void BlowWindPlatform()
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
        if (blowSpeed > -25 && blowSpeed < 27)
        {

        }
        //else if 
        //{
            
        //}
    }

    public void CalmWindPlatform()
    {
        float blowSpeed = WindController.instance.GetWindSpeed();
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
               
                moveTimer = Random.Range(minMoveTime, maxMoveTime);
                movementSpeed = Random.Range(minUpMoveSpeedHeavy, maxUpMoveSpeedHeavy);
                moving = true;
            }

            platformTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerTouching = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerTouching = false;
        }
    }
}
