using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;


/// <summary>
/// Handles the player input processing for item placement.
/// </summary>
public class Player_BuildingController : MonoBehaviour
{
	public GameObject beltPrefab;
	public GameObject ItemPlacementHelperPrefab;
	ItemPlacementHelper curItemPlacementScript;

	public bool isPlacingItem = false;
	public bool isBeltPlacing = false;
	public bool isSelling = false;

	public int curItemId = 0;

	Camera mycam;
	public GameObject UIBeltModeOverlay;

	// Use this for initialization
	void Start () {
		mycam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		if (isPlacingItem)
			PlaceItemCheck ();
		/*if (isBeltPlacing && !lastFrameStuff)
			PlaceBeltsCheck ();
		if (isSelling)
			SellCheck ();
		lastFrameStuff = false;*/
	}


	public TileBaseScript lastTile;
	public BuildingData buildingItem;
	void PlaceItemCheck () {
		if (Input.GetMouseButton(0)) {
			Ray myRay = mycam.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit = new RaycastHit ();
			if (Physics.Raycast (myRay, out hit)) {
				TileBaseScript tileS;
				
				tileS = hit.collider.gameObject.GetComponent<TileBaseScript> ();

				if (tileS == null)
					return;

				lastTile = tileS;
				curItemPlacementScript.UpdatePosition(tileS);
			}
		} else {
			print ("Item Placement Done");
			isPlacingItem = false;
			Player_MasterControlCheck.s.TogglePlacingItem(false);
			if (ObjectBuilderMaster.CheckPlaceable(buildingItem, lastTile.position)) {
				curItemPlacementScript.PlaceSelf();
				ObjectBuilderMaster.BuildObject(buildingItem, lastTile.position);
			} else {
				curItemPlacementScript.FailedPlacingSelf();
			}
		}
	}
	
	public void TryToPlaceItem (BuildingData myData) {
		print ("Placing Item");
		isPlacingItem = true;
		isBeltPlacing = false;
		isSelling = false;
		Player_MasterControlCheck.s.ToggleMovement(true);
		Player_MasterControlCheck.s.TogglePlacingItem(true);
		UIBeltModeOverlay.SetActive (false);
		buildingItem = myData;
		GameObject curItemPlacement = Instantiate (ItemPlacementHelperPrefab, transform.position, Quaternion.identity);
		curItemPlacementScript = curItemPlacement.GetComponent<ItemPlacementHelper> ();
		curItemPlacementScript.Setup(myData);
	}

	public void SelectDown () {
		isPlacingItem = false;
		isBeltPlacing = false;
		isSelling = false;
		UIBeltModeOverlay.SetActive (false);
		Player_MasterControlCheck.s.TogglePlacingItem(false);
	}

	public void SelectUp() {
		Player_MasterControlCheck.s.ToggleMovement(true);
		Player_MasterControlCheck.s.TogglePlacingItem(false);
	}

	public void SellEnable (){
		isPlacingItem = false;
		isBeltPlacing = false;
		isSelling = true;
		UIBeltModeOverlay.SetActive (false);
	}

	void SellCheck (){
		if (Input.GetMouseButton(0)) {
			Ray myRay = mycam.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit = new RaycastHit ();
			if (Physics.Raycast (myRay, out hit)) {

				print (hit.collider.gameObject.name);

				TileBaseScript tileS;
				try {
					tileS = hit.collider.gameObject.GetComponent<TileBaseScript> ();
				} catch {
					return;
				}

				if (tileS.areThereItem) {
					/*if (tileS.myItem.GetComponent<PlacedItemBaseScript> ())
						tileS.myItem.GetComponent<PlacedItemBaseScript> ().DestroyYourself ();
					else
						tileS.myItem.GetComponent<BeltScript> ().DestroyYourself ();*/
					throw new NotImplementedException("Building selling not implemented yet!");
				}
			}
		}
	}

	//--------------------------------------------------------------------------------------------------BELT STUFF
	/*bool lastFrameStuff = false;
	public void ActivateBeltMode () {
		isMovementEnabled = false;
		isBeltPlacing = true;
		isSelling = false;
		UIBeltModeOverlay.SetActive (true);
		lastFrameStuff = true;
	}

	PlacedItemBaseScript b_lastItem;
	BeltScript b_lastBelt;
	TileBaseScript b_lastTile;

	int n = 0;

	void PlaceBeltsCheck () {
		if (Input.GetMouseButton(0)) {
			Ray myRay = mycam.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit = new RaycastHit ();
			if (Physics.Raycast (myRay, out hit)) {										// cast the ray
				TileBaseScript tileS;
				try {
					tileS = hit.collider.gameObject.GetComponent<TileBaseScript> ();	//hit something
				} catch {
					return;
				}

				if (b_lastTile == tileS) {												//is it still the same tile?
					//print ("do nothing");
					return;
				}
				if (b_lastTile != null) {												//how much did we moved - if too much do shit
					if (Mathf.Abs (b_lastTile.x - tileS.x) >= 2 || Mathf.Abs (b_lastTile.y - tileS.y) >= 2 || Mathf.Abs (b_lastTile.y - tileS.y) + Mathf.Abs (b_lastTile.x - tileS.x) >= 2) {
						print ("we moved 2 blocks");

						if (b_lastBelt != null)
							b_lastBelt.UpdateGraphic ();

						b_lastBelt = null;
						b_lastItem = null;
						b_lastTile = null;
					}
				}

				if (tileS.beltPlaceable) {					//can we place a belt here
					if (!tileS.areThereItem) {											//there are no items here so place a belt
						BeltScript myBelt = ((GameObject)Instantiate (beltPrefab, tileS.transform.position, Quaternion.identity)).GetComponent<BeltScript> ();
						myBelt.gameObject.name = myBelt.gameObject.name + " - " + n;
						myBelt.x = tileS.x;
						myBelt.y = tileS.y;
						n++;
						tileS.areThereItem = true;
						tileS.myItem = myBelt.gameObject;
						myBelt.tileCovered = tileS;

						if (b_lastTile != null) {										//this is not the starting point - update in location
							UpdateBeltInOut (b_lastTile, tileS, myBelt, true);
						}

						if (b_lastBelt != null) {										//there was a belt before this one - update its out stuff
							UpdateBeltInOut (b_lastTile, tileS, b_lastBelt, false);

							b_lastBelt.feedingBelts [movementToArrayNum(b_lastTile,tileS)] = myBelt;
							myBelt.inputBelts [RevertLocation (movementToArrayNum (b_lastTile, tileS))] = b_lastBelt;
						}

						if (b_lastItem != null) {										//there was an item before us - update its out stuff
							b_lastItem.outConveyors [b_lastItem.n_out] = myBelt;
							b_lastItem.n_out++;
							myBelt.inputItems [RevertLocation (movementToArrayNum (b_lastTile, tileS))] = b_lastItem;
						}

						if (b_lastBelt != null)
							b_lastBelt.UpdateGraphic ();
						if (myBelt != null)
							myBelt.UpdateGraphic ();

						b_lastBelt = myBelt;
						b_lastItem = null;
						b_lastTile = tileS;

					} else {															//there is an item below us
						PlacedItemBaseScript myItem = null;
						BeltScript myBelt = null;
						myItem = tileS.myItem.GetComponent<PlacedItemBaseScript> ();
						myBelt = tileS.myItem.GetComponent<BeltScript> ();

						if (b_lastBelt == null && b_lastItem == null) {								//nothing to something
							//do nothing

						} else if (b_lastBelt == null && b_lastItem != null && myItem != null) {	//item to item
							//do nothing

						} else if (b_lastBelt == null && b_lastItem != null && myBelt != null) {	//item to belt
							b_lastItem.outConveyors [b_lastItem.n_out] = myBelt;
							b_lastItem.n_out++;
							b_lastBelt.inputItems [RevertLocation (movementToArrayNum (b_lastTile, tileS))] = b_lastItem;
							UpdateBeltInOut (b_lastTile, tileS, myBelt, true);

						} else if (b_lastBelt != null && b_lastItem == null && myBelt != null) {	//belt to belt
							UpdateBeltInOut (b_lastTile, tileS, b_lastBelt, false);
							b_lastBelt.feedingBelts [movementToArrayNum(b_lastTile,tileS)] = myBelt;
							myBelt.inputBelts [RevertLocation (movementToArrayNum (b_lastTile, tileS))] = b_lastBelt;

							UpdateBeltInOut (b_lastTile, tileS, myBelt, true);

						} else if (b_lastBelt != null && b_lastItem == null && myItem != null) {	//belt to item
							UpdateBeltInOut (b_lastTile, tileS, b_lastBelt, false);
							b_lastBelt.feedingItems [movementToArrayNum(b_lastTile,tileS)] = myItem;

							myItem.inConveyors [myItem.n_in] = b_lastBelt;
							myItem.n_in++;

						} else {
							Debug.LogError ("weird shit happened: " + b_lastTile + " - " + tileS + " - " + b_lastBelt + " - " + b_lastItem + " - " + myBelt + " - " + myItem);
						}

						if (b_lastBelt != null)
							b_lastBelt.UpdateGraphic ();
						if (myBelt != null)
							myBelt.UpdateGraphic ();

						b_lastBelt = myBelt;
						b_lastItem = myItem;
						b_lastTile = tileS;
					}
				} 
			}
		} else {
			if (b_lastBelt != null)
				b_lastBelt.UpdateGraphic ();

			b_lastBelt = null;
			b_lastItem = null;
			b_lastTile = null;
		}
	}

	void UpdateBeltInOut(TileBaseScript lastTile, TileBaseScript thisTile, BeltScript myBelt, bool isIn){
		int x = lastTile.x;
		int xOther = thisTile.x;
		int y = lastTile.y;
		int yOther = thisTile.y;

		if (isIn) {
			if (x > xOther)      //left
				myBelt.inLocations [2] = true;
			else if (x < xOther) //right
				myBelt.inLocations [0] = true;
			else if (y > yOther) //down
				myBelt.inLocations [1] = true;
			else if (y < yOther) //up
				myBelt.inLocations [3] = true;
		} else {
			if (x > xOther)      //left
				myBelt.outLocations [0] = true;
			else if (x < xOther) //right
				myBelt.outLocations [2] = true;
			else if (y > yOther) //down
				myBelt.outLocations [3] = true;
			else if (y < yOther) //up
				myBelt.outLocations [1] = true;
		}

		//myBelt.UpdateGraphic ();
	}*/

	int movementToArrayNum (TileBaseScript lastTile, TileBaseScript thisTile) {
		int x = lastTile.x;
		int xOther = thisTile.x;
		int y = lastTile.y;
		int yOther = thisTile.y;

		if (x > xOther)      //left
			return 0;
		else if (x < xOther) //right
			return 2;
		else if (y > yOther) //down
			return 3;
		else if (y < yOther) //up
			return 1;
		else {
			Debug.LogError("erorrr");
			return -1;
		}
	}

	int RevertLocation (int location) {
		//return location;

		switch (location) {
		case 0:
			return 2;
			break;
		case 1:
			return 3;
			break;
		case 2:
			return 0;
			break;
		case 3:
			return 1;
			break;
		default:
			Debug.LogError("given wrong number: " + location);
			return -1;
			break;
		}
	}
}

