using UnityEngine;
using System.Collections;

public class ShipEye : MonoBehaviour
{
    private int visibleLayer;
    private int disibleLayer;

    private void Awake()
    {
        visibleLayer = LayerMask.NameToLayer(Tags.VisibleLayer);
        disibleLayer = LayerMask.NameToLayer(Tags.DisibleLayer);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.gameObject.layer == disibleLayer)
            other.gameObject.layer = visibleLayer;
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == visibleLayer)
            other.gameObject.layer = disibleLayer;
    }
}
