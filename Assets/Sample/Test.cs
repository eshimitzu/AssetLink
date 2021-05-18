using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private AssetLink prefab;
    [SerializeField] private List<AssetLink> prefabs;
    [SerializeField] private AssetLink[] prefabs2;

    [SerializeField] private List<GameObject> gprefabs;

    private void Start()
    {
        GameObject p = prefabs[0];
    }
}
