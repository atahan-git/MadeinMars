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

    public bool isRocket = false;
    public List<InventoryItemSlot> myInv;

    private void Start() {
        myCont = GUI_BuildingBarController.s;
    }


    private SuccessFailCallback _callback;
    public void ChangeBuilding (BuildingData _myDat, bool _isRocket, List<InventoryItemSlot> inv, bool isTimed = false, float timer = 0, SuccessFailCallback callback = null) {
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

        _callback = callback;
        isRocket = _isRocket;
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
        if (state) {
            myCont.StartBuildingFromSlot(myDat, isRocket, myInv, _callback);
        }
    }
    
    public void PointerEnter () {
        myCont.PointerEnterBuildingBarSlot(myID);
    }

    public void PointerExit () {
        myCont.PointerExitBuildingBarSlot(myID);
    }
}
