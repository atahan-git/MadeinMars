﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Controls the Building Bar in the UI, the three slots that you can build buildings out from, and also the connections for the belt and sell buttons.
/// </summary>
public class GUI_BuildingBarController : MonoBehaviour {
    public static GUI_BuildingBarController s;
    
    public bool isOnFocus = true;
    GUI_SwitchController scont;

    public GameObject beltBuildingOverlay;
    public GameObject connectorBuildingOverlay;
    public GameObject sellModeOverlay;
    public GameObject buildingDragFromInventoryOverlay;

    public Transform BuildingBarSlotsParent;
    MiniGUI_BuildingBarSlot[] myBuildingBarSlots;

    public Image PlaceBeltsButton;
    public bool CanPlaceBelts = true;

    public Toggle beltSafeModeToggle;

    private void Awake () {
        if (s != null) {
            Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
        }
        s = this;

        myBuildingBarSlots = BuildingBarSlotsParent.GetComponentsInChildren<MiniGUI_BuildingBarSlot>();
        
        scont = GetComponent<GUI_SwitchController>();
        GameMaster.CallWhenLoaded(LoadBuildingSlots);
    }

    private void Start () {
        DataSaver.saveEvent += SaveBuildingSlots;

        //Player_InventoryController.s.inventoryContentsChangedEvent += UpdateSlotsBuildableStates;
    }

    void LoadBuildingSlots (bool isSuccess) {
        if (isSuccess) {
            var buildingBarData = DataSaver.s.GetSave().buildingBarData;
            if (buildingBarData != null) {
                for (int i = 0; i < buildingBarData.Length; i++) {
                    if (buildingBarData[i] != null)
                        if (buildingBarData[i].Length > 0)
                            myBuildingBarSlots[i].ChangeBuilding(
                                DataHolder.s.GetBuilding(buildingBarData[i]),
                                false,null, false ,0);

                }
            }

            UpdateSlotsBuildableStates();
        }
    }

    private void Update() {
        if (inventoryDragBegun)
#if UNITY_EDITOR
        if (Input.GetMouseButtonUp(0))
            StopDragInventoryBuilding();
#else
        if (Input.GetMouseButtonUp(0) || (Input.touchCount == 0 && Input.touchSupported))
            StopDragInventoryBuilding();
#endif
    }

    
    // IMPORTANT CHANGE
    //
    // Previously we needed items from the player inventoryItemSlots to build things, not anymore!
    // Now drones collect resources and build the things.
    public Color defColor = Color.white;
    public void UpdateSlotsBuildableStates () {
        var availableBuildings = ShipDataMaster.s.playerBuildableBuildingsSetByShipCards;
        for (int i = 0; i < myBuildingBarSlots.Length; i++) {
            if (myBuildingBarSlots[i].myDat != null) {
                //myBuildingBarSlots[i].UpdateBuildableState(Player_MasterControlCheck.s.inventoryController.CanPlaceBuilding(myBuildingBarSlots[i].myDat));
                var canBuildBuilding = false;
                for (int j = 0; j < availableBuildings.Count; j++) {
                    if (availableBuildings[i].uniqueName == myBuildingBarSlots[i].myDat.uniqueName) {
                        canBuildBuilding = true;
                        break;
                    }
                }
                myBuildingBarSlots[i].UpdateBuildableState(canBuildBuilding);
            }
        }

        //CanPlaceBelts = Player_MasterControlCheck.s.inventoryController.CanPlaceBuilding(FactoryBuilder.s.beltBuildingData);
        CanPlaceBelts = true;
        if (CanPlaceBelts) {
            PlaceBeltsButton.color = defColor;
        } else {
            float darkness = 0.5f;
            PlaceBeltsButton.color = new Color(darkness, darkness, darkness);
            if (beltBuildingOverlay.activeSelf)
                Deselect();
        }
    }

    public bool inventoryDragBegun = false;
    public BuildingData dragBuildDat = null;
    public void BeginDragInventoryBuilding (BuildingData building) {
        Debug.Log("Begin Drag inventoryItemSlots Building");
        buildingDragFromInventoryOverlay.SetActive(true);
        dragBuildDat = building;
        inventoryDragBegun = true;
    }

    public void StopDragInventoryBuilding () {
        Debug.Log("End Drag inventoryItemSlots Building");
        buildingDragFromInventoryOverlay.SetActive(false);
        inventoryDragBegun = false;
        //UpdateSlotsBuildableStates();
    }

    void OnGUI () {
        if (inventoryDragBegun) {
            float aspectRatio = dragBuildDat.gfxSprite.rect.width / dragBuildDat.gfxSprite.rect.height;
            Rect target = new Rect(SmartInput.inputPos.x, Screen.height - SmartInput.inputPos.y, 200f* aspectRatio, 200f);
            GUIDrawSprite(target, dragBuildDat.gfxSprite);
        }
    }

    BuildingData barSlotOldBuilding;
    public void PointerEnterBuildingBarSlot (int slot) {
        if (inventoryDragBegun) {
            if (slot != -1) {
                barSlotOldBuilding = myBuildingBarSlots[slot].myDat;
                myBuildingBarSlots[slot].ChangeBuilding(dragBuildDat, false, null, false , 0);
            }
        }
    }

    public void PointerExitBuildingBarSlot (int slot) {
        if (inventoryDragBegun) {
            RectTransform myRect = myBuildingBarSlots[slot].GetComponent<RectTransform>();
            Vector2 localMousePosition = myRect.InverseTransformPoint(SmartInput.inputPos);
            if (!myRect.rect.Contains(localMousePosition)) {
                myBuildingBarSlots[slot].ChangeBuilding(barSlotOldBuilding, false,null, false, 0);
                barSlotOldBuilding = null;
            }
        }
    }


    public void StartBuildingFromSlot (BuildingData dat, bool isRocket, List<InventoryItemSlot> inv, SuccessFailCallback buildCompleteCallback) {
        beltBuildingOverlay.SetActive(false);
        sellModeOverlay.SetActive(false);
        Player_MasterControlCheck.s.buildingController.TryToPlaceItem(dat, isRocket,  inv, buildCompleteCallback);
        
        scont.BringBuildingBarToFocus();
    }
    
    

    public void SellMode () {
        Deselect();
        sellModeOverlay.SetActive(true);
        Player_MasterControlCheck.s.buildingController.ActivateSellMode();
        scont.BringBuildingBarToFocus();
    }

    public void BeltMode () {
        Deselect();
        if (!CanPlaceBelts) {
            return;
        }
        beltBuildingOverlay.SetActive(true);
        Player_MasterControlCheck.s.buildingController.ActivateBeltMode();
        scont.BringBuildingBarToFocus();
    }
    
    public void ConnectorMode () {
        Deselect();
        if (!CanPlaceBelts) {
            return;
        }
        connectorBuildingOverlay.SetActive(true);
        Player_MasterControlCheck.s.buildingController.ActivateConnectorMode();
        scont.BringBuildingBarToFocus();
    }

    public void ToggleBeltSafeMode() {
        Player_MasterControlCheck.s.buildingController.ToggleBeltSafeMode(beltSafeModeToggle.isOn);
    }

    public void Deselect () {
        beltBuildingOverlay.SetActive(false);
        connectorBuildingOverlay.SetActive(false);
        sellModeOverlay.SetActive(false);
        Player_MasterControlCheck.s.buildingController.Deselect();
    }

    void SaveBuildingSlots () {
        var currentSave = DataSaver.s.GetSave();
        currentSave.buildingBarData = new string[myBuildingBarSlots.Length];
        for (int i = 0; i < myBuildingBarSlots.Length; i++) {
            if (myBuildingBarSlots[i].myDat != null) {
                currentSave.buildingBarData[i] = myBuildingBarSlots[i].myDat.uniqueName;
            }
        }
    }

	public void OnDestroy () {
        GameMaster.RemoveFromCall(LoadBuildingSlots);
        DataSaver.saveEvent -= SaveBuildingSlots;
        //Player_InventoryController.s.inventoryContentsChangedEvent -= UpdateSlotsBuildableStates;
    }


	static void GUIDrawSprite (Rect rect, Sprite sprite) {
        Rect spriteRect = sprite.rect;
        Texture2D tex = sprite.texture;
        GUI.DrawTextureWithTexCoords(rect, tex, new Rect(spriteRect.x / tex.width, spriteRect.y / tex.height, spriteRect.width / tex.width, spriteRect.height / tex.height));
    }
}
