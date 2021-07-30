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
    //set start point of the aim line
    public void SetStartPoint(Vector3 startPos)
    {

        lineRenderer.SetPosition(0, startPos);
    }
    //set end point of the aim line
    public void SetEndPoint(Vector3 endPos)
    {
        lineRenderer.SetPosition(1, endPos);
    }
}
