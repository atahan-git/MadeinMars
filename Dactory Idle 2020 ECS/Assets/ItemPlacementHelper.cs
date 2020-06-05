using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	public TileBaseScript currentOccupiedTile;

	public GameObject[,] mySprites = new GameObject[7,7];
	public void Setup (BuildingData _myData) {
		myData = _myData;

		for (int y = 0; y < myData.shape.rows.Length; y++) {
			for (int x = 0; x < myData.shape.rows[y].row.Length; x++) {
				if (myData.shape.rows[y].row[x]) {
					//print (x + " - " + y);
					Position myPos = (new Position(x, y) - BuildingData.center);
					//print(myPos);

					mySprites[x,y] = Instantiate(ItemSpritePrefab, myPos.vector3 - Vector3.forward, Quaternion.identity);
					mySprites[x,y].transform.parent = transform;
					ItemSprite mySprite = mySprites[x,y].GetComponent<ItemSprite>();
					mySprite.x = myPos.x;
					mySprite.y = myPos.y;
				}
			}
		}
	}

	void MoveToTile (TileBaseScript toMove) {
		currentOccupiedTile = toMove;
		x = currentOccupiedTile.x;
		y = currentOccupiedTile.y;
		transform.position = currentOccupiedTile.transform.position;
	}


	public void UpdatePosition (TileBaseScript positionTile) {
		MoveToTile(positionTile);
		CheckPlaceable();
	}


    public void CheckPlaceable () {
		isPlaceable = true;
		foreach (GameObject thisSprite in mySprites) {
			if (thisSprite != null) {
				ItemSprite mySprite = thisSprite.GetComponentInChildren<ItemSprite>();

				int checkX = x + mySprite.x;
				int checkY = y + mySprite.y;

				//print (checkX + " - " + checkY);

				bool myVal = false;
				myVal = ObjectBuilderMaster.CheckPlaceable(new Position(checkX, checkY));

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
