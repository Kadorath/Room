using System.Collections.Generic;
using UnityEngine;

public class PortalCameraBehaviour : MonoBehaviour
{
    private List<Renderer> clipped;

    void Start()
    {
        clipped = new List<Renderer>();   
    }

    void Update()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, -transform.localPosition, LayerMask.GetMask("PortalToggle"));
        List<Renderer> rendsHit = new List<Renderer>();
        foreach (RaycastHit hit in hits)
        {
            Renderer rend = hit.collider.gameObject.GetComponent<Renderer>();
            rendsHit.Add(rend);
        }

        for (int i = 0; i < clipped.Count; i ++)
        {
            Renderer r = clipped[i];
            if (!rendsHit.Contains(r))
            {
                r.enabled = true;
                clipped.Remove(r);
                i--;
            }
        }
    }

    // void OnTriggerEnter(Collider other)
    // {
    //     int otherLayerMask = 1 << other.gameObject.layer;
    //     if ((otherLayerMask & LayerMask.GetMask("PortalToggle")) > 0)
    //     {
    //         Renderer rend = other.transform.gameObject.GetComponent<Renderer>();
    //         if (!clipped.Contains(rend))
    //         {
    //             clipped.Add(rend);
    //             rend.enabled = false;
    //         }
    //     }
    // }
}
