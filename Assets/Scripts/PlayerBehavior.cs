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
    }

    // Update is called once per frame
    void Update()
    {
        if (!lockView)
        {
            Vector2 mouseDelta = lookAction.ReadValue<Vector2>() * lookSensitivity;
            viewCam.transform.RotateAround(viewCam.transform.position, viewCam.transform.right, -mouseDelta.y);
            transform.RotateAround(viewCam.transform.position, Vector3.up, mouseDelta.x);
        }
        interactPrompt.SetActive(Physics.Raycast(viewCam.transform.position, viewCam.transform.forward, interactDist, LayerMask.GetMask("Prop")));
    }

    void FixedUpdate()
    {
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
            audioSource.PlayOneShot(audio_footstep);
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
            }
        }
    }

    public void OnInteract()
    {
        RaycastHit hit;
        if (Physics.Raycast(viewCam.transform.position, viewCam.transform.forward, out hit, interactDist, LayerMask.GetMask("Prop")))
        {
            GameManager.instance.InteractProp(hit.collider.gameObject.GetComponent<PropBehavior>());
        }
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
