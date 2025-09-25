using UnityEngine;

public class Spike : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController controller = collision.GetComponent<PlayerController>();
        Debug.Log(collision.gameObject.name);

        if (controller != null)
        {
            controller.IsDead = true;
        }
    }
}
