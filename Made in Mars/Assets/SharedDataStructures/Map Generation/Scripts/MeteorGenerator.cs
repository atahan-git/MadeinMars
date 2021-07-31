using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MapFilters/MeteorGenerator")]
public class MeteorGenerator : MapFilter
{
	
	[Tooltip("meteor count per 100x100 area")]
	public int meteorDensity = 25;
    [Tooltip("Radius Range for all meteors")]
	public Vector2 meteorSizeRange = new Vector2(2, 20);
    [Tooltip("Bigger numbers means bigger meteors are rarer")]
	public float meteorBignessRarity = 10f;
    [Tooltip("Radius >= after the meteor sets the rim height instead of just adding a rim")]
	public float bigMeteorBoundary = 6f;
    [Tooltip("Radius >= after the meteor sets a wide rim and a middle circle")]
	public float megaMeteorBoundary = 15f;
	
	public override void ApplyFilter(ref int[,] heightMap, ref TileSet[,] materialsMap, float seed) {
        Random.InitState((int)seed);

        int meteorCount = ((heightMap.GetLength(0) * heightMap.GetLength(1)) / (100 * 100)) * meteorDensity;

        var rimTopHeight = 3;
        var rimAroundHeight = 2;
        var generalHeightChange = 1;

        for (int i = 0; i < meteorCount; i++) {
            Vector2 meteorLocation = new Vector2(Random.Range(0, heightMap.GetLength(0)), Random.Range(0, heightMap.GetLength(1)));
            float r = MapValues(Mathf.Pow(Random.value, meteorBignessRarity), 0, 1, meteorSizeRange.x, meteorSizeRange.y);


            if (r < bigMeteorBoundary) {
                AddCircle(heightMap, meteorLocation, r, generalHeightChange);
            }
            else if (r < megaMeteorBoundary) {
                for (int rin = 0; rin < r - 1; rin++) {
                    AddCircle(heightMap, meteorLocation, rin, -generalHeightChange);
                }

                if(Random.value > 0.5f)
                    AddCircle(heightMap, meteorLocation, 1, generalHeightChange);

                SetCircle(heightMap, meteorLocation, r-1, rimAroundHeight);
                SetCircle(heightMap, meteorLocation, r+1, rimAroundHeight);
                SetCircle(heightMap, meteorLocation, r, rimTopHeight);
                
            }
            else {
                for (int rin = 0; rin < r - 2; rin++) {
                    AddCircle(heightMap, meteorLocation, rin, -generalHeightChange);
                }

                AddCircle(heightMap, meteorLocation, 2, generalHeightChange);

                SetCircle(heightMap, meteorLocation, r - 2, rimAroundHeight);
                SetCircle(heightMap, meteorLocation, r - 1, rimAroundHeight);
                SetCircle(heightMap, meteorLocation, r + 1, rimAroundHeight);
                SetCircle(heightMap, meteorLocation, r + 2, rimAroundHeight);
                SetCircle(heightMap, meteorLocation, r, rimTopHeight);
            }
        }
    }
}
