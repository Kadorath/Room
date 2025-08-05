using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaticBehavior : MonoBehaviour
{
    public List<Sprite> staticSprites;
    public float speed = 1f;
    private float elapsedTime = 0f;
    private int currentInd = 0;
    private Image img;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        img = GetComponent<Image>();        
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= speed)
        {
            elapsedTime = 0f;
            currentInd += 1;
            currentInd %= staticSprites.Count;
            img.sprite = staticSprites[currentInd];
        }
    }
}
