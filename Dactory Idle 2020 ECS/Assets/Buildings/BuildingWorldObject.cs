using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// The main building object. Should work to store data for the other components, and deal with placing/removing the building.
/// </summary>
public class BuildingWorldObject : MonoBehaviour
{

	public BuildingData myData;
	public Position myPos;
	public List<TileData> myTiles;
	public List<BeltBuildingObject> myBelts;
	BuildingCraftingController myCrafter;
	BuildingInventoryController myInventory;
	BuildingInOutController myInOut;

	SpriteGraphicsController myRend;
	
	
	public void PlaceInWorld (BuildingData _myData, Position _location, List<TileData> _myTiles, List<BeltBuildingObject> _buildingBelts, 
		bool isSpaceLanding, bool isInventory, List<InventoryItemSlot> inventory) {
		_PlaceInWorld(_myData, _location, _myTiles, _buildingBelts, isSpaceLanding, isInventory, inventory);
	}

	public float width;
	public float height;

	void _PlaceInWorld(BuildingData _myData, Position _location, List<TileData> _myTiles, List<BeltBuildingObject> _buildingBelts,
		bool isSpaceLanding, bool isInventory, List<InventoryItemSlot> inventory) {
		myData = _myData;
		myPos = _location;
		myTiles = _myTiles;
		myBelts = _buildingBelts;

		myCrafter = GetComponent<BuildingCraftingController>();
		myInventory = GetComponent<BuildingInventoryController>();
		myInOut = GetComponent<BuildingInOutController>();

		width = myData.shape.width;
		height = myData.shape.height;
		Vector3 centerOffset = new Vector3(0.5f, myData.shape.maxHeightFromCenter - 1, 0);

		myRend = GetComponentInChildren<SpriteGraphicsController>();
		myRend.transform.localPosition = myData.spriteOffset.vector3() - centerOffset;

		switch (myData.gfxType) {
			case BuildingData.BuildingGfxType.SpriteBased:
				myRend.SetGraphics(myData.gfxSprite, myData.gfxShadowSprite != null ? myData.gfxShadowSprite : myData.gfxSprite);
				break;
			case BuildingData.BuildingGfxType.AnimationBased:
				myRend.SetGraphics(myData.gfxSpriteAnimation, myData.isAnimatedShadow);

				break;
			case BuildingData.BuildingGfxType.PrefabBased:
				myRend.SetGraphics(myData.gfxPrefab);
				break;
		}

		DataSaver.saveEvent += SaveYourself;
		transform.position = _location.Vector3(Position.Type.building) + centerOffset;

		BuildingMaster.myBuildingCrafters.Add(myCrafter);
		BuildingMaster.myBuildingInOuters.Add(myInOut);
		
		myCrafter.SetUp(myData, this, myInventory);
		if (isInventory)
			myInventory.SetUp(inventory);
		else
			myInventory.SetUp(myCrafter, myData);
		myInOut.SetUp(this, myData, myInventory);


		if (isSpaceLanding)
			GetComponentInChildren<SpriteGraphicsController>().DoSpaceLanding(null);
	}


	void SaveYourself () {
		DataSaver.ItemsToBeSaved[DataSaver.n] = new DataSaver.BuildingSaveData(myData.uniqueName, myPos);
		DataSaver.n++;
	}

	void OnDestroy () {
		DataSaver.saveEvent -= SaveYourself;
	}

	public void DestroyYourself () {
		foreach (TileData myTile in myTiles) {
			if (myTile != null)
				myTile.myBuilding = null;
		}
		foreach (BeltBuildingObject myBelt in myBelts) {
			if (myBelt != null)
				myBelt.DestroyYourself();
		}
		BuildingMaster.myBuildingCrafters.Remove(myCrafter);
		Destroy(gameObject);
	}

	
}
