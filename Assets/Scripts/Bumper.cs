using UnityEngine;

public class Bumper : MonoBehaviour
{
    [SerializeField] float _power = 1;
    [SerializeField] Vector2 _direction = Vector2.up;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController controller = collision.GetComponent<PlayerController>();
        Debug.Log(collision.gameObject.name);

        if (controller != null)
        {
            var rb = controller.GetComponent<Rigidbody2D>();
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(_direction * _power, ForceMode2D.Impulse);
        }
    }
}
