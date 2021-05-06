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
    public Color storageColor = Color.white;

    public Image bg;
    private bool updateColor;
    public void SetUp (InventoryItemSlot _myDat, IInventoryController _myCont, bool _updateColor) {
        myDat = _myDat;
        myCont = _myCont;
        updateColor = _updateColor;
        
        UpdateAmount();

        myCont.inventoryContentsChangedEvent += UpdateAmount;

        if (updateColor) {
            switch (myDat.mySlotType) {
                case InventoryItemSlot.SlotType.output:
                    bg.color = outputColor;
                    
                    break;
                case InventoryItemSlot.SlotType.input:
                    bg.color = inputColor;
                    
                    break;
                case InventoryItemSlot.SlotType.storage:
                    bg.color = storageColor;
                    
                    break;
            }
        }
    }

    public void UpdateAmount () {
        if (myDat.count > 0 || (updateColor && !myDat.myItem.isEmpty())) {
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
