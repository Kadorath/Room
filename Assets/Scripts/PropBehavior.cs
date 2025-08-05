using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PropBehavior : MonoBehaviour
{
    [SerializeField] private List<string> script;
    [SerializeField] private int lineInd = 0;
    public AudioSource audioSource;
    public AudioClip voice;
    public Color dialogueColor = Color.white;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public string GetDialogueLine()
    {
        string line = script[lineInd];
        lineInd += 1;
        lineInd %= script.Count;
        return line;
    }
}
