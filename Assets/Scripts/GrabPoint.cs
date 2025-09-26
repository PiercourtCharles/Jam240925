using UnityEngine;

public class GrabPoint : MonoBehaviour
{
    public GameObject TargetDisplay;

    private void Start()
    {
        Display(false);
    }

    public void Display(bool value)
    {
        if (TargetDisplay != null)
            TargetDisplay.SetActive(value);
    }
}
