using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PropBehavior : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip voice;
    public Color dialogueColor = Color.white;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        GetComponent<DialogueSystemTrigger>().conversationConversant = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
