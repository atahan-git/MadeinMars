﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// A helper class for the individual inventoryItemSlots slot listings in the inventoryItemSlots ui panel
/// </summary>
public class MiniGUI_InventoryListing : MonoBehaviour {

    public Text nameText;
    public Text numberText;
    public Image img;
    public InventoryItemSlot myDat;
    private IInventoryDisplayable myCont;

    public Color inputColor = Color.green;
    public Color outputColor = Color.red;
    public Color storageColor = Color.white;
    public Color houseColor = Color.blue;
    public Color workerColor = Color.cyan;

    public Image bg;
    private bool updateColor;
    public void SetUp (InventoryItemSlot _myDat, IInventoryDisplayable _myCont, bool _updateColor) {
        if (myCont != null) {
            myCont.inventoryContentsChangedEvent -= UpdateAmount;
        }
        
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
                case InventoryItemSlot.SlotType.house:
                    bg.color = houseColor;
                    
                    break;
                case InventoryItemSlot.SlotType.worker:
                    bg.color = workerColor;
                    
                    break;
            }
        }
    }

    public void UpdateAmount () {
        if (myDat.count > 0 || (updateColor && !myDat.myItem.isEmpty())) {
            if (img != null) {
                nameText.text = myDat.myItem.name;
                numberText.text = $"x{myDat.count}/{myDat.maxCount}";
                img.sprite = myDat.myItem.GetSprite();
                img.enabled = true;
            }
        } else {
            if (img != null) {
                nameText.text = "Empty";
                numberText.text = ""; 
                img.enabled = false;
            }
        }
    }

	private void OnDestroy () {
        myCont.inventoryContentsChangedEvent -= UpdateAmount;
    }
}
