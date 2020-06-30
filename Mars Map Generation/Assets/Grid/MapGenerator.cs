using UnityEngine;
using UnityEngine.Tilemaps;


public class MapGenerator : MonoBehaviour {

    public Tilemap ground;
    public int zeroHeightInArray = 1;
    public int minHeight = -1;
    public int maxHeight = 2;
    public TileBase[] dirt;
    public TileBase[] black;


    public Tilemap resources;
    public TileBase[] ironTiles;
    public TileBase[] copperTiles;
    public TileBase[] concreteTiles;

    public Vector2 mapSize = new Vector2(200, 200);

    // Start is called before the first frame update
    void Start () {

    }

    public void GenerateMap () {
        float seed = Random.Range(0f, 1000f);
        int[,] materials = new int[(int)mapSize.x, (int)mapSize.y];
        int[,] height = new int[(int)mapSize.x, (int)mapSize.y];

        TileBase[][] PossibleTiles = new TileBase[2][];
        PossibleTiles[0] = dirt;
        PossibleTiles[1] = black;

        // Create material distribution
        BlackAreas(materials, seed);

        // Create heightmap
        Meteors(height, seed);
        Rivers(height, seed);

        RenderMap(ground, materials, height, PossibleTiles);
        print("Map successfully generated");
    }

    [Header("Resource Generation Settings")]
    [Range(0.05f, 0.95f)]
    public float ironCutoff = 0.5f;
    [Range(0.001f, 0.2f)]
    public float ironPerlinScale = 0.05f;

    [Range(0.05f, 0.95f)]
    public float copperCutoff = 0.5f;
    [Range(0.001f, 0.2f)]
    public float copperPerlinScale = 0.05f;

    [Range(0.05f, 0.95f)]
    public float concreteCutoff = 0.5f;
    [Range(0.001f, 0.2f)]
    public float concretePerlinScale = 0.05f;

    [Range(0.001f, 0.2f)]
    public float oreEdgePercent = 0.01f;

    public void GenerateResources () {
        float seed = Random.Range(0f, 1000f);
        int[,] iron = new int[(int)mapSize.x, (int)mapSize.y];
        int[,] copper = new int[(int)mapSize.x, (int)mapSize.y];
        int[,] concrete = new int[(int)mapSize.x, (int)mapSize.y];

        PerlinNoise(iron, seed, new float[] { ironCutoff*(1f- oreEdgePercent), ironCutoff * (1f + oreEdgePercent) }, ironPerlinScale);

        PerlinNoise(copper, seed+1000f, new float[] { copperCutoff * (1f - oreEdgePercent), copperCutoff * (1f + oreEdgePercent) }, copperPerlinScale);

        PerlinNoise(concrete, seed + 2000f, new float[] { concreteCutoff * (1f - oreEdgePercent), concreteCutoff * (1f + oreEdgePercent) }, concretePerlinScale);

        resources.ClearAllTiles();

        UpdateMap(iron, resources, ironTiles);
        UpdateMap(copper, resources, copperTiles);
        UpdateMap(concrete, resources, concreteTiles);
    }


    void DebugIntArray (int[,] array) {
        string debug = "";
        for (int y = 0; y < array.GetLength(0); y++) {
            for (int x = 0; x < array.GetLength(1); x++) {
                debug += array[x, y];
            }
            debug += "\n";
        }

        print(debug);
    }

    public int[,] GenerateArray (Vector2 mapSize, bool filled) {
        int[,] map = new int[(int)mapSize.x, (int)mapSize.y];
        for (int x = 0; x < map.GetLength(0); x++) {
            for (int y = 0; y < map.GetLength(1); y++) {
                if (filled) {
                    map[x, y] = 1;
                } else {
                    map[x, y] = 0;
                }
            }
        }
        return map;
    }
    public void RenderMap (Tilemap tilemap, int[,] materials, int[,] height, TileBase[][] tiles) {
        //Clear the map (ensures we dont overlap)
        tilemap.ClearAllTiles();
        //Loop through the width of the map
        for (int x = 0; x < materials.GetLength(0); x++) {
            //Loop through the height of the map
            for (int y = 0; y < materials.GetLength(1); y++) {
                tilemap.SetTile(new Vector3Int(x, y, 0), tiles[materials[x,y]][height[x,y]+zeroHeightInArray]);
            }
        }
    }

    public void UpdateMap (int[,] map, Tilemap tilemap, TileBase tile) {
        UpdateMap(map, tilemap, new TileBase[] { tile });
    }

    public void UpdateMap (int[,] map, Tilemap tilemap, TileBase[] tile) //Takes in our map and tilemap, setting null tiles where needed
    {
        for (int x = 0; x < map.GetLength(0); x++) {
            for (int y = 0; y < map.GetLength(1); y++) {
                //We are only going to update the map, rather than rendering again
                //This is because it uses less resources to update tiles to null
                //As opposed to re-drawing every single tile (and collision data)
                for (int i = 0; i < tile.Length; i++) {
                    if (map[x, y] == i + 1) {
                        tilemap.SetTile(new Vector3Int(x, y, 0), tile[i]);
                    }
                }
            }
        }
    }


    [Header("Black Areas Settings")]
    [Range(0.05f, 0.95f)]
    public float blackCutoff = 0.5f;
    [Range(0.001f, 0.2f)]
    public float blackPerlinScale = 0.05f;

    public void BlackAreas (int[,] map, float seed) {
        PerlinNoise(map, seed, blackCutoff, blackPerlinScale);
    }

    public int[,] PerlinNoise (int[,] map, float seed, float cutoff, float perlinScale) {
        return PerlinNoise(map, seed, new float[] { cutoff }, perlinScale);
    }

    public int[,] PerlinNoise (int[,] map, float seed, float[] cutoff, float perlinScale) {

        //Create the Perlin
        for (int x = 0; x < map.GetLength(0); x++) {
            for (int y = 0; y < map.GetLength(1); y++) {
                float sample = Mathf.PerlinNoise(seed + x * perlinScale, seed + y * perlinScale);
                //print(sample);
                for (int i = cutoff.Length-1; i >= 0; i--) {
                    if (sample > cutoff[i]) {
                        map[x, y] = i+1;
                        i = -1;
                    }
                }
            }
        }

        return map;
    }

    [Header("Meteor Settings")]
    [Tooltip("meteor count per 100x100 area")]
    public int meteorDensity = 25;
    public Vector2 meteorSizeRange = new Vector2(2, 20);
    public float meteorBignessRarity = 2f;
    public float bigMeteorBoundary = 10f;
    public float MegaMeteorBoundary = 15f;


    public void Meteors (int[,] map, float seed) {
        Random.InitState((int)seed);

        int meteorCount = ((map.GetLength(0) * map.GetLength(1)) / (100 * 100)) * meteorDensity;

        for (int i = 0; i < meteorCount; i++) {
            Vector2 meteorLocation = new Vector2(Random.Range(0, map.GetLength(0)), Random.Range(0, map.GetLength(1)));
            float r = mapValues(Mathf.Pow(Random.value, meteorBignessRarity), 0, 1, meteorSizeRange.x, meteorSizeRange.y);


            if (r < bigMeteorBoundary) {
                AddCircle(map, meteorLocation, r, 1);
            }
            else if (r < MegaMeteorBoundary) {
                for (int rin = 0; rin < r - 1; rin++) {
                    AddCircle(map, meteorLocation, rin, -1);
                }

                if(Random.value > 0.5f)
                    AddCircle(map, meteorLocation, 1, 1);

                SetCircle(map, meteorLocation, r-1, 1);
                SetCircle(map, meteorLocation, r+1, 1);
                SetCircle(map, meteorLocation, r, 2);
                
            }
            else {
                for (int rin = 0; rin < r - 2; rin++) {
                    AddCircle(map, meteorLocation, rin, -1);
                }

                AddCircle(map, meteorLocation, 2, 1);

                SetCircle(map, meteorLocation, r - 2, 1);
                SetCircle(map, meteorLocation, r - 1, 1);
                SetCircle(map, meteorLocation, r + 1, 1);
                SetCircle(map, meteorLocation, r + 2, 1);
                SetCircle(map, meteorLocation, r, 2);
            }

        }
    }

    public void SetCircle (int[,] map, Vector2 center, float r, int num) {
        for (float theta = 0; theta < 2 * Mathf.PI; theta += 1f / (2 * Mathf.PI * r)) {
            int x = Mathf.RoundToInt(center.x + r * Mathf.Cos(theta));
            int y = Mathf.RoundToInt(center.y + r * Mathf.Sin(theta));

            if (x < map.GetLength(0) && y < map.GetLength(1) && x >= 0 && y >= 0) {
                map[x, y] = num;
            }
        }

        /*for (int x = (int)center.x - (int)r; x < (int)center.x + (int)r; x++) {
            for (int y = (int)center.y - (int)r; y < (int)center.y + (int)r; y++) {
                float distancetoCenter = Vector2.Distance(new Vector2(x, y), center);
                if (distancetoCenter > r - width && distancetoCenter < r + width) {
                    if (x < map.GetLength(0) && y < map.GetLength(1) && x >= 0 && y >= 0) {
                        map[x, y] = num;
                    }
                }
            }
        }*/
    }

    public void AddCircle (int[,] map, Vector2 center, float r, int num) {
        int[,] addition = new int[(int)mapSize.x, (int)mapSize.y];
        for (float theta = 0; theta < 2 * Mathf.PI; theta += 1f / (2 * Mathf.PI * r)) {
            int x = Mathf.RoundToInt(center.x + r * Mathf.Cos(theta));
            int y = Mathf.RoundToInt(center.y + r * Mathf.Sin(theta));

            if (x < map.GetLength(0) && y < map.GetLength(1) && x >= 0 && y >= 0) {
                addition[x, y] = num;
            }
        }
        for (int x = 0; x < map.GetLength(0); x++) {
            for (int y = 0; y < map.GetLength(1); y++) {
                map[x, y] += addition[x, y];
                map[x, y] = Mathf.Clamp(map[x, y], minHeight, maxHeight);
            }
        }
    }


    [Header("River settings")]
    [Tooltip("river count per 100x100 area")]
    public int riverDensity = 5;
    public Vector2 riverlengthRange = new Vector2(5, 20);
    public float riverLongnessRarity = 2f;
    public int maxRiverSet = 8;
    public float riverSetRarityBignessDifficulty = 2;
    public int maxRiverWidth = 5;
    public float riverBignessRarity = 10;
    public int bigRiverBoundary = 4;

    public void Rivers (int[,] map, float seed) {
        Random.InitState((int)seed);

        int riverCount = ((map.GetLength(0) * map.GetLength(1)) / (100 * 100)) * riverDensity;

        for (int i = 0; i < riverCount; i++) {
            Vector2 riverStartLocation = new Vector2(Random.Range(0, map.GetLength(0)), Random.Range(0, map.GetLength(1)));
            Vector2 riverDirection = Random.insideUnitCircle;

            float riverLength = mapValues(Mathf.Pow(Random.value, riverLongnessRarity), 0, 1, riverlengthRange.x, riverlengthRange.y);
            float riverWidth = mapValues(Mathf.Pow(Random.value, riverBignessRarity), 0, 1, 0, maxRiverWidth);
            if (riverWidth < 1)
                riverWidth = 1;

            int riverSetCount = Mathf.CeilToInt( mapValues(Mathf.Pow(Random.value, Mathf.Pow(riverWidth,riverSetRarityBignessDifficulty)), 0, 1, 0, maxRiverSet));


            for (int set = 0; set < riverSetCount; set++) {
                if(riverWidth > bigRiverBoundary)
                SetLine(map, riverStartLocation + (riverDirection*Random.value* riverLength / 5), riverDirection, riverLength * (1f + Random.value/10f), riverWidth, -1);
                else
                    AddLine(map, riverStartLocation + (riverDirection * Random.value * riverLength / 5), riverDirection, riverLength * (1f + Random.value / 10f), riverWidth, -1);
                riverStartLocation += Vector2.Perpendicular(riverDirection) * (riverWidth+1)*(3+Random.value * 5);
            }
        }
    }

    public void SetLine (int[,] map, Vector2 start, Vector2 direction, float length, float width, int value) {
        direction = direction.normalized;

        Vector2 cursor = start;

        int maxsteps = Mathf.CeilToInt(length);
        int steps = 0;
        while (steps<maxsteps) {
            if (cursor.x < map.GetLength(0) && cursor.y < map.GetLength(1) && cursor.x >= 0 && cursor.y >= 0) {
                for (int r = 0; r < width; r++) {
                    SetCircle(map, cursor, r, value);
                }
            }

            cursor += direction;

            steps++;
        }

    }
    public void AddLine (int[,] map, Vector2 start, Vector2 direction, float length, float width, int value) {

        int[,] addition = new int[(int)mapSize.x, (int)mapSize.y];
        direction = direction.normalized;

        Vector2 cursor = start;

        int maxsteps = Mathf.CeilToInt(length);
        int steps = 0;
        while (steps < maxsteps) {
            if (cursor.x < map.GetLength(0) && cursor.y < map.GetLength(1) && cursor.x >= 0 && cursor.y >= 0) {
                for (int r = 0; r < width; r++) {
                    SetCircle(addition, cursor, r, value);
                }
            }

            cursor += direction;

            steps++;
        }

        for (int x = 0; x < map.GetLength(0); x++) {
            for (int y = 0; y < map.GetLength(1); y++) {
                map[x, y] += addition[x, y];
                map[x, y] = Mathf.Clamp(map[x, y], minHeight, maxHeight);
            }
        }
    }


    float mapValues (float x, float in_min, float in_max, float out_min, float out_max) {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }


}
