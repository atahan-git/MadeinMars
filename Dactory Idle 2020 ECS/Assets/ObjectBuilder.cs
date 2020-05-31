using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBuilder : MonoBehaviour
{
	public static bool CheckPlaceable (Position location) {
		return Grid.s.myTiles[location.x, location.y].itemPlaceable && !Grid.s.myTiles[location.x, location.y].areThereItem;
	}

	public static bool CheckPlaceable (BuildingData myData, Position location) {
		/*for (int y = 0; y < myData.shape.rows.Length; y++) {
			for (int x = 0; x < myData.shape.rows[y].row.Length; x++) {
				if (myData.shape.rows[y].row[x]) {
			
			}
		}*/
		return false;
	}


	public static bool BuildObject (BuildingData myData, Vector2 location) {
		/*} else {
			GameObject InstantiatedItem = (GameObject)Instantiate(placedItemPrefab, transform.position, Quaternion.identity);
			PlacedItemBaseScript InstItemScript = InstantiatedItem.GetComponent<PlacedItemBaseScript>();

			foreach (GameObject thisSprite in mySprites) {
				if (thisSprite != null) {
					ItemSprite mySprite = thisSprite.GetComponentInChildren<ItemSprite>();

					if (mySprite != null) {
						int checkX = x - mySprite.x;
						int checkY = y - mySprite.y;

						try {
							TileBaseScript myTile = Grid.s.myTilesGameObjects[checkX, checkY].GetComponent<TileBaseScript>();
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

	*/
		return true;
    }
}
