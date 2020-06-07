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
	public List<TileBaseScript> myTiles;

	public SpriteRenderer myRend;

	public void PlaceInWorld (BuildingData _myData, Position _location, List<TileBaseScript> _myTiles) {
		myData = _myData;
		myPos = _location;
		myTiles = _myTiles;

		myRend.sprite = myData.BuildingSprite;
		DataSaver.saveEvent += SaveYourself;
		transform.position = _location.vector3 + myData.spriteOffset.vector3();
	}


	void SaveYourself () {
		DataSaver.ItemsToBeSaved[DataSaver.n] = new DataSaver.BuildingSaveData(myData.uniqueName, myPos);
		DataSaver.n++;
	}

	public void DestroyYourself () {
		foreach (TileBaseScript myTile in myTiles) {
			if (myTile != null)
				myTile.myItem = null;
		}
		Destroy(gameObject);
	}

	void OnDestroy () {
		DataSaver.saveEvent -= SaveYourself;
	}
}
