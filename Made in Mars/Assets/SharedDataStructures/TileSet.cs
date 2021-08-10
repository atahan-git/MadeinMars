using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(menuName = "MapFilters/TileSet", order = 0)]
public class TileSet : ScriptableObject {

    [Header("IMPORTANT! Make sure this id matches the dataholder array index")]
    public int tileSetId = 0;

    public TileBase[] tiles = new TileBase[4];
    public Color[] colors = new Color[4];
    
    
    public Color GetColor(int height) {
        Assert.IsTrue(height < colors.Length);
        Assert.IsTrue(height >= 0);
        return colors[height];
    }
    
    public TileBase GetTile(int height) {
        Assert.IsTrue(height < colors.Length);
        Assert.IsTrue(height >= 0);
        return tiles[height];
    }
}
