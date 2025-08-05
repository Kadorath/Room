using UnityEngine;

public class ObeliskBehavior : MonoBehaviour
{
    [SerializeField] private bool isVisible = false;
    private float delta;
    private float baseY;
    private float amplitude;
    public float rotateSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        delta = Random.value;
        baseY = transform.position.y;
        amplitude = 0.5f;
        rotateSpeed = 0.1f * Random.value;
    }

    // Update is called once per frame
    void Update()
    {
        if (isVisible)
        {
            delta += Time.deltaTime;
            if (delta > Mathf.PI * 2f) { delta = 0f; }

            transform.position = new Vector3(transform.position.x, baseY + Mathf.Sin(delta) * amplitude, transform.position.z);
            transform.Rotate(Vector3.up, rotateSpeed);
        }
    }

    void OnBecameInvisible()
    {
        isVisible = false;
    }

    void OnBecameVisible()
    {
        isVisible = true;
    }
}
