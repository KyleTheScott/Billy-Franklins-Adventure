using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimLine : MonoBehaviour
{
    LineRenderer lineRenderer = null;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetStartPoint(Vector3 startPos)
    {
        lineRenderer.SetPosition(0, startPos);
    }

    public void SetEndPoint(Vector3 endPos)
    {
        lineRenderer.SetPosition(1, endPos);
    }
}
