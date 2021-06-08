using UnityEngine;

public class Link : MonoBehaviour
{
    [SerializeField] private SuspendedPlatform platform;

    private void Awake()
    {
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
    }

    public void OnTriggerEnter2D(Collider2D collision)
    { 
        //If rope is hit by lightning...
        if (collision.CompareTag("Lightning"))
        {
            // I might cache this later but it is only happening the one time when it is hit
            // If I cache it then every link will have the platform // might be better
            if (platform != null)
            {
                platform.SetGrounded();
                platform.TurnOffConstraints();
            }

            //destroy rope link
            Destroy(gameObject);
        }
    }
}
