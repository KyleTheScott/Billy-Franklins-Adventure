using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWallController : MonoBehaviour
{
    private Collider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private bool isLowered;
    [SerializeField]
    private float lowerWallSpirteAlpha = 100;
    [SerializeField]
    private float raiseWallSpirteAlpha = 255;
    [SerializeField]
    private bool LowerOnStart = false;
    public bool IsLowered => isLowered;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (LowerOnStart)
        {
            LowerGhostWall();
        }
    }

    
    public void LowerGhostWall()
    {
        boxCollider.isTrigger = true;
        Color newColor = spriteRenderer.color;
        newColor.a = lowerWallSpirteAlpha / 255;
        spriteRenderer.color = newColor;
    }

    public void RaiseGhostWall()
    {
        boxCollider.isTrigger = false;
        Color newColor = spriteRenderer.color;
        newColor.a = raiseWallSpirteAlpha / 255;
        spriteRenderer.color = newColor;
    }
}
