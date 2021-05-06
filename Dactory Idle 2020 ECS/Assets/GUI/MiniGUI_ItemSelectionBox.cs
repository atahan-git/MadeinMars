using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;



/// <summary>
/// A helper class for the individual item selectors in the comms item buy/sell menu
/// </summary>
public class MiniGUI_ItemSelectionBox : MonoBehaviour {
    [FormerlySerializedAs("myItemReference")] public Item myItem;
    public bool isBuy;
    
    public Image icon;
    public Text itemName;
    public Color itemAvailableColor = Color.black;
    public Color itemNotAvailableColor = Color.red;
    public Text itemCost;
    public Text itemWeight;
    public Text itemCurrentAmount;
    public Text itemCurrentWeight;
    public Text itemCurrentCost;

    public int amount = 0;

    private MiniGUI_BuySellMenu master;

    public void SetUp(Item item, bool _isBuy, MiniGUI_BuySellMenu _master) {
        myItem = item;
        isBuy = _isBuy;
        master = _master;
        icon.sprite = myItem.GetSprite();
        itemName.text = myItem.name;

        if (isBuy) {
            itemCost.text = GUI_CommsController.FormatMoney(myItem.buyCost);
        } else {
            itemCost.text = GUI_CommsController.FormatMoney(myItem.sellCost);
        }
        
        itemWeight.text = myItem.weight.ToString() + " kg";

        SetAvailabilityInInventory(true);
        UpdateAmountText(false);
    }

    void UpdateAmountText(bool isCallback) {
        itemCurrentAmount.text = amount.ToString();
        itemCurrentWeight.text = (amount * myItem.weight).ToString() + "kg";
        if (isBuy) {
            itemCurrentCost.text = GUI_CommsController.FormatMoney(myItem.buyCost *amount);
        } else {
            itemCurrentCost.text = GUI_CommsController.FormatMoney(myItem.sellCost *amount);
        }
        
        if(isCallback)
            master.ValueChangedCallback();   
    }

    public void IncreaseAmount() {
        amount += myItem.butsellamount;
        
        UpdateAmountText(true);
    }

    public void DecreaseAmount() {
        amount -= myItem.butsellamount;

        if (amount < 0)
            amount = 0;
        
        UpdateAmountText(true);
    }

    public void ResetAmount() {
        amount = 0;
        UpdateAmountText(false);
    }


    private bool itemAvailable = true;
    public void SetAvailabilityInInventory(bool state) {
        itemAvailable = state;
        if (itemAvailable) {
            itemName.color = itemAvailableColor;
        } else {
            itemName.color = itemNotAvailableColor;
        }
    }
}
