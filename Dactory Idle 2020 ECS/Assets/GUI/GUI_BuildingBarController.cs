using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GUI_BuildingBarController : MonoBehaviour {

    public bool isOnFocus = true;
    public GUI_SwitchController scont;

    public GameObject beltBuildingOverlay;
    public GameObject sellModeOverlay;
    public GameObject buildingDragFromInventoryOverlay;

    // public int buildingSlotCount = 3; >> possibly auto generate building bar slots?

    public MiniGUI_BuildingBarSlot[] buildingBarSlots;

    private void Start () {
        if (GameLoader.isGameLoadingDone)
            LoadBuildingSlots();
        else
            DataSaver.saveEvent += SaveBuildingSlots;

        scont = GetComponent<GUI_SwitchController>();
    }

    void LoadBuildingSlots () {
        for (int i = 0; i < DataSaver.mySave.buildingBarData.Length; i++) {
            if (DataSaver.mySave.buildingBarData[i] != "") {
                buildingBarSlots[i].ChangeBuilding(DataHolder.s.GetBuilding(DataSaver.mySave.buildingBarData[i]));
            }
        }
    }

    private void Update () {
        if (inventoryDragBegun)
            if (Input.GetMouseButtonUp(0))
                StopDragInventoryBuilding();
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
            barSlotOldBuilding = buildingBarSlots[slot].myDat;
            buildingBarSlots[slot].ChangeBuilding(dragBuildDat);
        }
    }

    public void PointerExitBuildingBarSlot (int slot) {
        if (inventoryDragBegun) {
            buildingBarSlots[slot].ChangeBuilding(barSlotOldBuilding);
            barSlotOldBuilding = null;
        }
    }


    public void GetBuildingFromSlot (BuildingData dat) {
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
        DataSaver.BuildingBarDataToBeSaved = new string[buildingBarSlots.Length];
        for (int i = 0; i < buildingBarSlots.Length; i++) {
            if (buildingBarSlots[i].myDat != null) {
                DataSaver.BuildingBarDataToBeSaved[i] = buildingBarSlots[i].myDat.uniqueName;
            }
        }
    }


    static void GUIDrawSprite (Rect rect, Sprite sprite) {
        Rect spriteRect = sprite.rect;
        Texture2D tex = sprite.texture;
        GUI.DrawTextureWithTexCoords(rect, tex, new Rect(spriteRect.x / tex.width, spriteRect.y / tex.height, spriteRect.width / tex.width, spriteRect.height / tex.height));
    }
}
