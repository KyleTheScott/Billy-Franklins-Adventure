using UnityEngine;

public class Link : MonoBehaviour
{
    [SerializeField] private SuspendedPlatform platform;
    //[SerializeField] private bool kiteRope = false;
    [SerializeField]
    [FMODUnity.EventRef]
    private string ropeBurnEvent;

    private Player player;

    private void Awake()
    {
        //if (kiteRope)
        //{
        //    player = FindObjectOfType<Player>();
        //    player.PlayerMovingHorizontallyEvent.AddListener(MoveKiteWithPlayerHorizontal);
        //    player.PlayerMovingVerticallyEvent.AddListener(MoveKiteWithPlayerVertical);
        //}
        //else
        //{
        Transform platformTransform = null;
        platformTransform = transform.parent.Find("SuspendedPlatform");

        if (platformTransform == null)
        {
            platformTransform = transform.parent.parent.Find("SuspendedPlatform");
        }

        if (platformTransform != null)
        {
            platform = platformTransform.gameObject.GetComponent<SuspendedPlatform>();
        }
        //}
    }
    //public void MoveKiteWithPlayerHorizontal()
    //{
    //    transform.position = new Vector2(transform.position.x + -player.GetDistPlayerMoveX(), transform.position.y);
    //}
    //public void MoveKiteWithPlayerVertical()
    //{
    //    transform.position = new Vector2(transform.position.x, transform.position.y + -player.GetDistPlayerMoveY());
    //}


    public void OnTriggerEnter2D(Collider2D collision)
    { 
        //If rope is hit by lightning...
        if (collision.CompareTag("Lightning"))
        {
            FMODUnity.RuntimeManager.PlayOneShot(ropeBurnEvent);
            // I might cache this later but it is only happening the one time when it is hit
            // If I cache it then every link will have the platform // might be better
            if (platform != null)
            {
                platform.SetGrounded();
                platform.DisconnectPlatformFromPlayer();
                platform.TurnOffConstraints();
            }
            //destroy rope link
            
            Destroy(gameObject);
        }
    }
}
