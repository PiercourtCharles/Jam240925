using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Grapple : MonoBehaviour
{
    [SerializeField] DistanceJoint2D _distantJoint;
    [SerializeField] Transform _target;

    private void OnTriggerStay2D(Collider2D collision)
    {
        GrabPoint grab = collision.GetComponent<GrabPoint>();

        if (grab != null)
        {
            grab.Display(true);
            _target = grab.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GrabPoint grab = collision.GetComponent<GrabPoint>();

        if (grab != null)
        {
            grab.Display(false);
            _target = null;
        }
    }

    public bool Grab(bool value)
    {
        if (_target == null)
            return false;

        if (value)
        {
            _distantJoint.connectedAnchor = _target.position;
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
