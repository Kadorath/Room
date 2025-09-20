using System.Collections.Generic;
using UnityEngine;

public class CameraTestBehaviour : MonoBehaviour
{
    Camera cam;
    public List<Portal> portals;
    
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    static bool VisibleFromCamera(Renderer renderer, Camera camera)
    {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
    }

    void OnPreCull()
    {
        foreach (Portal p in portals)
        {
            if (VisibleFromCamera(p.screen, cam))
            {
                p.linkedPortal.Render();
            }
        }
    }
}
