using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// A helper class for the individual inventory slot listings in the inventory ui panel
/// </summary>
public class MiniGUI_InventoryListing : MonoBehaviour {

    public Text nameText;
    public Text numberText;
    public Image img;
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
            img.sprite = myDat.myItem.GetSprite();
            img.enabled = true;
        } else {
            nameText.text = "Empty";
            numberText.text = "";
            img.enabled = false;
        }
    }

	private void OnDestroy () {
        Player_InventoryController.inventoryContentsChangedEvent -= UpdateAmount;
    }
}
