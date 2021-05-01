﻿using UnityEngine;

public class Rope : MonoBehaviour
{
    //link at start at top of rope
    [SerializeField] Rigidbody2D hook;

    //middle rope links
    [SerializeField] GameObject linkPrefab;

    //last link at the end at the bottom of the rope
    [SerializeField] Weight weight;

    //sets how many links in the rope
    [SerializeField] int links = 100;

    // Start is called before the first frame update
    void Start()
    {
        GenerateRope();
    }

    void GenerateRope()
    { 
        //sets first links previous link to be the hook (first link) 
        Rigidbody2D previousRB = hook;
        //loops through to create rope
        for (int i = 0; i < links; i++)
        {
            //creates a link and connects it to the previous link
            GameObject link = Instantiate(linkPrefab, transform);
            HingeJoint2D joint = link.GetComponent<HingeJoint2D>();
            joint.connectedBody = previousRB;

            //sets the previous or attaches the final link to the weight
            if (i < links - 1)
            { 
                previousRB = link.GetComponent<Rigidbody2D>();
            }
            else
            {
                weight.ConnectRopeEnd(link.GetComponent<Rigidbody2D>());
            }
        }
    }
}
