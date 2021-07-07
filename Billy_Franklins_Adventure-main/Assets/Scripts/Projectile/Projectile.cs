//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    Rigidbody2D rb; //Projectile's rigid body
    Camera mainCam = null;

    public float speed = 20f; //projectile speed

    public Shooting owner = null; //Who owns this projectile?

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //organize the projectiles into one parent gameObject
        GameObject projectiles = GameObject.Find("Projectiles");
        transform.parent = projectiles.transform; 
        gameObject.SetActive(false);
        mainCam = Camera.main;
        owner = FindObjectOfType<Shooting>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCam != null)
        {
            //Get screen bounds 
            Vector2 screenBoundsLeftBottom = mainCam.ScreenToWorldPoint(new Vector3(0, 0, mainCam.transform.position.z));
            Vector2 screenBoundsRightTop = mainCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCam.transform.position.z));


            //If projectile goes out of the screen bounds
            if (transform.position.x >= screenBoundsRightTop.x ||
                transform.position.x <= screenBoundsLeftBottom.x ||
                transform.position.y >= screenBoundsRightTop.y ||
                transform.position.y <= screenBoundsLeftBottom.y)
            {
                //go back to owner
                if (owner != null)
                {
                    gameObject.SetActive(false);
                    owner.ReturnProjectile(this);
                }
            }
        }
    }

    public void SetProjectileDirection(Vector2 moveDir)
    {
        rb.velocity = moveDir * speed;
    }


    //Only trigger or collide with tile and interactable
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.gameObject.name);

        //If projectile is player's
        //if (gameObject.layer == (int)USER_LAYER.PLAYER_PROJECTILE &&
        //    collision.gameObject.layer == (int)USER_LAYER.OPPONENT)
        //{
        //    collision.gameObject.GetComponent<PlayerCharacter>().hp -= 1;
        //}
        //else if (gameObject.layer == (int)USER_LAYER.OPPONENT_PROJECTILE &&
        //    collision.gameObject.layer == (int)USER_LAYER.PLAYER)
        //{
        //    collision.gameObject.GetComponent<PlayerCharacter>().hp -= 1;
        //}

        //    if (collision.gameObject.layer == USER_LAYER.PLAYER)
        //{
        //    //Debug.Log("Collide with chafracter");
        //    //If projectile collide with opposite layer(either opponent or player)
        //    if(gameObject.layer != collision.gameObject.layer)
        //    {

        //    }
        //}    
        if (!collision.CompareTag("Bucket"))
        {
            //If collide with others
            if (owner != null)
            {
                gameObject.SetActive(false);
                owner.ReturnProjectile(this);
            }
        }
    }

    //Only trigger or collide with tile and interactable
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Bucket"))
        {
            //If collide with others
            if (owner != null)
            {
                gameObject.SetActive(false);
                owner.ReturnProjectile(this);
            }
        }
    }
}
