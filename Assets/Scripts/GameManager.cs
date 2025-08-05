using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public TMP_Text dialogue;
    private Coroutine dialogueCoroutine;

    public Transform church;
    public GameObject obelisk;

    public bool enableObelisks = true;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;

            if (enableObelisks)
            {
                for (int z = 0; z < 3; z++)
                {
                    float r = UnityEngine.Random.value * 10f;
                    for (int i = 0; i < 100; i++)
                    {
                        for (int j = 0; j < 100; j++)
                        {
                            GameObject obe = Instantiate(obelisk, new Vector3(i * 10 + r, (10 * z) + 3.92f, j * 10 + r), Quaternion.identity);
                            if (UnityEngine.Random.value < 0.01f)
                            {
                                obe.transform.GetChild(0).gameObject.SetActive(true);
                            }
                        }
                    }
                }

                if (church != null)
                {
                    Collider[] obelisksToClear = Physics.OverlapSphere(church.position, 30f, LayerMask.GetMask("Obelisk"));
                    foreach (Collider obelisk in obelisksToClear)
                    {
                        Destroy(obelisk.gameObject);
                    }
                }
            }
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool InteractProp(PropBehavior prop)
    {
        if (dialogueCoroutine != null) { StopCoroutine(dialogueCoroutine); }
        
        string newDialogue;   
        newDialogue = prop.GetDialogueLine();
        dialogue.color = prop.dialogueColor;
        dialogueCoroutine = StartCoroutine(PlayDialogue(newDialogue, prop.voice, prop.audioSource));
        return true;
    }

    private IEnumerator PlayDialogue(string text, AudioClip v, AudioSource audioSource)
    {
        dialogue.text = "";
        for (int i = 0; i < text.Length; i++)
        {
            dialogue.text += text[i];
            if (UnityEngine.Random.value < 0.5) { audioSource.PlayOneShot(v); }
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(3f);

        dialogue.text = "";
        dialogueCoroutine = null;
    }
}
