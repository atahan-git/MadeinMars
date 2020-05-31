using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Building Data", menuName = "Building Data")]
public class BuildingData : ScriptableObject {
	public ArrayLayout shape;
	public static Vector2 center = new Vector2(3, 3);

	public enum ItemType { Belt, Miner, Furnace, Processor }

	public ItemType myType;

	[Tooltip("Grade 1 factory will only be able to process grade 1 items etc.")]
	public bool[] BuildingAbility = new bool[5];

	public Sprite BuildingSprite;
}
