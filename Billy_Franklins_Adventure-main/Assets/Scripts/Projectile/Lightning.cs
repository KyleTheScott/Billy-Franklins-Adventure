using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Lightning : MonoBehaviour
{
    private LineRenderer lineRenderer;

    [SerializeField] private Texture[] textures;

    private int animationStep;

    [SerializeField] private float fps = 4f;

    private float fpsCounter;

    private bool shootingLightning; 

    //make setters for start and end pos
    public void SetStartPosition(Vector2 startPos)
    {
        lineRenderer.SetPosition(0, startPos);
    }

    public void SetTargetPosition(Vector2 endPos)
    {
        lineRenderer.SetPosition(1, endPos);
    }

    public void SetShootLightning(bool state)
    {
        lineRenderer.enabled = state;
        shootingLightning = state;
    }


    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    
    void Update()
    {
        if (shootingLightning)
        {
            ShootLightning();
        }
    }

    private void ShootLightning()
    {
        fpsCounter += Time.deltaTime;
        if (fpsCounter >= 1f / fps)
        {
            animationStep++;
            if (animationStep == textures.Length)
            {
                SetShootLightning(false);
                lineRenderer.enabled = false;
                animationStep = 0;
            }
            lineRenderer.material.SetTexture("_MainTex", textures[animationStep]);
            fpsCounter = 0;
        }
    }
}
