using UnityEngine;

public class BillboardScript : MonoBehaviour
{
    [SerializeField] private Transform lookTarget;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lookTarget = GameObject.Find("Player").transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(lookTarget);
        transform.Rotate(0f, 184f, 0f);
        transform.rotation = Quaternion.Euler(0f, transform.localEulerAngles.y, 0f);
    }
}
