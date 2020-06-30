using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class OreSpawnSettings : ScriptableObject
{
    [Header("make sure this is the same as with the item set!")]
    public string oreUniqueName = "no name";

    public TileBase[] tiles;

    [Header("Perlin Noise Settings")]
    [Range(0.05f, 0.95f)]
    public float cutoff = 0.75f;
    [Range(0.001f, 0.2f)]
    public float perlinScale = 0.025f;

    [Range(0.001f, 0.2f)]
    public float edgePercent = 0.05f;
}
