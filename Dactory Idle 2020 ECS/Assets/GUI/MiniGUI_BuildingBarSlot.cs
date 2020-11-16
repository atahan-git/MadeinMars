using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGUI_BuildingBarSlot : MonoBehaviour
{
    public int myID;
    public GUI_BuildingBarController mycont;

    public Text nameText;
    public Image img;
    public BuildingData myDat;

    public Image bgImg;

    public bool state = true;

    public Color defColor = Color.white;

    public void ChangeBuilding (BuildingData _myDat) {
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
        if(state)
            mycont.StartBuildingFromSlot(myDat);
    }

    public void PointerEnter () {
        mycont.PointerEnterBuildingBarSlot(myID);
    }

    public void PointerExit () {
        mycont.PointerExitBuildingBarSlot(myID);
    }
}
