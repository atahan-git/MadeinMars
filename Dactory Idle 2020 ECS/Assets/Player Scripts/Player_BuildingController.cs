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

	public bool isSpaceLanding = false;
	public bool isInventory = false;
	private List<InventoryItemSlot> inventory = null;
	public GenericCallback buildCompleteCallback;
	public bool isPlacingItem = false;
	public bool isBeltPlacing = false;
	public bool isSelling = false;
	public bool isBeltSafeMode = true;


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

	public void ToggleBeltSafeMode(bool state) {
		isBeltSafeMode = state;
	}


	public TileData lastTile;
	public BuildingData buildingItem;
	void PlaceItemCheck () {
		if (Input.GetMouseButton(0)) {
			TileData myTile = Grid.s.GetTileUnderPointer();
			if (myTile!=null) {
				lastTile = myTile;
				if(curItemPlacementScript != null)
					curItemPlacementScript.UpdatePosition(myTile);
			}
		} else {
			print("Item Placement Done");
			isPlacingItem = false;
			Player_MasterControlCheck.s.TogglePlacingItem(false);
			if (curItemPlacementScript != null) {
				if (SmartInput.inputPos.y > 200 && ObjectBuilderMaster.CheckPlaceable(buildingItem, lastTile.position)) {
					curItemPlacementScript.PlaceSelf();
					ObjectBuilderMaster.BuildObject(buildingItem, lastTile.position,false, isSpaceLanding, isInventory, inventory);
					Player_MasterControlCheck.s.inventoryController.UseBuildingResources(buildingItem, 1);
					buildCompleteCallback?.Invoke();
				} else {
					curItemPlacementScript.FailedPlacingSelf();
				}
			}
		}
	}

	/*public void TryToPlaceItem (BuildingData myData) {
		TryToPlaceItem(myData, false, false, null);
	}*/
	
	
	/// <summary>
	/// spawns a placement helper and lets the user try and place an item down in the world based on the parameters.
	/// </summary>
	/// <param name="myData"></param>
	/// <param name="_isSpaceLanding"></param>
	/// <param name="_isInventory"></param>
	/// <param name="_inventory"></param>
	public void TryToPlaceItem (BuildingData myData, bool _isSpaceLanding, bool _isInventory, List<InventoryItemSlot> _inventory, GenericCallback _buildCompleteCallback) {
		Deselect();
		print("Placing Item");
		buildCompleteCallback = _buildCompleteCallback;
		if (_isInventory) {
			isInventory = _isInventory;
			inventory = _inventory;
		}
		isSpaceLanding = _isSpaceLanding;
		isPlacingItem = true;
		isBeltPlacing = false;
		isSelling = false;
		Player_MasterControlCheck.s.ToggleMovement(true);
		Player_MasterControlCheck.s.TogglePlacingItem(true);
		//UIBeltModeOverlay.SetActive (false);
		buildingItem = myData;
		if (buildingItem != null) {
			GameObject curItemPlacement = Instantiate(ItemPlacementHelperPrefab, transform.position, Quaternion.identity);
			curItemPlacementScript = curItemPlacement.GetComponent<ItemPlacementHelper>();
			curItemPlacementScript.Setup(myData);
		}
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
			if (SmartInput.inputPos.y < 200)
				return;

			TileData myTile = Grid.s.GetTileUnderPointer();
			if (myTile != null) {

				Debug.Log("Selling " + myTile.name);

				if (!myTile.isEmpty) {
					if (myTile.myBuilding != null) {
						Player_MasterControlCheck.s.inventoryController.UseBuildingResources(myTile.myBuilding.GetComponent<BuildingWorldObject>().myData, -1);
						myTile.myBuilding.GetComponent<BuildingWorldObject>().DestroyYourself();
					} else {
						Player_MasterControlCheck.s.inventoryController.UseBuildingResources(myTile.myBelt.GetComponent<BeltObject>().myData, -1);
						myTile.myBelt.GetComponent<BeltObject>().DestroyYourself();
					}
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
	TileData b_lastTile;

	int n = 0;

	void PlaceBeltsCheck () {
		if (Input.GetMouseButton(0)) {
			TileData myTile = Grid.s.GetTileUnderPointer();
			if (myTile != null) {											//Get tile

				if (b_lastTile == myTile) {                                              //is it still the same tile?
					//print ("do nothing");
					return;
				}
				if (b_lastTile != null) {                                               //how much did we moved - if too much do shit
					if (Mathf.Abs(b_lastTile.x - myTile.x) >= 2 || Mathf.Abs(b_lastTile.y - myTile.y) >= 2 || Mathf.Abs(b_lastTile.y - myTile.y) + Mathf.Abs(b_lastTile.x - myTile.x) >= 2) {
						print("we moved 2 blocks");

						if (b_lastBelt != null)
							b_lastBelt.UpdateGraphics();

						b_lastBelt = null;
						b_lastBuilding = null;
						b_lastTile = null;
					}
				}

				if (!isBeltSafeMode || b_lastBelt != null) {
					if (myTile.beltPlaceable) {
						//can we place a belt here
						if (myTile.isEmpty) {
							//there are no items here so place a belt
							BeltObject myBelt = ObjectBuilderMaster.BuildBelt(myTile);
							Player_MasterControlCheck.s.inventoryController.UseBuildingResources(ObjectBuilderMaster.beltBuildingData, 1);

							if (b_lastBelt != null) {
								//there was a belt before this one - update its out stuff
								BeltObject.ConnectBeltsBuildingOnly(b_lastBelt, myBelt);
								BeltMaster.s.AddOneBeltConnectedToOne(myBelt, b_lastBelt);
							} else if (b_lastTile != null) {
								//this is not the starting point - update in location
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
							b_lastTile = myTile;

							if (!Player_MasterControlCheck.s.inventoryController.CanPlaceBuilding(ObjectBuilderMaster.beltBuildingData)) {
								Deselect();
							}

						} else {
							//there is an item below us
							BuildingWorldObject myBuilding = null;
							BeltObject myBelt = null;
							if (myTile.myBuilding != null)
								// We never connect to buildings anymore so just make it null.
								myBuilding = null; //tileS.myBuilding.GetComponent<BuildingWorldObject>();
							if (myTile.myBelt != null)
								myBelt = myTile.myBelt.GetComponent<BeltObject>();

							if (b_lastBelt == null && b_lastBuilding == null) {
								//nothing to something
								//do nothing

							} else if (b_lastBelt == null && b_lastBuilding != null && myBuilding != null) {
								//item to item
								//do nothing

							} else if (b_lastBelt == null && b_lastBuilding != null && myBelt != null) {
								//item to belt
								BeltObject.ConnectBeltsBuildingOnly(b_lastTile, myBelt);
								BeltMaster.s.AddOneBelt(myBelt);

							} else if (b_lastBelt != null && b_lastBuilding == null && myBelt != null) {
								//belt to belt
								BeltObject.ConnectBeltsBuildingOnly(b_lastBelt, myBelt);
								BeltMaster.s.AddOneBeltConnectedToOne(myBelt, b_lastBelt);

							} else if (b_lastBelt != null && b_lastBuilding == null && myBuilding != null) {
								//belt to item
								BeltObject.ConnectBeltsBuildingOnly(b_lastBelt, myTile);
								BeltMaster.s.AddOneBelt(myBelt);

							} else {
								Debug.LogError("Connection failure: " + b_lastTile + " - " + myTile + " - " + b_lastBelt + " - " + b_lastBuilding + " - " + myBelt + " - " + myBuilding);
							}

							if (b_lastBelt != null)
								b_lastBelt.UpdateGraphics();
							if (myBelt != null)
								myBelt.UpdateGraphics();

							b_lastBelt = myBelt;
							b_lastBuilding = myBuilding;
							b_lastTile = myTile;
						}
					}
				} else {
					if (isBeltSafeMode) {
						if (myTile.myBelt != null) {
							b_lastBelt = myTile.myBelt.GetComponent<BeltObject>();
							b_lastTile = myTile;
						}
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
}