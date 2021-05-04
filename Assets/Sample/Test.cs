using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private AssetLink prefab;
    [SerializeField] [ResourceType(typeof(Sprite))] private List<AssetLink> prefabs;
    [SerializeField] private List<GameObject> gprefabs;

    private void Start()
    {
        var cube = prefab.GetInstance<GameObject>();
        
        var sprite = prefabs[0].GetAsset<Sprite>();
        var g = new GameObject();
        var sp = g.AddComponent<SpriteRenderer>();
        sp.sprite = sprite;
    }
}
