using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;


public class MapGenerator : MonoBehaviour  {

    public Tilemap[] tilemaps;
    public Tilemap resources;

    public Vector2Int mapSize = new Vector2Int(200, 200);
    
    const int defaultHeight = 1;
    const int minHeight = 0;
    const int maxHeight = 3;
    
    
    public PlanetSchematic editorMapSchematic;

    public void Clear () {
        foreach (var tilemap in tilemaps) {
            tilemap.ClearAllTiles();
        }
        resources.ClearAllTiles();
	}

    public OreSpawnSettings debugOreSpawnSettings;
    public int[,] debug_heights;
    public Color[] heightColors = new Color[4];
    public void ShowHeightDebug() {
        foreach (var tilemap in tilemaps) {
            for (int x = 0; x < debug_heights.GetLength(0); x++) {
                for (int y = 0; y < debug_heights.GetLength(1); y++) {
                    var pos = new Vector3Int(x, y, 0);
                    var tile = tilemap.GetTile(pos);
                    if (tile != null) {
                        SetTileColor(tilemap, pos, heightColors[debug_heights[x, y]]);
                    }
                }
            }
        }
    }

	public (int[,],int[,]) GenerateMap (PlanetSchematic scheme) {
        
        float seed = Random.Range(0f, 1000f);
        int[,] materials = GenerateArray(mapSize.x, mapSize.y, 0);
        int[,] height = GenerateArray(mapSize.x, mapSize.y, defaultHeight);

        // Create material distribution
        BlackAreas(materials, seed, scheme);
        // Create heightmap
        Meteors(height, seed, scheme);
        Rivers(height, seed, scheme);

        ConvertMaterialsAndHeightToMap(materials, height, scheme);
        Debug.Log("Map successfully generated");

        debug_heights = height;
        return (materials, height);
    }

    public void LoadMap (int[,] materials, int[,] height, PlanetSchematic scheme) {
        ConvertMaterialsAndHeightToMap( materials, height, scheme);
    }

    public int[,] GenerateResources (OreSpawnSettings setting) {
        float seed = Random.Range(0f, 1000f);

        int[,] map = new int[(int)mapSize.x, (int)mapSize.y];

        PerlinNoise(map, seed, new float[] { setting.cutoff * (1f - setting.edgePercent), setting.cutoff * (1f + setting.edgePercent) }, setting.perlinScale);

        UpdateMap(map, resources, setting.tiles);

        return map;
    }

    public void LoadResources (int[,] map, OreSpawnSettings setting) {
        UpdateMap(map, resources, setting.tiles);
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

    public int[,] GenerateArray (int xSize, int ySize, int value) {
        int[,] arr = new int[xSize, ySize];
        for (int x = 0; x < arr.GetLength(0); x++) {
            for (int y = 0; y < arr.GetLength(1); y++) {
                arr[x, y] = value;
            }
        }
        return arr;
    }
    
    public void ConvertMaterialsAndHeightToMap (int[,] materials, int[,] height, PlanetSchematic scheme) {
        int layerCount = 4;
        for (int layerHeight = 0; layerHeight < layerCount; layerHeight++) {
            bool[,] filter = new bool[height.GetLength(0), height.GetLength(1)];
            FilterForHeight(filter, layerHeight);

            var swelled = SwellMap(filter);
            
            SetMap(swelled, tilemaps[layerHeight], layerHeight, true);
            SetMap(filter, tilemaps[layerHeight], layerHeight, false);
        }


        void FilterForHeight(bool[,] _filter, int heightTarget) {
            for (int x = 0; x < materials.GetLength(0); x++) {
                for (int y = 0; y < materials.GetLength(1); y++) {
                    _filter[x, y] = height[x, y] == heightTarget;
                }
            }
        }
        
        
        
        void SetMap(bool[,] filter,Tilemap tilemap, int layerHeight, bool isEdge) {
            var positions = new List<Vector3Int>();
            var tiles = new List<TileBase>();
            for (int x = 0; x < materials.GetLength(0); x++) {
                for (int y = 0; y < materials.GetLength(1); y++) {
                    if (filter[x, y]) {
                        var position = new Vector3Int(x, y, 0);
                        positions.Add(position);
                        tiles.Add(scheme.tileSettings.GetTile(layerHeight, materials[x,y], isEdge));
                    }
                }
            }
            
            tilemap.SetTiles(positions.ToArray(), tiles.ToArray());
            
            for (int x = 0; x < materials.GetLength(0); x++) {
                for (int y = 0; y < materials.GetLength(1); y++) {
                    if (filter[x, y]) {
                        var position = new Vector3Int(x, y, 0);
                        //tilemap.SetTile(position, scheme.tileSettings.GetTile(layerHeight, materials[x,y], isEdge));
                        SetTileColor(tilemap, position, scheme.colorSettings.GetColor(layerHeight, materials[x, y]));
                    }
                }
            }
        }
    }

    void RenderMap(List<Vector3Int> positions, List<TileBase> tiles, List<Color> colors) {
        
    }

    void SetTileColor(Tilemap tilemap, Vector3Int position, Color color) {
        tilemap.SetTileFlags(position, TileFlags.None);
        tilemap.SetColor(position, color);
    }

    public void UpdateMap (int[,] map, Tilemap tilemap, TileBase[] tile) 
    {
        for (int x = 0; x < map.GetLength(0); x++) {
            for (int y = 0; y < map.GetLength(1); y++) {
                for (int i = 0; i < tile.Length; i++) {
                    if (map[x, y] == i + 1) {
                        tilemap.SetTile(new Vector3Int(x, y, 0), tile[i]);
                    }
                }
            }
        }
    }

    public void BlackAreas (int[,] map, float seed, PlanetSchematic scheme) {
        var blackCutoff = scheme.generationSettings.blackCutoff;
        var blackPerlinScale = scheme.generationSettings.blackPerlinScale;
        PerlinNoise(map, seed, blackCutoff, blackPerlinScale);
    }

    public int[,] PerlinNoise (int[,] map, float seed, float cutoff, float perlinScale) {
        return PerlinNoise(map, seed, new float[] { cutoff }, perlinScale);
    }

    public int[,] PerlinNoise (int[,] map, float seed, float[] cutoff, float perlinScale) {

        //Create the Perlin
        for (int x = 0; x < map.GetLength(0); x++) {
            for (int y = 0; y < map.GetLength(1); y++) {
                float sample = Mathf.PerlinNoise(seed + x * perlinScale, seed + y * perlinScale); // 0 - 1

                /*if (sample < cutoff[0]) {
                    map[x, y] = 0;
                } else {
                    map[x, y] = 1;
                }*/
                
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


    public void Meteors (int[,] map, float seed, PlanetSchematic scheme) {
        Random.InitState((int)seed);

        var genSet = scheme.generationSettings;
        var meteorDensity = genSet.meteorDensity;
        var meteorBignessRarity = genSet.meteorBignessRarity;
        var meteorSizeRange = genSet.meteorSizeRange;
        var bigMeteorBoundary = genSet.bigMeteorBoundary;
        var megaMeteorBoundary = genSet.MegaMeteorBoundary;

        int meteorCount = ((map.GetLength(0) * map.GetLength(1)) / (100 * 100)) * meteorDensity;

        var rimTopHeight = 3;
        var rimAroundHeight = 2;
        var generalHeightChange = 1;

        for (int i = 0; i < meteorCount; i++) {
            Vector2 meteorLocation = new Vector2(Random.Range(0, map.GetLength(0)), Random.Range(0, map.GetLength(1)));
            float r = mapValues(Mathf.Pow(Random.value, meteorBignessRarity), 0, 1, meteorSizeRange.x, meteorSizeRange.y);


            if (r < bigMeteorBoundary) {
                AddCircle(map, meteorLocation, r, generalHeightChange);
            }
            else if (r < megaMeteorBoundary) {
                for (int rin = 0; rin < r - 1; rin++) {
                    AddCircle(map, meteorLocation, rin, -generalHeightChange);
                }

                if(Random.value > 0.5f)
                    AddCircle(map, meteorLocation, 1, generalHeightChange);

                SetCircle(map, meteorLocation, r-1, rimAroundHeight);
                SetCircle(map, meteorLocation, r+1, rimAroundHeight);
                SetCircle(map, meteorLocation, r, rimTopHeight);
                
            }
            else {
                for (int rin = 0; rin < r - 2; rin++) {
                    AddCircle(map, meteorLocation, rin, -generalHeightChange);
                }

                AddCircle(map, meteorLocation, 2, generalHeightChange);

                SetCircle(map, meteorLocation, r - 2, rimAroundHeight);
                SetCircle(map, meteorLocation, r - 1, rimAroundHeight);
                SetCircle(map, meteorLocation, r + 1, rimAroundHeight);
                SetCircle(map, meteorLocation, r + 2, rimAroundHeight);
                SetCircle(map, meteorLocation, r, rimTopHeight);
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
    

    public void Rivers (int[,] map, float seed, PlanetSchematic scheme) {
        Random.InitState((int)seed);
        
        var genSet = scheme.generationSettings;
        var riverDensity = genSet.riverDensity;
        var riverLongnessRarity = genSet.riverLongnessRarity;
        var riverLengthRange = genSet.riverLengthRange;
        var riverBignessRarity = genSet.riverBignessRarity;
        var maxRiverWidth = genSet.maxRiverWidth;
        var riverSetRarityBignessDifficulty = genSet.riverSetRarityBignessDifficulty;
        var maxRiverSet = genSet.maxRiverSet;
        var bigRiverBoundary = genSet.bigRiverBoundary;

        int riverCount = ((map.GetLength(0) * map.GetLength(1)) / (100 * 100)) * riverDensity;

        var riverBottomHeight = minHeight;

        for (int i = 0; i < riverCount; i++) {
            Vector2 riverStartLocation = new Vector2(Random.Range(0, map.GetLength(0)), Random.Range(0, map.GetLength(1)));
            Vector2 riverDirection = Random.insideUnitCircle;

            float riverLength = mapValues(Mathf.Pow(Random.value, riverLongnessRarity), 0, 1, riverLengthRange.x, riverLengthRange.y);
            float riverWidth = mapValues(Mathf.Pow(Random.value, riverBignessRarity), 0, 1, 0, maxRiverWidth);
            if (riverWidth < 1)
                riverWidth = 1;

            int riverSetCount = Mathf.CeilToInt( mapValues(Mathf.Pow(Random.value, Mathf.Pow(riverWidth,riverSetRarityBignessDifficulty)), 0, 1, 0, maxRiverSet));


            for (int set = 0; set < riverSetCount; set++) {
                if(riverWidth > bigRiverBoundary)
                    SetLine(map, riverStartLocation + (riverDirection*Random.value* riverLength / 5), riverDirection, riverLength * (1f + Random.value/10f), riverWidth, riverBottomHeight);
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

    public bool[,] SwellMap(bool[,] map) {
        bool[,] temp = new bool[map.GetLength(0), map.GetLength(1)];

        for (int x = 0; x < map.GetLength(0); x++) {
            for (int y = 0; y < map.GetLength(1); y++) {
                if (SampleIfAnyInTheZoneIsTrue(map, x, y)) {
                    temp[x, y] = true;
                }
            }
        }

        return temp;

        bool SampleIfAnyInTheZoneIsTrue(bool[,] source, int _x, int _y) {
            var xMin = Mathf.Max(0, _x - 1);
            var xMax = Mathf.Min(source.GetLength(0), _x + 2);
            var yMin = Mathf.Max(0, _y - 1);
            var yMax = Mathf.Min(source.GetLength(1), _y + 2);
            for (int x = xMin; x < xMax; x++) {
                for (int y = yMin; y < yMax; y++) {
                    if (source[x, y]) {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
