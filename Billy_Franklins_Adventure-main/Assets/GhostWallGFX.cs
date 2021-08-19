using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWallGFX : MonoBehaviour
{
    [SerializeField]
    GhostWallController ghostWall;

    public void Appear()
    {
        ghostWall.LowerGhostWallEnd();
    }

    public void Disappear()
    {
        ghostWall.RaiseGhostWallEnd();
    }

    public void GhostsAppear()
    {
        ghostWall.MakeGhostsAppear();
    }
}
