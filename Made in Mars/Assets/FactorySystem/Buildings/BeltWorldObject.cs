using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// The main building object. Should work to store data for the other components, and deal with placing/removing the building.
/// </summary>
public class BeltWorldObject : MonoBehaviour {

	[SerializeField] bool isConstruction;
	[SerializeField] Belt myBelt;
	[SerializeField] Construction myConstruction;
	[SerializeField] TileData myTile;
	[SerializeField] Position location;
	[SerializeField] int direction;

	SpriteGraphicsController myRend;

	[SerializeField] float width;
	[SerializeField] float height;

	public void UpdateSelf(Position _location, Belt _belt) {
		RemoveSelfFromTile();
		myBelt = _belt;
		direction = myBelt.direction;
		isConstruction = false;
		

		/*if (isSpaceLanding)
			GetComponentInChildren<SpriteGraphicsController>().DoSpaceLanding(null);*/
		
		
		myRend = GetComponentInChildren<SpriteGraphicsController>();
		GenericUpdateSelf(_location);
		myRend.SetBuildState(SpriteGraphicsController.BuildState.built);
	}
	
	public void UpdateSelf(Position _location, Construction _construction) {
		RemoveSelfFromTile();
		myConstruction = _construction;
		direction = myConstruction.direction;
		isConstruction = true;

		/*if (isSpaceLanding)
			GetComponentInChildren<SpriteGraphicsController>().DoSpaceLanding(null);*/
		
		myRend = GetComponentInChildren<SpriteGraphicsController>();
		GenericUpdateSelf(_location);
		if (myConstruction.isConstruction) {
			myRend.SetBuildState(SpriteGraphicsController.BuildState.construction);
		} else {
			myRend.SetBuildState(SpriteGraphicsController.BuildState.destruction);
		}
	}

	void GenericUpdateSelf(Position _location) {
		myTile = Grid.s.GetTile(_location);
		location = _location;
		
		myTile.worldObject = this.gameObject;
		myTile.objectUpdatedCallback -= TileUpdated; // we want to make sure we get the update callback only once
		myTile.objectUpdatedCallback += TileUpdated;
		
		transform.position = _location.Vector3(Position.Type.belt) + Vector3.up / 2f + Vector3.right / 2f;
		myRend.SetGraphics(FactoryVisuals.s.beltSprites[direction]);
	}
	
	void TileUpdated() {
		if (isConstruction) {
			if (myTile.areThereConstruction) {
				myConstruction = myTile.myConstruction;
				direction = myConstruction.direction;
					
				myRend.SetGraphics(FactoryVisuals.s.beltSprites[myConstruction.direction]);
			} else {
				DestroyYourself();
			}
		} else {
			if (myTile.areThereBelt) {
				myBelt = myTile.myBelt;
				direction = myBelt.direction;
			
				myRend.SetGraphics(FactoryVisuals.s.beltSprites[myBelt.direction]);
			} else {
				DestroyYourself();
			}
		}
	}

	public void RemoveSelfFromTile() {
		myTile.worldObject = null;
		myTile.objectUpdatedCallback -= TileUpdated;
	}

	public void DestroyYourself() {
		RemoveSelfFromTile();
		GetComponent<PooledGameObject>().DestroyPooledObject();
	}
}
