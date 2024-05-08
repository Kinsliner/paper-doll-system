using UnityEngine;

public class SwitchActiveKey : MonoBehaviour
{
    public GameObject target;
    public KeyCode keyCode = KeyCode.F12;
    public bool defaultActive;

    private void Start()
    {
        if (target != null)
        {
            target.SetActive(defaultActive);
        }
    }

    private void Update()
    {
        if (target != null)
        {
            if (Input.GetKeyDown(keyCode))
            {
                target.SetActive(!target.activeSelf);
            }
        }
    }
}
