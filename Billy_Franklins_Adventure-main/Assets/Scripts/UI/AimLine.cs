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

    public void SetStartPoint(Vector3 startPos)
    {
        lineRenderer.SetPosition(0, startPos);
    }

    public void SetEndPoint(Vector3 endPos)
    {
        lineRenderer.SetPosition(1, endPos);
    }
}
