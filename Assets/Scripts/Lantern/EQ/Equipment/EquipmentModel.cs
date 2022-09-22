using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EquipmentModel : MonoBehaviour
{
    public List<Renderer> Renderers;

    public void SetLayer(int layer)
    {
        foreach (var r in Renderers)
        {
            r.gameObject.layer = layer;
        }
    }

    #if UNITY_EDITOR
    public void FindRenderers()
    {
        Renderers = GetComponentsInChildren<Renderer>().ToList();
    }
    #endif
}
