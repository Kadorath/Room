using System.Collections.Generic;
using UnityEngine;

public class MaskRainBehaviour : MonoBehaviour
{
    [SerializeField] List<GameObject> masks;
    private List<GameObject> maskPool;
    [SerializeField] private float elapsedTime = 0f;
    public float spawnRate = 1f;
    private BoxCollider box;

    private Transform player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player").transform;

        GameObject poolHolder = new GameObject("MaskRainPool");
        poolHolder.transform.parent = transform.parent;

        box = GetComponent<BoxCollider>();

        maskPool = new List<GameObject>();
        for (int i = 0; i < 12; i++)
        {
            GameObject newMask = Instantiate(masks[i%masks.Count], poolHolder.transform);
            newMask.SetActive(false);
            maskPool.Add(newMask);
        }
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        transform.position = new Vector3(player.position.x, transform.position.y, player.position.z);

        if (elapsedTime > spawnRate)
        {
            elapsedTime = 0f;

            Bounds bbox = box.bounds;

            GameObject maskToSpawn = null;
            foreach (GameObject m in maskPool)
            {
                if (!m.activeSelf) { maskToSpawn = m; }
            }

            if (maskToSpawn == null)
                maskToSpawn = maskPool[Random.Range(0, maskPool.Count)];

            float x = bbox.min.x + (Random.value * (bbox.max.x - bbox.min.x));
            float y = bbox.min.y + (Random.value * (bbox.max.y - bbox.min.y));
            float z = bbox.min.z + (Random.value * (bbox.max.z - bbox.min.z));

            maskToSpawn.transform.position = new Vector3(x, y, z);
            maskToSpawn.transform.rotation = Random.rotation;

            maskToSpawn.SetActive(true);
        }
    }
}
