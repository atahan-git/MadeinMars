using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Used just as a stroge object. Should have no functionality of its own
/// </summary>
public class BuildingWorldObject : MonoBehaviour
{

	public BuildingData myData;
	public Position myPos;
	public List<TileData> myTiles;
	public List<BeltBuildingObject> myBelts;
	public BuildingCraftingController myCrafter;

	SpriteGraphicsController myRend;
	
	
	public void PlaceInWorld (BuildingData _myData, Position _location, List<TileData> _myTiles, List<BeltBuildingObject> _buildingBelts, bool spaceLanding) {
		PlaceInWorld(_myData,_location,_myTiles,_buildingBelts);
		if (spaceLanding)
			GetComponentInChildren<SpriteGraphicsController>().DoSpaceLanding(null);
	}

	public float width;
	public float height;
	public void PlaceInWorld (BuildingData _myData, Position _location, List<TileData> _myTiles, List<BeltBuildingObject> _buildingBelts) {
		myData = _myData;
		myPos = _location;
		myTiles = _myTiles;
		myBelts = _buildingBelts;
		myCrafter.myBelts = myBelts;
		myCrafter.myTiles = myTiles;

		width = myData.shape.width;
		height = myData.shape.height;
		Vector3 centerOffset = new Vector3(0.5f, myData.shape.maxHeightFromCenter -1, 0);

		myRend = GetComponentInChildren<SpriteGraphicsController>();
		myRend.transform.localPosition = myData.spriteOffset.vector3() - centerOffset;

		switch (myData.gfxType) {
			case BuildingData.BuildingGfxType.SpriteBased:
				myRend.SetGraphics(myData.gfxSprite, myData.gfxShadowSprite != null? myData.gfxShadowSprite : myData.gfxSprite);
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
		BuildingMaster.myBuildings.Add(myCrafter);
		myCrafter.SetUpCraftingProcesses(myData);
		
		//myRend.DoSpaceLanding();
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
		BuildingMaster.myBuildings.Remove(myCrafter);
		Destroy(gameObject);
	}

	
}
