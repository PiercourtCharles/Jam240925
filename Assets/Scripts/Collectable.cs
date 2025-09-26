using UnityEngine;

public class Collectable : MonoBehaviour
{
    public enum Type
    {
        Interro,
        Excla
    }

    [SerializeField] Type _type;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController controller = collision.GetComponent<PlayerController>();

        if (controller != null)
        {
            if (_type == Type.Excla)
            {
                controller.Block.transform.position = controller.BlockInitPos.position;
                controller.Block.transform.rotation = Quaternion.Euler(Vector3.zero);
                controller.Block.Drop(true);
                controller.Block.gameObject.SetActive(true);
                controller.BlockBonus = true;

                controller.GrabDisplay.SetActive(false);
                controller.GrabBonus = false;
            }
            else if (_type == Type.Interro)
            {
                controller.GrabDisplay.SetActive(true);
                controller.GrabBonus = true;

                controller.Block.Drop(false);
                controller.Block.gameObject.SetActive(false);
                controller.BlockBonus = false;
            }

            this.gameObject.SetActive(false);
        }
    }
}
