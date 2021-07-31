using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(menuName = "MapFilters/RiverGenerator")]
public class RiverGenerator : MapFilter
{
	
	[Tooltip("river count per 100x100 area")]
	public int riverDensity = 20;
	public Vector2 riverLengthRange = new Vector2(10, 200);
	public float riverLongnessRarity = 5f;
	public int maxRiverSet = 8;
	public float riverSetRarityBignessDifficulty = 20;
	public int maxRiverWidth = 5;
	public float riverBignessRarity = 1;
	public int bigRiverBoundary = 2;

	public override void ApplyFilter(ref int[,] heightMap, ref TileSet[,] materialsMap, float seed) {
		Random.InitState((int) seed);

		int riverCount = ((heightMap.GetLength(0) * heightMap.GetLength(1)) / (100 * 100)) * riverDensity;

		var riverBottomHeight = minHeight;

		for (int i = 0; i < riverCount; i++) {
			Vector2 riverStartLocation = new Vector2(Random.Range(0, heightMap.GetLength(0)), Random.Range(0, heightMap.GetLength(1)));
			Vector2 riverDirection = Random.insideUnitCircle;

			float riverLength = MapValues(Mathf.Pow(Random.value, riverLongnessRarity), 0, 1, riverLengthRange.x, riverLengthRange.y);
			float riverWidth = MapValues(Mathf.Pow(Random.value, riverBignessRarity), 0, 1, 0, maxRiverWidth);
			if (riverWidth < 1)
				riverWidth = 1;

			int riverSetCount = Mathf.CeilToInt(MapValues(Mathf.Pow(Random.value, Mathf.Pow(riverWidth, riverSetRarityBignessDifficulty)), 0, 1, 0, maxRiverSet));


			for (int set = 0; set < riverSetCount; set++) {
				if (riverWidth > bigRiverBoundary)
					SetLine(heightMap, riverStartLocation + (riverDirection * Random.value * riverLength / 5), riverDirection, riverLength * (1f + Random.value / 10f), riverWidth, riverBottomHeight);
				else
					AddLine(heightMap, riverStartLocation + (riverDirection * Random.value * riverLength / 5), riverDirection, riverLength * (1f + Random.value / 10f), riverWidth, -1);
				riverStartLocation += Vector2.Perpendicular(riverDirection) * (riverWidth + 1) * (3 + Random.value * 5);
			}
		}
	}
}
