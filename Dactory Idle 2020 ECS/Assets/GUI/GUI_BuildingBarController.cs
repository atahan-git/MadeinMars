using System;
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
    public GameObject sellModeOverlay;
    public GameObject buildingDragFromInventoryOverlay;

    // public int buildingSlotCount = 3; >> possibly auto generate building bar slots?

    public MiniGUI_BuildingBarSlot[] myBuildingBarSlots;

    public Image PlaceBeltsButton;
    public bool CanPlaceBelts = true;

    public Toggle beltSafeModeToggle;

    private void Awake () {
        if (s != null) {
            Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
        }
        s = this;
        GameLoader.CallWhenLoaded(LoadBuildingSlots);
    }

    private void Start () {
        DataSaver.saveEvent += SaveBuildingSlots;

        Player_InventoryController.s.inventoryContentsChangedEvent += UpdateSlotsBuildableStates;

        scont = GetComponent<GUI_SwitchController>();
    }

    void LoadBuildingSlots () {
        if (DataSaver.mySave != null) {
            if (DataSaver.mySave.buildingBarData != null) {
                for (int i = 0; i < DataSaver.mySave.buildingBarData.Length; i++) {
                    if (DataSaver.mySave.buildingBarData[i] != null)
                        if (DataSaver.mySave.buildingBarData[i].Length > 0)
                            myBuildingBarSlots[i].ChangeBuilding(
                                DataHolder.s.GetBuilding(DataSaver.mySave.buildingBarData[i]),
                                false,false,false,null, false ,0);

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
                myBuildingBarSlots[slot].ChangeBuilding(dragBuildDat, false,false, false, null, false , 0);
            }
        }
    }

    public void PointerExitBuildingBarSlot (int slot) {
        if (inventoryDragBegun) {
            RectTransform myRect = myBuildingBarSlots[slot].GetComponent<RectTransform>();
            Vector2 localMousePosition = myRect.InverseTransformPoint(SmartInput.inputPos);
            if (!myRect.rect.Contains(localMousePosition)) {
                myBuildingBarSlots[slot].ChangeBuilding(barSlotOldBuilding, false,false,false,null, false, 0);
                barSlotOldBuilding = null;
            }
        }
    }


    public void StartBuildingFromSlot (BuildingData dat, bool isSpaceLanding, bool isInventory, List<InventoryItemSlot> inv, GenericCallback buildCompleteCallback) {
        beltBuildingOverlay.SetActive(false);
        sellModeOverlay.SetActive(false);
        Player_MasterControlCheck.s.buildingController.TryToPlaceItem(dat, isSpaceLanding,  isInventory, inv, buildCompleteCallback);
        
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

    public void ToggleBeltSafeMode() {
        Player_MasterControlCheck.s.buildingController.ToggleBeltSafeMode(beltSafeModeToggle.isOn);
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
        Player_InventoryController.s.inventoryContentsChangedEvent -= UpdateSlotsBuildableStates;
    }


	static void GUIDrawSprite (Rect rect, Sprite sprite) {
        Rect spriteRect = sprite.rect;
        Texture2D tex = sprite.texture;
        GUI.DrawTextureWithTexCoords(rect, tex, new Rect(spriteRect.x / tex.width, spriteRect.y / tex.height, spriteRect.width / tex.width, spriteRect.height / tex.height));
    }
}
