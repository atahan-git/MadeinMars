using Boo.Lang.Environments;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Handles the player input processing for item placement.
/// </summary>
public class Player_BuildingController : MonoBehaviour {
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
			PlaceItemCheck();
		if (isBeltPlacing && !lastFrameStuff)
			PlaceBeltsCheck();
		if (isSelling)
			SellCheck();
		lastFrameStuff = false;
	}


	public TileBaseScript lastTile;
	public BuildingData buildingItem;
	void PlaceItemCheck () {
		if (Input.GetMouseButton(0)) {
			Ray myRay = GetRay();
			RaycastHit hit = new RaycastHit();
			if (Physics.Raycast(myRay, out hit)) {
				TileBaseScript tileS;

				tileS = hit.collider.gameObject.GetComponent<TileBaseScript>();

				if (tileS == null)
					return;

				lastTile = tileS;
				curItemPlacementScript.UpdatePosition(tileS);
			}
		} else {
			print("Item Placement Done");
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
		Deselect();
		print("Placing Item");
		isPlacingItem = true;
		isBeltPlacing = false;
		isSelling = false;
		Player_MasterControlCheck.s.ToggleMovement(true);
		Player_MasterControlCheck.s.TogglePlacingItem(true);
		//UIBeltModeOverlay.SetActive (false);
		buildingItem = myData;
		GameObject curItemPlacement = Instantiate(ItemPlacementHelperPrefab, transform.position, Quaternion.identity);
		curItemPlacementScript = curItemPlacement.GetComponent<ItemPlacementHelper>();
		curItemPlacementScript.Setup(myData);
	}

	public void Deselect () {
		isPlacingItem = false;
		isBeltPlacing = false;
		isSelling = false;
		// UIBeltModeOverlay.SetActive (false);
		Player_MasterControlCheck.s.ToggleMovement(true);
		Player_MasterControlCheck.s.TogglePlacingItem(false);
	}


	public void ActivateSellMode () {
		Deselect();
		isPlacingItem = false;
		isBeltPlacing = false;
		isSelling = true;
		Player_MasterControlCheck.s.ToggleMovement(true);
		Player_MasterControlCheck.s.TogglePlacingItem(true);
		//UIBeltModeOverlay.SetActive (false);
	}

	void SellCheck () {
		if (Input.GetMouseButton(0)) {
			Ray myRay = GetRay();
			RaycastHit hit = new RaycastHit();
			if (Physics.Raycast(myRay, out hit)) {

				Debug.Log("Selling " + hit.collider.gameObject.name);

				TileBaseScript tileS;
				try {
					tileS = hit.collider.gameObject.GetComponent<TileBaseScript>();
				} catch {
					return;
				}

				if (!tileS.isEmpty) {
					if (tileS.myBuilding != null)
						tileS.myBuilding.GetComponent<BuildingWorldObject> ().DestroyYourself ();
					else
						tileS.myBelt.GetComponent<BeltObject> ().DestroyYourself ();
				}
			}
		}
	}

	//--------------------------------------------------------------------------------------------------BELT STUFF
	bool lastFrameStuff = false;
	public void ActivateBeltMode () {
		Deselect();
		Player_MasterControlCheck.s.ToggleMovement(true);
		Player_MasterControlCheck.s.TogglePlacingItem(true);
		isBeltPlacing = true;
		isSelling = false;
		//UIBeltModeOverlay.SetActive (true); // handled by GUI building bar controller
		lastFrameStuff = true;
	}

	BuildingWorldObject b_lastBuilding;
	BeltObject b_lastBelt;
	TileBaseScript b_lastTile;

	int n = 0;

	void PlaceBeltsCheck () {
		if (Input.GetMouseButton(0)) {
			Ray myRay = GetRay();
			if (Physics.Raycast(myRay, out RaycastHit hit)) {                                      // cast the ray
				TileBaseScript tileS;
				try {
					tileS = hit.collider.gameObject.GetComponent<TileBaseScript>(); //hit something
				} catch {
					return;
				}

				if (b_lastTile == tileS) {                                              //is it still the same tile?
																						//print ("do nothing");
					return;
				}
				if (b_lastTile != null) {                                               //how much did we moved - if too much do shit
					if (Mathf.Abs(b_lastTile.x - tileS.x) >= 2 || Mathf.Abs(b_lastTile.y - tileS.y) >= 2 || Mathf.Abs(b_lastTile.y - tileS.y) + Mathf.Abs(b_lastTile.x - tileS.x) >= 2) {
						print("we moved 2 blocks");

						if (b_lastBelt != null)
							b_lastBelt.UpdateGraphics();

						b_lastBelt = null;
						b_lastBuilding = null;
						b_lastTile = null;
					}
				}

				if (tileS.beltPlaceable) {                  //can we place a belt here
					if (tileS.isEmpty) {                                          //there are no items here so place a belt
						BeltObject myBelt = ObjectBuilderMaster.BuildBelt(tileS);

						if (b_lastBelt != null) {                                       //there was a belt before this one - update its out stuff
							BeltObject.ConnectBeltsBuildingOnly(b_lastBelt, myBelt);
							BeltMaster.s.AddOneBeltConnectedToOne(myBelt, b_lastBelt);
						} else if (b_lastTile != null) {                                     //this is not the starting point - update in location
							BeltObject.ConnectBeltsBuildingOnly(b_lastTile, myBelt);
							BeltMaster.s.AddOneBelt(myBelt);
						} else {
							BeltMaster.s.AddOneBelt(myBelt);
						}

						/*if (b_lastItem != null) {										//there was an item before us - update its out stuff
							b_lastItem.outConveyors [b_lastItem.n_out] = myBelt;
							b_lastItem.n_out++;
							myBelt.inputItems [RevertLocation (movementToArrayNum (b_lastTile, tileS))] = b_lastItem;
						}*/ // This is magically handled by the belt master now

						if (b_lastBelt != null)
							b_lastBelt.UpdateGraphics();
						if (myBelt != null)
							myBelt.UpdateGraphics();

						b_lastBelt = myBelt;
						b_lastBuilding = null;
						b_lastTile = tileS;

					} else {                                                            //there is an item below us
						BuildingWorldObject myBuilding = null;
						BeltObject myBelt = null;
						if (tileS.myBuilding != null)
							// We never connect to buildings anymore so just make it null.
							myBuilding = null;//tileS.myBuilding.GetComponent<BuildingWorldObject>();
						if (tileS.myBelt != null)
							myBelt = tileS.myBelt.GetComponent<BeltObject>();

						if (b_lastBelt == null && b_lastBuilding == null) {                             //nothing to something
																										//do nothing

						} else if (b_lastBelt == null && b_lastBuilding != null && myBuilding != null) {    //item to item
																											//do nothing

						} else if (b_lastBelt == null && b_lastBuilding != null && myBelt != null) {    //item to belt
							BeltObject.ConnectBeltsBuildingOnly(b_lastTile, myBelt);
							BeltMaster.s.AddOneBelt(myBelt);

						} else if (b_lastBelt != null && b_lastBuilding == null && myBelt != null) {    //belt to belt
							BeltObject.ConnectBeltsBuildingOnly(b_lastBelt, myBelt);
							BeltMaster.s.AddOneBeltConnectedToOne(myBelt, b_lastBelt);

						} else if (b_lastBelt != null && b_lastBuilding == null && myBuilding != null) {    //belt to item
							BeltObject.ConnectBeltsBuildingOnly(b_lastBelt, tileS);
							BeltMaster.s.AddOneBelt(myBelt);

						} else {
							Debug.LogError("Connection failure: " + b_lastTile + " - " + tileS + " - " + b_lastBelt + " - " + b_lastBuilding + " - " + myBelt + " - " + myBuilding);
						}

						if (b_lastBelt != null)
							b_lastBelt.UpdateGraphics();
						if (myBelt != null)
							myBelt.UpdateGraphics();

						b_lastBelt = myBelt;
						b_lastBuilding = myBuilding;
						b_lastTile = tileS;
					}
				}
			}
		} else {
			if (b_lastBelt != null)
				b_lastBelt.UpdateGraphics();

			b_lastBelt = null;
			b_lastBuilding = null;
			b_lastTile = null;
		}
	}

	Ray GetRay () {
		Ray myRay;
		if (Input.touchCount > 0)
			myRay = mycam.ScreenPointToRay(Input.GetTouch(0).position);
		else
			myRay = mycam.ScreenPointToRay(Input.mousePosition);
		return myRay;
	}
}