using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Kite : MonoBehaviour
{
    //private Player player;
    //private Vector2 destination;
    //[SerializeField] private float kiteSpeed = 0.01f;
    //[SerializeField] private float kiteTimer = 0;
    //[SerializeField] private float currentKiteTimerMax = 1;
    //[SerializeField] private float KiteTimerMin = 1;
    //[SerializeField] private float kiteTimerMax = 2;
    


    //void Start()
    //{
    //    player = FindObjectOfType<Player>();
    //    player.PlayerMovingHorizontallyEvent.AddListener(MoveKiteWithPlayerHorizontal);
    //    player.PlayerMovingVerticallyEvent.AddListener(MoveKiteWithPlayerVertical);
    //    destination = transform.position;
    //}

    //private void FixedUpdate()
    //{
    //    //if (kiteTimer <= currentKiteTimerMax)
    //    //{
    //    //    kiteTimer += Time.deltaTime;
            
    //    //    float step = kiteSpeed * Time.deltaTime;
    //    //    transform.position = new Vector2(transform.position.x + step, transform.position.y + step);
    //    //}
    //    //else
    //    //{
    //    //    kiteSpeed = Random.Range(-1, 1);
    //    //    kiteTimer = 0;
    //    //    currentKiteTimerMax = Random.Range(KiteTimerMin, kiteTimerMax);
    //    //}

    //    //if (Vector2.Distance(transform.position, destination) <= Mathf.Epsilon)
    //    //{
    //    //    float destXOffset = Random.Range(0,1);
    //    //    float destYOffset = Random.Range(3, 6);
    //    //    destination = new Vector2(player.transform.position.x + destXOffset, player.transform.position.y + destXOffset);
    //    //}
    //    //else
    //    //{
    //    //    if (Vector2.Distance(transform.position, player.transform.position) > 1)
    //    //    {
    //    //        destination = new Vector2(player.transform.position.x, player.transform.position.y + 10);
    //    //    }
    //    //    else
    //    //    {
    //    //        float step = kiteSpeed * Time.deltaTime;
    //    //        transform.position = Vector2.MoveTowards(transform.position, destination, step);
    //    //    }

    //    //}
    //}

    //public void MoveKiteWithPlayerHorizontal()
    //{
    //    transform.position = new Vector2(transform.position.x + -player.GetDistPlayerMoveX(), transform.position.y);
    //}
    //public void MoveKiteWithPlayerVertical()
    //{
    //    transform.position = new Vector2(transform.position.x , transform.position.y + -player.GetDistPlayerMoveY());
    //}

    //public void SetKiteStartPosition(Vector2 playerPos)
    //{
    //    transform.position = new Vector2(playerPos.x, playerPos.y + 3);
    //}
}