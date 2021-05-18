//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class MouseCursor : MonoBehaviour
//{

//    public static bool mouseClick;
//    //public static bool visibleCursor;
//    float timeLeft;
//    float visibleCursorTimer = 0.5f;
//    float cursorPosition;
//    float cursorPosY;
//    //Vector2 cursorSensitivity;
//    bool catchCursor = true;
//    public Camera mouseCamera;
//    //SpriteRenderer cursorSpriteRenderer;
//    //public Sprite cursorSprite;
//    //CircleCollider2D cursorCollider;

//    void Start()
//    {
//        //transform.position = new Vector2(transform.position.x + 100, transform.position.y + 100);
//        //cursorCollider = gameObject.GetComponent<CircleCollider2D>();
//        //cursorSensitivity = new Vector2(0.1f, 0.1f);
//        mouseClick = false;
//        //mouseGlitchFix = false;
//        //cursorSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
//        Cursor.visible = false;
//    }
//    // Update is called once per frame
//    void Update()
//    {
//        if (!mouseClick)
//        {
//            if (catchCursor)
//            {
//                catchCursor = false;

//                //Vector2 mouseMovement = new Vector2(Input.GetAxisRaw("Mouse X") * cursorSensitivity.x,
//                //                    Input.GetAxisRaw("Mouse Y") * cursorSensitivity.y);
//                cursorPosition = Input.GetAxis("Mouse X");

//                //cursorCollider.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//            }
//            if (Input.GetAxis("Mouse X") == cursorPosition)
//            {
//                timeLeft -= Time.deltaTime;

//                if (timeLeft < 0)
//                {
//                    //mouseGlitchFix = false;
//                    timeLeft = visibleCursorTimer;
//                    //cursorSpriteRenderer.sprite = null;
//                    Cursor.visible = false;
//                    //visibleCursor = false;
//                    catchCursor = true;
//                }
//            }
//            else
//            {
//                //cursorSpriteRenderer.sprite = cursorSprite;
//                timeLeft = visibleCursorTimer;
//                Cursor.visible = true;
//                //visibleCursor = true;
//                catchCursor = true;
//            }
//        }
//    }
//}

//else if (mouseGlitchFix)
//{
//    cursorSpriteRenderer.sprite = null;
//    Cursor.visible = false;
//    timeLeftGlitchSecond -= Time.deltaTime;
//    if (timeLeftGlitchSecond < 0)
//    {
//        mouseGlitchFix = false;
//        timeLeftGlitchSecond = visibleCursorGlitchTimerSecond;
//        //timeLeft = visibleCursorTimer;
//        cursorSpriteRenderer.sprite = null;
//        Cursor.visible = false;
//        catchCursor = true;
//    }
//}

//timeLeftGlitch -= Time.deltaTime;
//if (timeLeftGlitch < 0)
//{
//    mouseGlitchFix = true;
//    timeLeftGlitch = visibleCursorGlitchTimer;
//    //timeLeft = visibleCursorTimer;
//    cursorSpriteRenderer.sprite = null;
//    Cursor.visible = false;
//    catchCursor = true;
//}

//if (Input.GetAxis("Mouse X")< 0) || (Input.GetAxis("Mouse Y") != 0);
//{
//    Vector2 cursorPos = mouseCamera.ScreenToWorldPoint(Input.mousePosition);
//    transform.position = cursorPos;
//}
