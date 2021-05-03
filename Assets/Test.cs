using System;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] [ResourceLink] private AssetLink prefab;
    
    [SerializeField] [ResourceLink(typeof(GameObject))] private List<AssetLink> prefabs;
    
    [SerializeField] private List<GameObject> gprefabs;

    private void Start()
    {
//        assetkd
    }
}
