using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGUI_InventoryListing : MonoBehaviour {

    public Text nameText;
    public Text numberText;
    public RawImage img;
    public InventoryItemSlot myDat;
    GUI_InventoryController myCont;

    public void SetUp (InventoryItemSlot _myDat, GUI_InventoryController _myCont) {
        myDat = _myDat;
        myCont = _myCont;
        UpdateAmount();

        Player_InventoryController.inventoryContentsChangedEvent += UpdateAmount;
    }

    public void UpdateAmount () {
        if (myDat.count > 0) {
            nameText.text = myDat.myItem.name;
            numberText.text = "x" + myDat.count.ToString();
            img.material = myDat.myItem.GetMaterial();
            img.color = Color.white;
        } else {
            nameText.text = "Empty";
            numberText.text = "";
            img.color = new Color(0, 0, 0, 0);
        }
    }

	private void OnDestroy () {
        Player_InventoryController.inventoryContentsChangedEvent -= UpdateAmount;
    }
}
