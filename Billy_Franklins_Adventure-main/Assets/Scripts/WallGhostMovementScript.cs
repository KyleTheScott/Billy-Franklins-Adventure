using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGhostMovementScript : MonoBehaviour
{
    [SerializeField]
    private float RotateSpeed = 5.0f;
    [SerializeField]
    private float Radius = 0.15f;
    private Vector2 centre;
    private float angle;
    // Start is called before the first frame update
    void Start()
    {
        centre = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        angle += RotateSpeed * Time.deltaTime;

        Vector2 offset = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * Radius;
        transform.localPosition = centre + offset;
    }
}
