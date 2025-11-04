using System.Collections.Generic;
using UnityEngine;

public class EmissionHighlighter : IHighlighter
{
    private readonly Dictionary<GameObject, Material[]> originalMaterials = new Dictionary<GameObject, Material[]>();

    public void Highlight (GameObject target, Color color)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        if (!originalMaterials.ContainsKey(target))
        {
            Material[] mats = new Material[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                mats[i] = renderers[i].material;
            }
            originalMaterials[target] = mats;
        }
        foreach (var renderer in renderers)
        {
            Material mat = renderer.material;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", color*0.5f);
        }
    }
    public void RemoveHighlight (GameObject target)
    {
        if (!originalMaterials.TryGetValue(target, out Material[] mats)) return;

        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        
        for (int i = 0; i < renderers.Length && i < mats.Length; i++)
            renderers[i].material = mats[i];

        originalMaterials.Remove(target);
    }
}