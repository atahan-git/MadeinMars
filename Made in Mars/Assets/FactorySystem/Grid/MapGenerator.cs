using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;


public class MapGenerator : MonoBehaviour  {

    public Tilemap[] tilemaps;
    public bool[] layersToSwell = new bool[] {true, false, true, false};
    public bool[] layersToSwellAgain = new bool[] {true, false, false, false};
    public Tilemap resources;

    public Vector2Int mapSize = new Vector2Int(200, 200);


    public PlanetSchematic editorMapSchematic;

    public void Clear () {
        foreach (var tilemap in tilemaps) {
            if (tilemap != null) {
                tilemap.ClearAllTiles();
            }
        }
        resources.ClearAllTiles();
	}
    
    public void ClearResources () {
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

	public (TileSet[,],int[,]) GenerateMap (PlanetSchematic scheme) {
        
        float seed = Random.Range(0f, 1000f);
        int[,] height = GenerateArray<int>(mapSize.x, mapSize.y);
        TileSet[,] materials = GenerateArray<TileSet>(mapSize.x, mapSize.y);
        
        //Apply filters
        for (int i = 0; i < scheme.filters.Length; i++) {
            scheme.filters[i].ApplyFilter(ref height, ref materials, seed);
        }


        ConvertMaterialsAndHeightToMap(materials, height, scheme);
        Debug.Log("Map successfully generated");

        debug_heights = height;
        return (materials, height);
    }

    public void LoadMap (TileSet[,] materials, int[,] height, PlanetSchematic scheme) {
        ConvertMaterialsAndHeightToMap( materials, height, scheme);
    }

    public int[,] GenerateResources (OreSpawnSettings setting) {
        float seed = Random.Range(0f, 1000f);

        int[,] map = new int[(int)mapSize.x, (int)mapSize.y];

        if(setting.isPerlinOre)
            MapFilter.PerlinNoise(map, seed, new float[] { setting.cutoff * (1f - setting.edgePercent), setting.cutoff * (1f + setting.edgePercent) }, setting.perlinScale);
        if(setting.isRandomSpotsOre)
            MapFilter.RandomSpots(map, seed, setting.spotDensity);
            
        ConvertMapToResources(map, resources, setting.tiles);

        return map;
    }

    public void LoadResources (int[,] map, OreSpawnSettings setting) {
        ConvertMapToResources(map, resources, setting.tiles);
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

    public T[,] GenerateArray <T> (int xSize, int ySize) {
        return new T[xSize, ySize];
    }
    
    public void ConvertMaterialsAndHeightToMap (TileSet[,] materials, int[,] height, PlanetSchematic scheme) {
        int layerCount = 4;
        for (int layerHeight = 0; layerHeight < layerCount; layerHeight++) {
            bool[,] filter = new bool[height.GetLength(0), height.GetLength(1)];
            FilterForHeight(filter, layerHeight);

            bool[,] swelled;
            if (layersToSwell[layerHeight]) {
                swelled = SwellMap(filter);
            } else {
                swelled = filter;
            }

            if (layersToSwellAgain[layerHeight]) {
                swelled = SwellMap(swelled);
            }

            SetMap(swelled, tilemaps[layerHeight], layerHeight, true);
            //SetMap(filter, tilemaps[layerHeight], layerHeight, false);
        }


        void FilterForHeight(bool[,] _filter, int heightTarget) {
            for (int x = 0; x < materials.GetLength(0); x++) {
                for (int y = 0; y < materials.GetLength(1); y++) {
                    _filter[x, y] = height[x, y] == heightTarget;
                }
            }
        }
        
        
        
        void SetMap(bool[,] filter,  Tilemap tilemap, int layerHeight, bool isEdge) {
            var positions = new List<Vector3Int>();
            var tiles = new List<TileBase>();
            var colors = new List<Color>();
            for (int x = 0; x < materials.GetLength(0); x++) {
                for (int y = 0; y < materials.GetLength(1); y++) {
                    if (filter[x, y]) {
                        var position = new Vector3Int(x, y, 0);
                        positions.Add(position);
                        tiles.Add(materials[x, y].GetTile(layerHeight)); 
                        colors.Add(materials[x, y].GetColor(layerHeight)); 
                    }
                }
            }

            RenderMap(tilemap, positions, tiles, colors);
        }
    }

    void RenderMap(Tilemap tilemap, List<Vector3Int> positions, List<TileBase> tiles, List<Color> colors) {
        tilemap.SetTiles(positions.ToArray(), tiles.ToArray());
        if (colors != null) {
            for (int n = 0; n < colors.Count; n++) {
                SetTileColor(tilemap, positions[n], colors[n]);
            }
        }
        
        //StartCoroutine(RenderMapOverFrames(tilemap, positions, tiles, colors));
        
    }

    public int tilePerFrame = 1000;
    public DataLogger _logger;
    IEnumerator RenderMapOverFrames(Tilemap tilemap, List<Vector3Int> positions, List<TileBase> tiles, List<Color> colors) {

        yield return null;
        
        // For some reason (probably because we are referencing rather than copying) 
        // Waiting a frame then setting the tiles fuck everything up.
        tilemap.SetTiles(positions.ToArray(), tiles.ToArray());
        if (colors != null) {
            for (int n = 0; n < colors.Count; n++) {
                SetTileColor(tilemap, positions[n], colors[n]);
            }
        }
        
        /*Vector3Int[][] positionChunks = MakeChunks<Vector3Int>(positions, tilePerFrame);
        TileBase[][] tileChunks = MakeChunks<TileBase>(tiles, tilePerFrame);
        Color[][] colorChunks = null;
        if(colors != null)
            colorChunks = MakeChunks<Color>(colors, tilePerFrame);
        

        for (int i = 0; i < positionChunks.Length; i++) {
            tilemap.SetTiles(positionChunks[i], tileChunks[i]);
            if (colors != null) {
                var curColorChunk = colorChunks[i];
                var curPositionChunk = positionChunks[i];
                for (int n = 0; n < curColorChunk.Length; n++) {
                    SetTileColor(tilemap, curPositionChunk[n], curColorChunk[n]);
                }
            }

            yield return null;
        }*/

        /*for (int i = 0; i < (positions.Count / tilePerFrame)  + 1; i++) {
            var posChunk = positions.Skip(i * tilePerFrame).Take(tilePerFrame).ToArray();
            var tileChunk = tiles.Skip(i * tilePerFrame).Take(tilePerFrame).ToArray();
            tilemap.SetTiles(posChunk,tileChunk );
            
            
            if (colors != null) {
                var colorChunk = colors.Skip(i * tilePerFrame).Take(tilePerFrame).ToArray();
                for (int n = 0; n < colorChunk.Length; n++) {
                    SetTileColor(tilemap, posChunk[n], colorChunk[n]);
                }
            }

            yield return null;
        }*/

        /*if (colors != null) {
            for (int n = 0; n < colors.Count; n++) {
                SetTileColor(tilemap, positions[n], colors[n]);
            }
        }*/
    }

    T[][] MakeChunks<T>(List<T> source, int chunkSize) {
        return source
            .Select((s, i) => new { Value = s, Index = i })
            .GroupBy(x => x.Index / chunkSize)
            .Select(grp => grp.Select(x => x.Value).ToArray())
            .ToArray();
    }

    void SetTileColor(Tilemap tilemap, Vector3Int position, Color color) {
        tilemap.SetTileFlags(position, TileFlags.None);
        tilemap.SetColor(position, color);
    }

    public void ConvertMapToResources(int[,] map, Tilemap tilemap, TileBase[] tile) {
        var positions = new List<Vector3Int>();
        var tiles = new List<TileBase>();
        for (int x = 0; x < map.GetLength(0); x++) {
            for (int y = 0; y < map.GetLength(1); y++) {
                for (int i = 0; i < tile.Length; i++) {
                    if (map[x, y] == i + 1) {
                        positions.Add(new Vector3Int(x, y, 0));
                        tiles.Add(tile[i]);
                    }
                }
            }
        }

        RenderMap(tilemap, positions, tiles, null);
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
