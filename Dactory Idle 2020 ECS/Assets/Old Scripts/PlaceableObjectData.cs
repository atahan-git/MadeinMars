using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Placeable Object", menuName = "Placeable Object")]
public class PlaceableObjectData : ScriptableObject
{
	public ArrayLayout shape;
	public static Vector2 center = new Vector2(3, 3);

	public enum ItemType { Miner, Furnace, Processor }

	public ItemType myType;

	[Tooltip("Grade 1 factory will only be able to process grade 1 items etc.")]
	public bool[] ItemAbility = new bool[5];

	public Sprite ItemSprite;
}
