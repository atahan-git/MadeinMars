using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MapFilters/BaseMap")]
public class BaseMap : MapFilter {

	public int baseHeight = 1;
	public TileSet baseTileSet;
	
	public override void ApplyFilter(ref int[,] heightMap, ref TileSet[,] materialsMap, float seed) {
		for (int x = 0; x < heightMap.GetLength(0); x++) {
			for (int y = 0; y < heightMap.GetLength(1); y++) {
				heightMap[x, y] = baseHeight;
				materialsMap[x, y] = baseTileSet;
			}
		}
	}
}
