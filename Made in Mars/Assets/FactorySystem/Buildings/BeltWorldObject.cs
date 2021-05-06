using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// The main building object. Should work to store data for the other components, and deal with placing/removing the building.
/// </summary>
public class BeltWorldObject : MonoBehaviour, IBuildable {

	public Belt myBelt;
	public Position myPos;
	public int myDir;
	public TileData myTile;
	public bool isBuilt = false;

	BuildingInventoryController myInventory;

	SpriteGraphicsController myRend;

	public float width;
	public float height;

	public void PlaceInWorld(int _direction, Position _location, TileData _myTile, bool isSpaceLanding, bool _isBuilt, List<InventoryItemSlot> inventory) {
		myPos = _location;
		myDir = _direction;
		myTile = _myTile;
		isBuilt = _isBuilt;

		CreateConstructionInventory(inventory);

		myTile.worldObject = this.gameObject;

		myRend = GetComponentInChildren<SpriteGraphicsController>();

		myRend.SetGraphics(FactoryVisuals.s.beltSprites[myDir]);
		myRend.SetBuildState(SpriteGraphicsController.BuildState.construction);

		DataSaver.saveEvent += SaveYourself;
		transform.position = _location.Vector3(Position.Type.belt) + Vector3.up / 2f + Vector3.right / 2f;

		if (isSpaceLanding)
			GetComponentInChildren<SpriteGraphicsController>().DoSpaceLanding(null);

		if (isBuilt)
			CompleteBuilding();
	}

	public BuildingInventoryController GetConstructionInventory() {
		return myInventory;
	}

	public BuildingInventoryController CreateConstructionInventory(List<InventoryItemSlot> inventory) {
		myInventory = new BuildingInventoryController();
		myInventory.SetUpConstruction(myPos);
		myInventory.SetInventory(inventory);
		
		return myInventory;
	}

	public void CompleteBuilding() {
		myBelt = FactorySystem.s.CreateBelt(myPos, myDir);

		isBuilt = true;
		
		myTile.objectUpdatedCallback += TileUpdated;
		
		
		myRend.SetGraphics(FactoryVisuals.s.beltSprites[myBelt.direction]);
		myRend.SetBuildState(SpriteGraphicsController.BuildState.built);
	}


	void TileUpdated() {
		if (myTile.areThereBelt) {
			myBelt = myTile.myBelt;
			myDir = myBelt.direction;
			myRend.SetGraphics(FactoryVisuals.s.beltSprites[myBelt.direction]);
		} else {
			MarkForDeconstruction();
		}
	}

	void SaveYourself () {
		DataSaver.BeltsToBeSaved.Add(new DataSaver.BeltData(myPos, myDir, isBuilt, myInventory.inventory));
	}

	void OnDestroy () {
		DataSaver.saveEvent -= SaveYourself;
	}
	
	public bool isMarkedForDestruction = false;
	public void MarkForDeconstruction() {
		if (!isMarkedForDestruction) {
			if (isBuilt) {
				isMarkedForDestruction = true;
				isBuilt = false;
				DroneSystem.s.AddDroneDestroyTask(myPos, FactoryBuilder.s.connectorBuildingData);
				myRend.SetBuildState(SpriteGraphicsController.BuildState.destruction);
				
				myTile.objectUpdatedCallback -= TileUpdated;
				FactorySystem.s.RemoveBelt(myPos);
				
			} else {
				DestroyYourself();
			}
		}
	}

	public void DestroyYourself() {
		if (myTile != null)
			myTile.worldObject = null;

		if (isBuilt) {
			myTile.objectUpdatedCallback -= TileUpdated;
			FactorySystem.s.RemoveBelt(myPos);
		}

		DroneSystem.s.RemoveDroneTask(myPos);

		Destroy(gameObject);
	}
}
