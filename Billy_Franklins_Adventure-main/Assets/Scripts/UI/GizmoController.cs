using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoController : MonoBehaviour
{
    [SerializeField] private Vector4 gizmo_color_ = new Vector4(0, 1, 1, 0.75f);
    [SerializeField] private float gizmo_radius_ = 0.3f;

    void OnDrawGizmos()
    {
        // Draw a green sphere at the transform's position
        Gizmos.color = gizmo_color_;
        Gizmos.DrawWireSphere(transform.position, gizmo_radius_);
    }
}
