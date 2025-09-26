using UnityEngine;

public class CollectGenerator : MonoBehaviour
{
    [SerializeField] GameObject _obj;
    [SerializeField] float _timer;
    float _time = 0;

    private void Start()
    {
        _time = _timer;
    }

    private void Update()
    {
        if (!_obj.activeSelf)
        {
            _time -= Time.deltaTime;

            if (_time <= 0)
            {
                _obj.SetActive(true);
                _time = _timer;
            }
        }
    }
}
