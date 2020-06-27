using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGUI_BuildingListing : MonoBehaviour {

    public Text name;
    public Image img;
    public BuildingData myDat;
    GUI_BuildingBarController myCont;
    
    public void SetUp (BuildingData _myDat, GUI_BuildingBarController _myCont) {
        myDat = _myDat;
        name.text = myDat.name;
        img.sprite = myDat.BuildingSprite;
        myCont = _myCont;
    }

    public void BeginDrag () {
        myCont.BeginDragInventoryBuilding(myDat);
    }
}
