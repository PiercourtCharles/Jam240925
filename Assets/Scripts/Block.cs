using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rb;

    public void Drop()
    {
        if (_rb.bodyType == RigidbodyType2D.Dynamic)
            _rb.bodyType = RigidbodyType2D.Kinematic;
        else
            _rb.bodyType = RigidbodyType2D.Dynamic;
    }
}
