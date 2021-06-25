using System;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // [SerializeField] private GameObject prefab;
    // [SerializeField] private AssetLink link;

    // [SerializeField] private List<GameObject> prefabs;
    [SerializeField] [Header("16")] [ResourceType(typeof(GameObject))] private List<AssetLink> links;

    // [SerializeField] [ResourceType(typeof(Sprite))] private List<AssetLink> sprites;

    
    private void Start()
    {
        // GameObject p = link;
        // Sprite s = sprites[0].GetAsset<Sprite>();
    }
}