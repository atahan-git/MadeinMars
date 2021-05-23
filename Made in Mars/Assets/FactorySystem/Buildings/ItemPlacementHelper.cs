using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Controls the pre-placement item 'shadow' that you move around with your pointer before placing a building down
/// Spawns and controls the green/red placement blocks
/// </summary>
public class ItemPlacementHelper : MonoBehaviour
{
	[HideInInspector]
	BuildingData myData;

	public GameObject ItemSpritePrefab;

	public GameObject placedItemPrefab;

    [HideInInspector]
    public bool isPlaceable = false;

	public int x = 0;
	public int y = 0;

	public TileData currentOccupiedTile;

	public GameObject[,] mySprites = new GameObject[7,7];
	public void Setup (BuildingData _myData) {
		myData = _myData;

		for (int y = 0; y < myData.shape.column.Length; y++) {
			for (int x = 0; x < myData.shape.column[y].row.Length; x++) {
				if (myData.shape.column[y].row[x]) {
					//print (x + " - " + y);
					Position myPos = (new Position(x, y) - BuildingData.center);
					//print(center);

					mySprites[x,y] = Instantiate(ItemSpritePrefab, myPos.Vector3(Position.Type.itemPlacementHelper) - Vector3.forward, Quaternion.identity);
					mySprites[x,y].transform.parent = transform;
					ItemPlacementSpriteHelper mySprite = mySprites[x,y].GetComponent<ItemPlacementSpriteHelper>();
					mySprite.x = myPos.x;
					mySprite.y = myPos.y;
				}
			}
		}
	}

	void MoveToTile (TileData toMove) {
		currentOccupiedTile = toMove;
		x = currentOccupiedTile.x;
		y = currentOccupiedTile.y;
		transform.position = currentOccupiedTile.location.Vector3(Position.Type.itemPlacementHelper) + new Vector3(0.5f, 0.5f, 0);
	}


	public void UpdatePosition (TileData positionTile) {
		MoveToTile(positionTile);
		CheckPlaceable();
	}


    public void CheckPlaceable () {
		isPlaceable = true;
		foreach (GameObject thisSprite in mySprites) {
			if (thisSprite != null) {
				ItemPlacementSpriteHelper mySprite = thisSprite.GetComponentInChildren<ItemPlacementSpriteHelper>();

				int checkX = x + mySprite.x;
				int checkY = y + mySprite.y;

				//print (checkX + " - " + checkY);

				bool myVal = false;
				
				myVal = FactoryPlayerConnector.s.CheckPlaceable(new Position(checkX, checkY));

				if (myVal) {
					mySprite.Placeable();
				} else {
					mySprite.CantPlace();
					isPlaceable = false;
				}
			}
		}
	}


	public void PlaceSelf () {
		//Debug.Break();
		Destroy(gameObject);
	}

	// Create different particle effects/sound etc.
	public void FailedPlacingSelf () {
		PlaceSelf();
	}
}
