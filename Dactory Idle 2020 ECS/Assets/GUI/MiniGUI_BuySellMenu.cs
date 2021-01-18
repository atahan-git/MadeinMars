using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGUI_BuySellMenu : MonoBehaviour {

    public GUI_CommsController MyController;
    public GameObject ItemSelectionBoxPrefab;

    public Transform ItemSelectionParent;


    public Text shipCountText;
    public Text shipCapacityText;
    public Text shipCostText;
    public Color normalColor = Color.black;
    public Color moneyColor = Color.green;
    public Color unavailableColor = Color.red;
    
    [Space]
    public MiniGUI_ItemSelectionBox[] itemSelectors;

    public bool isBuy;
    public void SetUp(GUI_CommsController controller, Item[] items, bool _isBuy, int shipCount) {
        MyController = controller;
        isBuy = _isBuy;
        itemSelectors = new MiniGUI_ItemSelectionBox[items.Length];
        
        for (int i = 0; i < items.Length; i++) {
            itemSelectors[i] = Instantiate(ItemSelectionBoxPrefab, ItemSelectionParent).GetComponent<MiniGUI_ItemSelectionBox>();
            itemSelectors[i].SetUp(items[i], isBuy, this);
        }

        shipCountText.text = "Available Ships: " + shipCount.ToString();
        
        ValueChangedCallback();
    }

    public void OpenPanel() {
        gameObject.SetActive(true);
    }

    private float curCapacity = 0;
    private const float carryCapacity = 100; // This should eventually be set by the Player_CommsController
    private float curCost = 0;

    private bool canSendShip = false;
    public void ValueChangedCallback() {
        curCapacity = 0;
        curCost = 0;
        canSendShip = true;
        for (int i = 0; i < itemSelectors.Length; i++) {
            curCapacity += itemSelectors[i].amount * itemSelectors[i].myItem.weight;
            if (isBuy) {
                curCost += itemSelectors[i].amount * itemSelectors[i].myItem.buyCost;
            } else {
                curCost += itemSelectors[i].amount * itemSelectors[i].myItem.sellCost;
                if (Player_InventoryController.s.GetItemCount(itemSelectors[i].myItem) < itemSelectors[i].amount) {
                    itemSelectors[i].SetAvailabilityInInventory(false);
                    canSendShip = false;
                } else {
                    itemSelectors[i].SetAvailabilityInInventory(true);
                }
            }
        }

        shipCountText.text = "Available Ships:\n" + Player_CommsController.s.availableShipCount.ToString() + "/" + 3.ToString();
        shipCapacityText.text = "Capacity:\n" + curCapacity.ToString() +"/" + carryCapacity.ToString() + " kg";
        shipCostText.text = "Cost:\n" + GUI_CommsController.FormatMoney(curCost);

        if (curCapacity <= carryCapacity) {
            shipCapacityText.color = normalColor;
        } else {
            shipCapacityText.color = unavailableColor;
            canSendShip = false;
        }

        if (isBuy) {
            if (curCost <= Player_CommsController.s.money) {
                shipCostText.color = moneyColor;
            } else {
                shipCostText.color = unavailableColor;
                canSendShip = false;
            }
        } else {
            shipCostText.color = moneyColor;
        }
    }

    public void Request() {
        if (canSendShip) {
            List<InventoryItemSlot> myItems = new List<InventoryItemSlot>();

            for (int i = 0; i < itemSelectors.Length; i++) {
                if (itemSelectors[i].amount > 0) {
                    myItems.Add(new InventoryItemSlot(itemSelectors[i].myItem, itemSelectors[i].amount, itemSelectors[i].amount, true));
                    itemSelectors[i].ResetAmount();
                }
            }
        

            if (isBuy) {
                MyController.BuyShipWithMaterial(myItems);
            } else {
                MyController.SellShipWithMaterial(myItems);
            }
            ClosePanel();
            GUI_SwitchController.s.HideAllMenus();
        }
    }

    public void ClosePanel() {
        gameObject.SetActive(false);
    }
}
