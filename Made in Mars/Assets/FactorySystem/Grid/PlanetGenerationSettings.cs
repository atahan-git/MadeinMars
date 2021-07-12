using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class PlanetGenerationSettings : ScriptableObject
{
	[Header("Alternate Material Generation Settings")]
	[Range(0.05f, 0.95f)]
	public float blackCutoff = 0.5f;
	[Range(0.001f, 0.2f)]
	public float blackPerlinScale = 0.05f;
	
	[Header("Meteor Settings")]
	[Tooltip("meteor count per 100x100 area")]
	public int meteorDensity = 25;
	public Vector2 meteorSizeRange = new Vector2(2, 20);
	public float meteorBignessRarity = 2f;
	public float bigMeteorBoundary = 10f;
	public float MegaMeteorBoundary = 15f;
	
	
	[Header("River settings")]
	[Tooltip("river count per 100x100 area")]
	public int riverDensity = 5;
	public Vector2 riverLengthRange = new Vector2(5, 20);
	public float riverLongnessRarity = 2f;
	public int maxRiverSet = 8;
	public float riverSetRarityBignessDifficulty = 2;
	public int maxRiverWidth = 5;
	public float riverBignessRarity = 10;
	public int bigRiverBoundary = 4;

}
