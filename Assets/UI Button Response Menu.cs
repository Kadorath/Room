using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class UIButtonResponseMenu : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction interactAction;
    private bool movePressed = false;
    private bool interactPressed = false;
    [SerializeField] int curSelection = -1;

    [SerializeField] List<Selectable> responses;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GameObject.Find("Player").GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        interactAction = playerInput.actions["Interact"];
    }

    void OnEnable()
    {
        responses = new List<Selectable>();
        curSelection = -1;
        movePressed = false;
        interactPressed = false;
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
                responses.Add(child.GetComponent<Selectable>());
        }
    }
    // Update is called once per frame
    void Update()
    {
        float vIn = moveAction.ReadValue<Vector2>().y;
        if (Mathf.Abs(vIn) > 0.01f)
        {
            if (!movePressed)
            {
                int oldSelection = curSelection;
                movePressed = true;
                if (curSelection == -1)
                {
                    if (vIn > 0f)
                        curSelection = responses.Count - 1;
                    else
                        curSelection = 0;
                }
                else
                {
                    curSelection = (int)(curSelection - vIn) % responses.Count;
                    if (curSelection < 0)
                        curSelection = responses.Count - 1;
                }
                responses[curSelection].Select();
                if (oldSelection != -1)
                    responses[oldSelection].GetComponent<StandardUIResponseButton>().label.color = Color.white;
                responses[curSelection].GetComponent<StandardUIResponseButton>().label.color = Color.red;
                Debug.Log(curSelection);
            }
        }
        else
            movePressed = false;

        if (interactAction.ReadValue<float>() > 0.5f)
        {
            if (!interactPressed && curSelection != -1)
            {
                responses[curSelection].GetComponent<StandardUIResponseButton>().OnClick();
                interactPressed = true;
            }
        }
    }
}
