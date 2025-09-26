using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Grapple : MonoBehaviour
{
    public Transform Target;
    [SerializeField] DistanceJoint2D _distantJoint;

    private void OnTriggerStay2D(Collider2D collision)
    {
        GrabPoint grab = collision.GetComponent<GrabPoint>();

        if (grab != null)
        {
            grab.Display(true);
            Target = grab.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GrabPoint grab = collision.GetComponent<GrabPoint>();

        if (grab != null)
        {
            grab.Display(false);
            Target = null;
        }
    }

    public bool Grab(bool value)
    {
        if (Target == null)
            return false;

        if (value)
        {
            _distantJoint.connectedAnchor = Target.position;
            _distantJoint.enabled = true;
        }
        else
        {
            _distantJoint.connectedAnchor = transform.position;
            _distantJoint.enabled = false;
        }

        return true;
    }
}
