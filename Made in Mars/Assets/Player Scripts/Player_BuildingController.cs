using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player_BuildingController : MonoBehaviour {
	public GameObject ItemPlacementHelperPrefab;
	ItemPlacementHelper curItemPlacementScript;

	public bool isRocket = false;
	private List<InventoryItemSlot> inventory = null;
	SuccessFailCallback buildCompleteCallback;

	public RectTransform buildingBarArea;


	public bool instantBuildCheat = false;
	public enum  PlacementState {
		nothing, item, belt, connector, sell
	}

	public PlacementState curState = PlacementState.nothing;
	public bool isBeltSafeMode = true;

	private void Awake() {
		GameMaster.CallWhenClearPlanet(OnClearPlanet);
	}

	private void OnDestroy() {
		GameMaster.RemoveFromCall(OnClearPlanet);
	}

	private void OnClearPlanet() {
		isRocket = false;
		inventory = null;
		buildCompleteCallback = null;
		curState = PlacementState.nothing;
		isBeltSafeMode = true;
	}

	private void Start() {
		if (instantBuildCheat)
			Debug.LogError("Warning: Instant Cheat Building mode is enabled!");
	}

	// Update is called once per frame
	void Update () {
		if (curState == PlacementState.item)
			PlaceItemCheck();
		if (curState == PlacementState.belt && !lastFrameStuff)
			PlaceBeltsCheck();
		if (curState == PlacementState.connector && !lastFrameStuff)
			PlaceConnectorsCheck();
		if (curState == PlacementState.sell)
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
			curState = PlacementState.nothing;
			Player_MasterControlCheck.s.TogglePlacingItem(false);
			if (curItemPlacementScript != null) {
				if (IsPointerOutsideBuildingBar() && FactoryPlayerConnector.s.CheckPlaceable(buildingItem, lastTile.location)) {
					curItemPlacementScript.PlaceSelf();
					FactoryPlayerConnector.s.BuildObject(buildingItem, lastTile.location,false, isRocket, instantBuildCheat, inventory);
					buildCompleteCallback?.Invoke(true);
					buildCompleteCallback = null;
				} else {
					Debug.Log("Item placement failed due: pointer place =" + IsPointerOutsideBuildingBar() + " or checkplaceable = " + FactoryPlayerConnector.s.CheckPlaceable(buildingItem, lastTile.location));
					curItemPlacementScript.FailedPlacingSelf();
					buildCompleteCallback?.Invoke(false);
					buildCompleteCallback = null;
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
	public void TryToPlaceItem (BuildingData myData, bool _isRocket, List<InventoryItemSlot> _inventory, SuccessFailCallback _buildCompleteCallback) {
		Deselect();
		print("Placing Item");
		buildCompleteCallback = _buildCompleteCallback;
		
		inventory = _inventory;
		
		isRocket = _isRocket;
		curState = PlacementState.item;
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
		curState = PlacementState.nothing;
		// UIBeltModeOverlay.SetActive (false);
		Player_MasterControlCheck.s.ToggleMovement(true);
		Player_MasterControlCheck.s.TogglePlacingItem(false);
	}

	public void ActivateSellMode () {
		Deselect();
		curState = PlacementState.sell;
		Player_MasterControlCheck.s.ToggleMovement(true);
		Player_MasterControlCheck.s.TogglePlacingItem(true);
		//UIBeltModeOverlay.SetActive (false);
	}

	void SellCheck() {
		if (Input.GetMouseButton(0) && IsPointerOutsideBuildingBar()) {
			TileData myTile = Grid.s.GetTileUnderPointer();
			if (myTile != null) {

				Debug.Log("Selling " + myTile.name);

				if (myTile.areThereSimObject) {
					if (!instantBuildCheat) {
						FactoryBuilder.StartDeconstruction(myTile.location);
					} else {
						FactoryBuilder.StartDeconstruction(myTile.location);
						FactoryBuilder.CompleteDeconstruction(myTile.location);
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
		curState = PlacementState.belt;
		//UIBeltModeOverlay.SetActive (true); // handled by GUI building bar controller
		lastFrameStuff = true;
	}
	
	public void ActivateConnectorMode () {
		Deselect(); 
		Player_MasterControlCheck.s.ToggleMovement(true);
		Player_MasterControlCheck.s.TogglePlacingItem(true);
		curState = PlacementState.connector;
		//UIBeltModeOverlay.SetActive (true); // handled by GUI building bar controller
		lastFrameStuff = true;
	}

	TileData b_lastTile;

	int n = 0;

	void PlaceBeltsCheck() {
		if (Input.GetMouseButton(0) && IsPointerOutsideBuildingBar()) {
			TileData myTile = Grid.s.GetTileUnderPointer();
			if (myTile != null) {
				//Get tile

				if (b_lastTile == myTile) {
					//is it still the same tile?
					//print ("do nothing");
					return;
				} else if (b_lastTile != null) {
					int direction = Position.CardinalDirection(b_lastTile.location, myTile.location);
					FactoryPlayerConnector.s.BuildBelt(myTile.location, direction, instantBuildCheat);
				}

				b_lastTile = myTile;
			}

		} else {
			b_lastTile = null;
		}
	}

	void PlaceConnectorsCheck() {
		if (Input.GetMouseButton(0) && IsPointerOutsideBuildingBar()) {
			TileData myTile = Grid.s.GetTileUnderPointer();
			if (myTile != null) {
				//Get tile

				if (b_lastTile == myTile) {
					//is it still the same tile?
					//print ("do nothing");
					return;
				} else if (b_lastTile != null) {
					int direction = Position.ParallelDirection(b_lastTile.location, myTile.location);
					FactoryPlayerConnector.s.BuildConnector(myTile.location, direction, instantBuildCheat);
				} else {
					int direction = Position.ParallelDirection(myTile.location, myTile.location);
					FactoryPlayerConnector.s.BuildConnector(myTile.location, direction, instantBuildCheat);
				}

				b_lastTile = myTile;
			}
		} else {
			b_lastTile = null;
		}
	}

	bool IsPointerOutsideBuildingBar() {
		return !RectTransformUtility.RectangleContainsScreenPoint(buildingBarArea, SmartInput.inputPos);
	}
}