using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;



/// <summary>
/// A helper class for the individual item selectors in the comms item buy/sell menu
/// </summary>
public class MiniGUI_ItemSelectionBox : MonoBehaviour {
    public Item myItem;
    public bool isBuy;
    
    public Image icon;
    public Text itemName;
    public Text itemCost;
    public Text itemWeight;
    public Text itemCurrentAmount;
    public Text itemCurrentWeight;
    public Text itemCurrentCost;

    public int amount = 0;

    public void SetUp(Item item, bool _isBuy) {
        myItem = item;
        isBuy = _isBuy;
        icon.sprite = myItem.GetSprite();
        itemName.text = myItem.name;

        if (isBuy) {
            itemCost.text = GUI_CommsController.FormatMoney(myItem.buyCost);
        } else {
            itemCost.text = GUI_CommsController.FormatMoney(myItem.sellCost);
        }
        
        itemWeight.text = myItem.weight.ToString() + " kg";

        UpdateAmountText();
    }

    void UpdateAmountText() {
        itemCurrentAmount.text = amount.ToString();
        itemCurrentWeight.text = (amount * myItem.weight).ToString() + "kg";
        if (isBuy) {
            itemCurrentCost.text = GUI_CommsController.FormatMoney(myItem.buyCost *amount);
        } else {
            itemCurrentCost.text = GUI_CommsController.FormatMoney(myItem.sellCost *amount);
        }
    }

    public void IncreaseAmount() {
        amount += myItem.butsellamount;
        
        UpdateAmountText();
    }

    public void DecreaseAmount() {
        amount -= myItem.butsellamount;

        if (amount < 0)
            amount = 0;
        
        UpdateAmountText();
    }
}
