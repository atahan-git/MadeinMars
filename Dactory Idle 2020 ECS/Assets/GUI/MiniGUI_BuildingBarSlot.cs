using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGUI_BuildingBarSlot : MonoBehaviour
{
    public int myID;
    public GUI_BuildingBarController mycont;

    public Text name;
    public Image img;
    public BuildingData myDat;


	public void ChangeBuilding (BuildingData _myDat) {
        myDat = _myDat;
        if (myDat != null) {
            name.text = myDat.name;
            img.sprite = myDat.BuildingSprite;
            img.color = Color.white;
        } else {
            name.text = "empty";
            img.sprite = null;
            img.color = new Color(0,0,0,0);
        }
    }

    public void PointerDown () {
        if (myDat != null) {
            mycont.GetBuildingFromSlot(myDat);
        }
    }

    public void PointerEnter () {
        mycont.PointerEnterBuildingBarSlot(myID);
    }

    public void PointerExit () {
        mycont.PointerExitBuildingBarSlot(myID);
    }
}
