using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MapFilters/MountainGenerator")]
public class MountainGenerator : MapFilter {

	[Range(0f, 1f)]
	public float[] mountainLayerCutoffs = new float[] {
		0.35f, 0.50f, 0.65f
	};
		
		[Range(0.001f, 0.2f)]
	public float perlinScale = 0.013f;
	
	public override void ApplyFilter(ref int[,] heightMap, ref TileSet[,] materialsMap, float seed) {
		PerlinNoise(heightMap, seed, mountainLayerCutoffs, perlinScale);
	}
}
