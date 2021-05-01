using UnityEngine;

public class Link : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    { 
        //If rope is hit by lightning...
        if (collision.CompareTag("Lightning"))
        {
            Debug.Log("Rope Hit");
            //destroy rope link
            Destroy(gameObject);
        }
    }

}
