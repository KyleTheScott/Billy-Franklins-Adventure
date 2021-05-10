using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //If rope is hit by lightning...
        if (collision.CompareTag("Lightning"))
        {
            // I might cache this later but it is only happening the one time when it is hit
            // If I cache it then every link will have the platform // might be better
            Transform platform = transform.parent.Find("SuspendedPlatform");
            platform.gameObject.GetComponent<SuspendedPlatform>().SetGrounded();

            //destroy rope link
            Destroy(gameObject);
        }
    }

}
