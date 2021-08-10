using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Holds all the building data. This will be passed around the RecipeSets, building world objects, etc.
/// </summary>
[CreateAssetMenu]
public class BuildingData : ScriptableObject {
	public string uniqueName = "Base_newbuilding"; // The universal identifier. 
	
	public string name = "New Building";
	[TextArea] public string description = "This is a new Item";

	public ArrayLayout shape;
	public static Vector2 center = new Vector2(3, 3);

	// Make sure to also add new crafting type to RecipeSet/CraftingNode if you are adding new crafting type building!
	public enum ItemType { Belt, Miner, Furnace, AssemblerSingle, AssemblerDouble, Press, Coiler, Cutter, Lab, Building, Base, Decal, Storage, Connector, Spaceship, ShipCard , Processor}

	public ItemType myType;

	[Tooltip("Grade 1 factory will only be able to process grade 1 items etc. Also determines miner range")]
	public int myTier = 0;

	public float energyUse = 0;

	public enum BuildingGfxType {
		SpriteBased, AnimationBased, PrefabBased
	}

	[Header("In the world, only one of these will be used")]
	public BuildingGfxType gfxType = BuildingGfxType.SpriteBased;
	[Header("but always set the sprite for UI uses")]
	public Sprite gfxSprite;
	public SpriteAnimationHolder gfxSpriteAnimation;
	public bool copySpriteAnimationToShadow = true;
	public GameObject gfxPrefab;
	[Space]
	public Sprite gfxShadowSprite;
	public SpriteAnimationHolder gfxShadowAnimation;
	
	[Space]
	public Vector2 spriteOffset = new Vector2(0.5f, 0.5f);

	public bool playerBuildBarApplicable = true;

	public bool makesSpaceLanding = false;
	public float spaceLandingXDisp = 0f;

	public Sprite GetSprite() {
		return gfxSprite;
	}
}
