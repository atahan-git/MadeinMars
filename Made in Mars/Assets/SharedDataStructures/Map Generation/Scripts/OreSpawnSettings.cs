using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(menuName = "OreSpawnSettings/OreSpawnSetting")]
public class OreSpawnSettings : ScriptableObject {
    public Item oreItem;

    public string oreUniqueName {
        get {
            return oreItem.uniqueName;
        }
    }

    public TileBase[] tiles;

    [Header("Perlin Noise Settings")]
    [Range(0.05f, 0.95f)]
    public float cutoff = 0.75f;
    [Range(0.001f, 0.2f)]
    public float perlinScale = 0.025f;

    [Range(0.001f, 0.2f)]
    public float edgePercent = 0.05f;
}
