using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



/// <summary>
/// A helper class for the individual building slots in the building bar
/// </summary>
public class MiniGUI_BuildingBarSlot : MonoBehaviour
{
    public int myID = -1;
    public GUI_BuildingBarController myCont;

    public Text nameText;
    public Image img;
    public BuildingData myDat;

    public Image bgImg;

    public bool state = true;

    public Color defColor = Color.white;

    public bool isOneUse = false;
    public bool isInventory = false;
    public bool isSpaceLanding = false;
    public List<InventoryItemSlot> myInv;

    private void Start() {
        myCont = GUI_BuildingBarController.s;
    }

    /// <summary>
    /// Use to change the building set in the building slot.
    /// The player will later be able to build this.
    /// </summary>
    /// <param name="_myDat"></param>
    /// <param name="_isSpaceLanding"></param>
    /// <param name="_isInventory"></param>
    /// <param name="inv"></param>
    public void ChangeBuilding (BuildingData _myDat, bool _isOneUse, bool _isSpaceLanding, bool _isInventory, List<InventoryItemSlot> inv, bool isTimed, float timer) {
        myDat = _myDat;
        if (myDat != null) {
            nameText.text = myDat.name;
            img.sprite = myDat.gfxSprite;
            img.color = defColor;
        } else {
            nameText.text = "empty";
            img.sprite = null;
            img.color = new Color(0,0,0,0);
        }

        isInventory = _isInventory;
        isSpaceLanding = _isSpaceLanding;
        isOneUse = _isOneUse;
        myInv = inv;

        if (isTimed) {
            StartCoroutine(Timer(timer));
            UpdateBuildableState(false);
        }
    }

    IEnumerator Timer(float timer) {
        while (timer > 0) {
            nameText.text = TimeSpan.FromSeconds(timer).ToString(@"m\:ss");
            timer -= Time.deltaTime;
            yield return null;
        }

        nameText.text = "ready";
        UpdateBuildableState(true);
    }


    public void UpdateBuildableState (bool _state) {
        state = _state;
        if (state) {
            bgImg.color = new Color(1, 1, 1);
        } else {
            float darkness = 0.5f;
            bgImg.color = new Color(darkness, darkness, darkness);
        }
    }

    public void PointerDown () {
        if (state)
            myCont.StartBuildingFromSlot(myDat, isSpaceLanding, isInventory, myInv, BuildingBuildCompleteCallback);
    }

    public void BuildingBuildCompleteCallback() {
        if(isOneUse)
            Destroy(gameObject);
    }

    public void PointerEnter () {
        myCont.PointerEnterBuildingBarSlot(myID);
    }

    public void PointerExit () {
        myCont.PointerExitBuildingBarSlot(myID);
    }
}
