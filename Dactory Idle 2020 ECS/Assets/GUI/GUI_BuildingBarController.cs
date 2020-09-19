using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GUI_BuildingBarController : MonoBehaviour {

    public bool isOnFocus = true;
    GUI_SwitchController scont;

    public GameObject beltBuildingOverlay;
    public GameObject sellModeOverlay;
    public GameObject buildingDragFromInventoryOverlay;

    // public int buildingSlotCount = 3; >> possibly auto generate building bar slots?

    public MiniGUI_BuildingBarSlot[] myBuildingBarSlots;

    public Image PlaceBeltsButton;
    public bool CanPlaceBelts = true;

    private void Start () {
        GameLoader.CallWhenLoaded(LoadBuildingSlots);

        DataSaver.saveEvent += SaveBuildingSlots;

        Player_InventoryController.inventoryContentsChangedEvent += UpdateSlotsBuildableStates;

        scont = GetComponent<GUI_SwitchController>();
    }

    void LoadBuildingSlots () {
        if (DataSaver.mySave != null) {
            if (DataSaver.mySave.buildingBarData != null) {
                for (int i = 0; i < DataSaver.mySave.buildingBarData.Length; i++) {
                    if (DataSaver.mySave.buildingBarData[i] != null)
                        if (DataSaver.mySave.buildingBarData[i].Length > 0)
                            myBuildingBarSlots[i].ChangeBuilding(DataHolder.s.GetBuilding(DataSaver.mySave.buildingBarData[i]));

                }
            }
        }
    }

    private void Update () {
        if (inventoryDragBegun)
            if (Input.GetMouseButtonUp(0) || (Input.touchCount == 0 && Input.touchSupported))
                StopDragInventoryBuilding();
    }

    public Color defColor = Color.white;
    public void UpdateSlotsBuildableStates () {
        for (int i = 0; i < myBuildingBarSlots.Length; i++) {
            if(myBuildingBarSlots[i].myDat != null)
            myBuildingBarSlots[i].UpdateBuildableState(Player_MasterControlCheck.s.inventoryController.CanPlaceBuilding(myBuildingBarSlots[i].myDat));
        }

        CanPlaceBelts = Player_MasterControlCheck.s.inventoryController.CanPlaceBuilding(ObjectBuilderMaster.beltBuildingData);
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
        buildingDragFromInventoryOverlay.SetActive(true);
        dragBuildDat = building;
        inventoryDragBegun = true;
    }

    public void StopDragInventoryBuilding () {
        buildingDragFromInventoryOverlay.SetActive(false);
        inventoryDragBegun = false;
        UpdateSlotsBuildableStates();
    }

    void OnGUI () {
        if (inventoryDragBegun) {
            float aspectRatio = dragBuildDat.BuildingSprite.rect.width / dragBuildDat.BuildingSprite.rect.height;
            Rect target = new Rect(SmartInput.inputPos.x, Screen.height - SmartInput.inputPos.y, 200f* aspectRatio, 200f);
            GUIDrawSprite(target, dragBuildDat.BuildingSprite);
        }
    }

    BuildingData barSlotOldBuilding;
    public void PointerEnterBuildingBarSlot (int slot) {
        if (inventoryDragBegun) {
            barSlotOldBuilding = myBuildingBarSlots[slot].myDat;
            myBuildingBarSlots[slot].ChangeBuilding(dragBuildDat);
        }
    }

    public void PointerExitBuildingBarSlot (int slot) {
        if (inventoryDragBegun) {
            RectTransform myRect = myBuildingBarSlots[slot].GetComponent<RectTransform>();
            Vector2 localMousePosition = myRect.InverseTransformPoint(SmartInput.inputPos);
            if (!myRect.rect.Contains(localMousePosition)) {
                myBuildingBarSlots[slot].ChangeBuilding(barSlotOldBuilding);
                barSlotOldBuilding = null;
            }
        }
    }


    public void StartBuildingFromSlot (BuildingData dat) {
        beltBuildingOverlay.SetActive(false);
        sellModeOverlay.SetActive(false);
        Player_MasterControlCheck.s.buildingController.TryToPlaceItem(dat);
        
        scont.BringBuildingBarToFocus();
    }

    public void SellMode () {
        beltBuildingOverlay.SetActive(false);
        sellModeOverlay.SetActive(true);
        Player_MasterControlCheck.s.buildingController.ActivateSellMode();
        scont.BringBuildingBarToFocus();
    }

    public void BeltMode () {
        if (!CanPlaceBelts) {
            Deselect();
            return;
        }
        beltBuildingOverlay.SetActive(true);
        sellModeOverlay.SetActive(false);
        Player_MasterControlCheck.s.buildingController.ActivateBeltMode();
        scont.BringBuildingBarToFocus();
    }

    public void Deselect () {
        beltBuildingOverlay.SetActive(false);
        sellModeOverlay.SetActive(false);
        Player_MasterControlCheck.s.buildingController.Deselect();
    }

    void SaveBuildingSlots () {
        DataSaver.BuildingBarDataToBeSaved = new string[myBuildingBarSlots.Length];
        for (int i = 0; i < myBuildingBarSlots.Length; i++) {
            if (myBuildingBarSlots[i].myDat != null) {
                DataSaver.BuildingBarDataToBeSaved[i] = myBuildingBarSlots[i].myDat.uniqueName;
            }
        }
    }

	public void OnDestroy () {
        GameLoader.RemoveFromCall(LoadBuildingSlots);
        DataSaver.saveEvent -= SaveBuildingSlots;
        Player_InventoryController.inventoryContentsChangedEvent -= UpdateSlotsBuildableStates;
    }


	static void GUIDrawSprite (Rect rect, Sprite sprite) {
        Rect spriteRect = sprite.rect;
        Texture2D tex = sprite.texture;
        GUI.DrawTextureWithTexCoords(rect, tex, new Rect(spriteRect.x / tex.width, spriteRect.y / tex.height, spriteRect.width / tex.width, spriteRect.height / tex.height));
    }
}
