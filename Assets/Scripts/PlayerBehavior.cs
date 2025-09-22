using System.Collections;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehavior : MonoBehaviour
{
    private const float EPS_F = 0.00001f;

    private Rigidbody rb;
    [SerializeField] private GameObject viewCam;

    // Player input control
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    [Header("Player Control")]
    public float speed = 1f;
    public float gravityScale = 1f;
    public bool lockView = false;
    public float lookSensitivity = 0.1f;
    public float interactDist = 6f;
    // Camera bob control
    private bool cameraBobbing = false;
    private float cameraBobA = 0f;
    private float cameraHeight;
    private float bobDelay;
    [Header("Camera Bob")]
    public float bobFreq = 0.35f;
    public float bobAmp = 0.05f;

    // Audio
    private AudioSource audioSource;
    [Header("Audio")]
    public AudioClip audio_footstep;

    // UI
    public GameObject interactPrompt;

    // Dialogue Fields
    private Transform focusTarget;
    private Coroutine focusCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();

        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];

        cameraHeight = viewCam.transform.localPosition.y;
        bobDelay = bobFreq;

        DialogueManager.instance.conversationEnded += OnDialogueClose;
    }

    // Update is called once per frame
    void Update()
    {
        if (lockView) { return; }
        Vector2 mouseDelta = lookAction.ReadValue<Vector2>() * lookSensitivity;
        viewCam.transform.RotateAround(viewCam.transform.position, viewCam.transform.right, -mouseDelta.y);
        transform.RotateAround(viewCam.transform.position, Vector3.up, mouseDelta.x);
        interactPrompt.SetActive(Physics.Raycast(viewCam.transform.position, viewCam.transform.forward, interactDist, LayerMask.GetMask("Prop")));
    }

    void FixedUpdate()
    {
        if (lockView) { return; }
        Vector2 moveIn = moveAction.ReadValue<Vector2>() * speed;
        rb.AddForce(Physics.gravity * (gravityScale - 1), ForceMode.Acceleration);
        rb.linearVelocity = transform.forward * moveIn.y + transform.right * moveIn.x + transform.up * rb.linearVelocity.y;

        UpdateCameraBob(Mathf.Abs(moveIn.x) > EPS_F || Mathf.Abs(moveIn.y) > EPS_F);
    }

    private void UpdateCameraBob(bool isMoving)
    {
        if (!cameraBobbing && bobDelay >= bobFreq && isMoving)
        {
            cameraBobbing = true;
        }

        if (cameraBobbing)
        {
            cameraBobA += 0.16f;
            float bobMod = Mathf.Sin(cameraBobA) * bobAmp;
            viewCam.transform.localPosition =
                new Vector3(viewCam.transform.localPosition.x, cameraHeight + bobMod, viewCam.transform.localPosition.z);

            if (cameraBobA > 3.14f)
            {
                viewCam.transform.localPosition =
                    new Vector3(viewCam.transform.localPosition.x, cameraHeight + bobMod, viewCam.transform.localPosition.z);
                cameraBobA = 0f;
                cameraBobbing = false;
                bobDelay = 0f;
            }
        }
        else if (bobDelay < bobFreq)
        {

            bobDelay += Time.fixedDeltaTime;
            if (bobDelay > bobFreq)
            {
                cameraBobbing = false;
                if (moveAction.ReadValue<Vector2>() != Vector2.zero) audioSource.PlayOneShot(audio_footstep);
            }
        }
    }

    public void OnInteract()
    {
        if (lockView) { return; }
        RaycastHit hit;
        if (Physics.Raycast(viewCam.transform.position, viewCam.transform.forward, out hit, interactDist, LayerMask.GetMask("Prop")))
        {
            if (GameManager.instance.InteractProp(hit.collider.gameObject.GetComponent<PropBehavior>()))
            {
                FocusCamera(hit.transform);
            }
        }
    }

    private void FocusCamera(Transform target)
    {
        lockView = true;
        interactPrompt.SetActive(false);
        Transform head = target.Find("HeadPoint");
        focusCoroutine = StartCoroutine(SlerpCameraToTarget(head ? head : target, 1f));
    }

    public void OnDialogueClose(Transform actor)
    {
        lockView = false;
        if (focusCoroutine != null)
            StopCoroutine(focusCoroutine);
    }

    IEnumerator SlerpCameraToTarget(Transform target, float duration)
    {
        float elapsed = 0f;
        Vector3 a = viewCam.transform.forward;
        Vector3 b = (target.position - viewCam.transform.position).normalized;
        while (elapsed < duration)
        {
            float cubic = 1f - Mathf.Pow(1 - elapsed / duration, 3);
            viewCam.transform.LookAt(viewCam.transform.position + Vector3.Slerp(a, b, cubic));

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        transform.localRotation = Quaternion.Euler(0f, viewCam.transform.localRotation.eulerAngles.y + transform.rotation.eulerAngles.y, 0f);
        viewCam.transform.localRotation = Quaternion.Euler(viewCam.transform.localRotation.eulerAngles.x, 0f, 0f);

    }

    public void OnJump()
    {
        //lockView = !lockView;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(viewCam.transform.position, viewCam.transform.forward * interactDist);
    }
}
