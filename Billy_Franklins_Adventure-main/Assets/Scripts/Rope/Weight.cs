
using UnityEngine;

public class Weight : MonoBehaviour
{
    [SerializeField] float distanceFromChainEnd = .1f;

    //connects the final weight link to the last link
    public void ConnectRopeEnd(Rigidbody2D endRB)
    {
        HingeJoint2D joint = gameObject.AddComponent<HingeJoint2D>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedBody = endRB;
        joint.anchor = Vector2.zero;
        joint.connectedAnchor = new Vector2(0f, -distanceFromChainEnd);

    }
}
