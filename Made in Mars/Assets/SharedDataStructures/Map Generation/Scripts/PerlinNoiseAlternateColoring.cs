using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "MapFilters/PerlinNoiseAlternateColoring")]
public class PerlinNoiseAlternateColoring : MapFilter
{

	public TileSet alternateTileSet;
	
	[Range(0.05f, 0.95f)]
	public float blackCutoff = 0.65f;
	[Range(0.001f, 0.2f)]
	public float blackPerlinScale = 0.013f;

    public override void ApplyFilter(ref int[,] heightMap, ref TileSet[,] materialsMap, float seed) {
	    int[,] perlinOutput = new int[heightMap.GetLength(0), heightMap.GetLength(1)];
	    PerlinNoise(perlinOutput, seed, blackCutoff, blackPerlinScale);

	    for (int y = 0; y < perlinOutput.GetLength(0); y++) {
		    for (int x = 0; x < perlinOutput.GetLength(1); x++) {
			    if (perlinOutput[x, y] == 1) {
				    materialsMap[x, y] = alternateTileSet;
			    }
		    }
	    }
    }
}
