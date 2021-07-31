using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class MapFilter : ScriptableObject  {

	public abstract void ApplyFilter(ref int[,] heightMap, ref TileSet[,] materialsMap, float seed);
	
	
	// Common utilities
	
	
	
	public const int defaultHeight = 1;
	public const int minHeight = 0;
	public const int maxHeight = 3;

	public static int[,] scratchDisk = new int[0,0];
	public static int[,] GetScratchDisk(int[,] map) {
		if (scratchDisk.GetLength(0) != map.GetLength(0) || scratchDisk.GetLength(1) != map.GetLength(1)) {
			scratchDisk = new int[map.GetLength(0), map.GetLength(1)];
		}

		Array.Clear(scratchDisk, 0,scratchDisk.Length);
		return scratchDisk;
	}
	
	public static void PerlinNoise (int[,] map, float seed, float cutoff, float perlinScale) {
		PerlinNoise(map, seed, new float[] { cutoff }, perlinScale);
	}

	public static void PerlinNoise (int[,] map, float seed, float[] cutoff, float perlinScale) {
		//Create the Perlin
		for (int x = 0; x < map.GetLength(0); x++) {
			for (int y = 0; y < map.GetLength(1); y++) {
				float sample = Mathf.PerlinNoise(seed + x * perlinScale, seed + y * perlinScale); // 0 - 1

				map[x, y] = 0;
				for (int i = cutoff.Length-1; i >= 0; i--) {
					if (sample > cutoff[i]) {
						map[x, y] = i+1;
						i = -1;
					}
				}
			}
		}
	}
	
	
	protected static void SetCircle (int[,] map, Vector2 center, float r, int num) {
		for (float theta = 0; theta < 2 * Mathf.PI; theta += 1f / (2 * Mathf.PI * r)) {
			int x = Mathf.RoundToInt(center.x + r * Mathf.Cos(theta));
			int y = Mathf.RoundToInt(center.y + r * Mathf.Sin(theta));

			if (x < map.GetLength(0) && y < map.GetLength(1) && x >= 0 && y >= 0) {
				map[x, y] = num;
			}
		}
	}

	protected static void AddCircle (int[,] map, Vector2 center, float r, int num) {
		int[,] addition = GetScratchDisk(map);
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
	
	protected static void SetLine (int[,] map, Vector2 start, Vector2 direction, float length, float width, int value) {
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
	protected static void AddLine (int[,] map, Vector2 start, Vector2 direction, float length, float width, int value) {

		int[,] addition = GetScratchDisk(map);;
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
	
	protected static float MapValues (float x, float in_min, float in_max, float out_min, float out_max) {
		return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
	}

}
