using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPlacementHelper : MonoBehaviour
{
    [HideInInspector]
    PlaceableObjectData myObject;

    public GameObject ItemSpritePrefab;

	public GameObject placedItemPrefab;

    [HideInInspector]
    public bool isPlaceable = false;

	public int x = 0;
	public int y = 0;

	public TileBaseScript currentOccupiedTile;

	public GameObject[,] mySprites = new GameObject[7,7];
	public void Setup (PlaceableObjectData _myObject) {
		myObject = _myObject;

		for (int y = 0; y < myObject.shape.rows.Length; y++) {
			for (int x = 0; x < myObject.shape.rows[y].row.Length; x++) {
				if (myObject.shape.rows[y].row[x]) {
					//print (x + " - " + y);
					Vector3 displacement = new Vector3(x - PlaceableObjectData.center.x, y - PlaceableObjectData.center.y, 0);

					mySprites[x,y] = Instantiate(ItemSpritePrefab, transform.position + displacement, transform.rotation);
					mySprites[x,y].transform.parent = transform;
					ItemSprite mySprite = mySprites[x,y].GetComponent<ItemSprite>();
					mySprite.x = (int)displacement.x;
					mySprite.y = (int)displacement.y;
				}
			}
		}

		transform.rotation = Quaternion.Euler(0, 0, 180);
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

				int checkX = x - mySprite.x;
				int checkY = y - mySprite.y;

				//print (checkX + " - " + checkY);

				bool myVal = false;
				if (!Grid.s.myTiles[checkX, checkY].areThereItem)
					myVal = Grid.s.myTiles[checkX, checkY].itemPlaceable;




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
		if (!isPlaceable) {
			Destroy(gameObject);
		} else {
			GameObject InstantiatedItem = (GameObject)Instantiate(placedItemPrefab, transform.position, Quaternion.identity);
			PlacedItemBaseScript InstItemScript = InstantiatedItem.GetComponent<PlacedItemBaseScript>();

			foreach (GameObject thisSprite in mySprites) {
				if (thisSprite != null) {
					ItemSprite mySprite = thisSprite.GetComponentInChildren<ItemSprite>();

					if (mySprite != null) {
						int checkX = x - mySprite.x;
						int checkY = y - mySprite.y;

						try {
							TileBaseScript myTile = Grid.s.myTiles[checkX, checkY];
							//myTile.itemPlaceable = false;
							myTile.areThereItem = true;
							myTile.myItem = InstantiatedItem;
							InstItemScript.tilesCovered[InstItemScript.n_cover] = myTile;
							InstItemScript.n_cover++;
						} catch {
						}
					}
				}
			}

			InstItemScript.x = x;
			InstItemScript.y = y;
			//print ("destroy");
			Destroy(gameObject);
		}
	}
}
