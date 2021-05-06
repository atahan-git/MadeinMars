using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// The main building object. Should work to store data for the other components, and deal with placing/removing the building.
/// </summary>
public class BeltWorldObject : MonoBehaviour, IBuildable
{

	public Belt myBelt;
	public Position myPos;
	public int myDir;
	public TileData myTile;
	public bool isBuilt = false;

	SpriteGraphicsController myRend;
	
	public void PlaceInWorld (int _direction, Position _location, TileData _myTile, bool isSpaceLanding, bool _isBuilt) {
		_PlaceInWorld(_direction, _location, _myTile, isSpaceLanding, _isBuilt);
	}

	public float width;
	public float height;

	void _PlaceInWorld(int _direction, Position _location, TileData _myTile, bool isSpaceLanding, bool _isBuilt) {
		myPos = _location;
		myDir = _direction;
		myTile = _myTile;
		isBuilt = _isBuilt;

		myTile.worldObject = this.gameObject;

		myTile.objectUpdatedCallback += TileUpdated;

		myRend = GetComponentInChildren<SpriteGraphicsController>();

		myRend.SetGraphics(FactoryVisuals.s.beltSprites[myDir]);
		myRend.SetBuildState(false);

		DataSaver.saveEvent += SaveYourself;
		transform.position = _location.Vector3(Position.Type.belt) + Vector3.up/2f + Vector3.right/2f;

		if (isSpaceLanding)
			GetComponentInChildren<SpriteGraphicsController>().DoSpaceLanding(null);

		if (isBuilt)
			CompleteBuilding();
	}

	public void CompleteBuilding() {
		myBelt = FactorySystem.s.CreateBelt(myPos, myDir);

		isBuilt = true;
		
		// This is called by TileUpdated Instead
		//myRend.SetGraphics(FactoryVisuals.s.beltSprites[myBelt.direction]);
		myRend.SetBuildState(true);
	}


	void TileUpdated() {
		if (myTile.areThereBelt) {
			myBelt = myTile.myBelt;
			myDir = myBelt.direction;
			myRend.SetGraphics(FactoryVisuals.s.beltSprites[myBelt.direction]);
		} else {
			DestroyYourself();
		}
	}

	void SaveYourself () {
		DataSaver.BeltsToBeSaved.Add(new DataSaver.BeltData(myPos, myDir, isBuilt));
	}

	void OnDestroy () {
		DataSaver.saveEvent -= SaveYourself;
	}

	public void DestroyYourself() {
		if (myTile != null)
			myTile.worldObject = null;

		myTile.objectUpdatedCallback -= TileUpdated;
		FactorySystem.s.RemoveBelt(myPos);
		DroneSystem.s.RemoveDroneTask(myPos);

		Destroy(gameObject);
	}
}
