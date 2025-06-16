using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureManager : MonoBehaviour
{
    [System.Serializable]
    public class TagMaterialPair
    {
        public string tag;
        public Material material;
    }

    public List<TagMaterialPair> materialsByTag;

    private Dictionary<string, Material> materialDict;

    void Awake()
    {
        // Convert list to dictionary for faster lookup
        materialDict = new Dictionary<string, Material>();
        foreach (var pair in materialsByTag)
        {
            if (!materialDict.ContainsKey(pair.tag))
                materialDict[pair.tag] = pair.material;
        }

        ApplyMaterials();
        ApplyColliders();
    }

    void ApplyMaterials()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (!obj.activeInHierarchy) continue;

            string objTag = obj.tag;

            if (materialDict.ContainsKey(objTag))
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer == null)
                    renderer = obj.GetComponent<SkinnedMeshRenderer>();

                if (renderer != null && materialDict.ContainsKey(obj.tag) && materialDict[obj.tag] != null)
                {
                    var targetMaterial = materialDict[obj.tag];
                    Material[] newMats = new Material[renderer.sharedMaterials.Length];
                    for (int i = 0; i < newMats.Length; i++)
                    {
                        newMats[i] = targetMaterial;
                    }
                    renderer.materials = newMats;
                }
                else
                {
                    Debug.LogWarning($"Missing material or renderer for: {obj.name} (tag: {obj.tag})");
                }
            }
        }

        Debug.Log("Materials assigned based on tags.");
    }
    void ApplyColliders()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("MAP_Trigger"))
                { continue; }
            if (obj.CompareTag("MAP_NoCol"))
                { continue; }
            if (!obj.GetComponent<MeshCollider>())
                obj.AddComponent<MeshCollider>();
        }
    }
}
