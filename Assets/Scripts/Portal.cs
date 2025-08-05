using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] public Portal linkedPortal;
    public MeshRenderer screen;
    [SerializeField] Camera playerCam;
    [SerializeField] Camera portalCam;
    RenderTexture viewTexture;

    public Vector3 normal;
    public Vector3 origin;
    public Plane plane;
    public int id;
    private Vector3 warpDirection;

    [Header ("Advanced Settings")]
    public float nearClipOffset = 0.05f;
    public float nearClipLimit = 0.2f;

    int playerLastSide;
    [SerializeField] Transform trackedPlayer;

    void Awake()
    {
        playerCam = Camera.main;
        portalCam = GetComponentInChildren<Camera>();

        ProtectScreenFromClipping();

        portalCam.enabled = false;
    }

    void ProtectScreenFromClipping()
    {
        float halfHeight = playerCam.nearClipPlane * Mathf.Tan(playerCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float halfWidth = halfHeight * playerCam.aspect;
        float dstToNearClipPlaneCorner = new Vector3(halfWidth, halfHeight, playerCam.nearClipPlane).magnitude;

        Transform screenT = screen.transform;
        bool camFacingSameDirAsPortal = Vector3.Dot(transform.right, transform.position - playerCam.transform.position) > 0;
        screenT.localScale = new Vector3(dstToNearClipPlaneCorner, screenT.localScale.y, screenT.localScale.z);
        screenT.localPosition = Vector3.right * dstToNearClipPlaneCorner * (camFacingSameDirAsPortal ? 0.5f : -0.5f);
    }

    void CreateViewTexture()
    {
        if (viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height)
        {
            if (viewTexture != null)
            {
                viewTexture.Release();
            }

            viewTexture = new RenderTexture(Screen.width, Screen.height, 0);

            portalCam.targetTexture = viewTexture;

            linkedPortal.screen.material.SetTexture("_MainTex", viewTexture);
        }
    }

    void Start()
    {
        origin += transform.position;
        normal = (transform.rotation * normal).normalized;
        
        if (normal == Vector3.zero) { normal = transform.forward; }

        plane = new Plane(normal, origin);

        warpDirection = linkedPortal.gameObject.transform.position - transform.position;
    }

    void Update()
    {
        Collider[] toggles = Physics.OverlapSphere(transform.position, 50f, LayerMask.GetMask("PortalToggle"));
        foreach (Collider col in toggles)
        {
            Renderer r = col.gameObject.GetComponent<Renderer>();
            float distToCamera = Vector3.Distance(transform.position, portalCam.transform.position);
            float distToTog = Vector3.Distance(transform.position, col.transform.position);

            if (distToTog <= distToCamera)
            {
                r.enabled = false;
            }
            else
            {
                r.enabled = true;
            }
        }
    }

    void LateUpdate()
    {
        if (trackedPlayer != null)
        {
            Vector3 toPortal = trackedPlayer.position - transform.position;
            int portalSide = System.Math.Sign(Vector3.Dot(toPortal, transform.right));

            if (portalSide != playerLastSide)
            {
                Matrix4x4 m = linkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * trackedPlayer.localToWorldMatrix;
                trackedPlayer.position = m.GetColumn(3);
                trackedPlayer.rotation = m.rotation;
                trackedPlayer = null;
            }

            playerLastSide = portalSide;
        }
    }

    public void Render()
    {
        screen.enabled = false;
        CreateViewTexture();

        Matrix4x4 m = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix * playerCam.transform.localToWorldMatrix;
        portalCam.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);

        ProtectScreenFromClipping();
        portalCam.Render();

        screen.enabled = true;
    }
    void OnTriggerEnter(Collider other)
    {
        int otherLayerMask = 1 << other.gameObject.layer;
        if ((otherLayerMask & LayerMask.GetMask("Player")) > 0)
        {
            trackedPlayer = other.transform;
            Vector3 toPortal = trackedPlayer.position - transform.position;
            playerLastSide = System.Math.Sign(Vector3.Dot(toPortal, transform.right));
        }
    }
    void OnTriggerExit(Collider other)
    {
        int otherLayerMask = 1 << other.gameObject.layer;
        if ((otherLayerMask & LayerMask.GetMask("Player")) > 0)
        {
            trackedPlayer = null;
        }
    }
}
