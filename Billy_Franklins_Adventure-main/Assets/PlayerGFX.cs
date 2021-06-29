using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGFX : MonoBehaviour
{
    private SpriteRenderer playerSprite;
    private Animator playerAnimator;
    private Player player;
    private Player.PlayerState currentPlayerState;

    // Start is called before the first frame update
    void Start()
    {
        playerSprite = gameObject.GetComponent<SpriteRenderer>();
        playerAnimator = gameObject.GetComponent<Animator>();
        player = GameObject.FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        ChooseAnimation();
    }

    private void ChooseAnimation()
    {
        if (currentPlayerState != player.GetPlayerState())
        {
            switch (player.GetPlayerState())
            {
                case Player.PlayerState.IDLE:

                    break;
                case Player.PlayerState.WALKING:

                    break;
                case Player.PlayerState.JUMP:

                    break;
                case Player.PlayerState.JUMPING:

                    break;
                case Player.PlayerState.JUMP_FALLING:

                    break;
                case Player.PlayerState.FALLING:

                    break;
                case Player.PlayerState.FALL_FIX:

                    break;
                case Player.PlayerState.MOVING_OBJECT:

                    break;
                case Player.PlayerState.MOVING_OBJECT_IDLE:

                    break;
                case Player.PlayerState.MOVING_OBJECT_LEFT:

                    break; 
                case Player.PlayerState.MOVING_OBJECT_STOPPED_LEFT:

                    break;
                case Player.PlayerState.MOVING_OBJECT_RIGHT:

                    break;
                case Player.PlayerState.MOVING_OBJECT_STOPPED_RIGHT:

                    break;
            }
        }

        currentPlayerState = player.GetPlayerState();
    }
}
