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
    private IInventoryController myCont;

    public Color inputColor = Color.green;
    public Color outputColor = Color.red;

    public Image bg;
    private bool updateColor;
    public void SetUp (InventoryItemSlot _myDat, IInventoryController _myCont, bool _updateColor) {
        myDat = _myDat;
        myCont = _myCont;
        updateColor = _updateColor;
        
        UpdateAmount();

        myCont.inventoryContentsChangedEvent += UpdateAmount;

        if (updateColor) {
            if (myDat.isOutputSlot) {
                bg.color = outputColor;
            } else {
                bg.color = inputColor;
            }
        }
    }

    public void UpdateAmount () {
        if (myDat.count > 0 || (updateColor && myDat.myItem != null)) {
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
        myCont.inventoryContentsChangedEvent -= UpdateAmount;
    }
}
