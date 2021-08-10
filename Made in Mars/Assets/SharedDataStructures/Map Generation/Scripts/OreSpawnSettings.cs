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


    public bool isPerlinOre = true;
    [Header("Perlin Noise Settings")]
    [Range(0.05f, 0.95f)]
    public float cutoff = 0.75f;
    [Range(0.001f, 0.2f)]
    public float perlinScale = 0.025f;
    [Range(0.001f, 0.2f)]
    public float edgePercent = 0.05f;
    
    
    public bool isRandomSpotsOre = false;
    [Header("Random Spots Settings")]
    [Tooltip("spot count per 100x100 area")]
    public int spotDensity = 25;
    
}
