using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EmissionHighlighter : IHighlighter
{
    private readonly Dictionary<GameObject, Material[]> originalMaterials = new Dictionary<GameObject, Material[]>();
    private readonly Dictionary<GameObject, Material[]> highlightMaterial = new Dictionary<GameObject, Material[]>();
    
    public void Highlight (GameObject target, Color color)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        if (!originalMaterials.ContainsKey(target))
        {
            originalMaterials[target] = renderers.Select(r => r.sharedMaterial).ToArray();
            
            Material[] mats = new Material[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                mats[i] = new Material(renderers[i].sharedMaterial);
                mats[i].EnableKeyword("_EMISSION");
                mats[i].SetColor("_EmissionColor", color*0.5f);
                renderers[i].material = mats[i];
            }
            highlightMaterial[target] = mats;
        }
    }
    
    public void RemoveHighlight (GameObject target)
    {
        if (!originalMaterials.TryGetValue(target, out Material[] originals)) return;

        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length && i < originals.Length; i++)
            renderers[i].material = originals[i];

        if (highlightMaterial.TryGetValue(target, out Material[] mats))
        {
            foreach (var mat in mats)
                if (mat != null) Object.Destroy(mat);
            highlightMaterial.Remove(target);
        }
        originalMaterials.Remove(target);
    }
}