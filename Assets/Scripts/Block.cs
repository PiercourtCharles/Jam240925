using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rb;

    public void Drop(bool value)
    {
        if (value)
            _rb.bodyType = RigidbodyType2D.Kinematic;
        else
            _rb.bodyType = RigidbodyType2D.Dynamic;
    }
}
