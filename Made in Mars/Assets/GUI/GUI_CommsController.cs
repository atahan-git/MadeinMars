using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Controls the "Comms" UI panel
/// </summary>
public class GUI_CommsController : MonoBehaviour {


    public Text moneyDisp;

    private Player_CommsController coms;
    // Start is called before the first frame update
    void Start() {
        coms = transform.parent.GetComponentInChildren<Player_CommsController>();
        UpdateMoneyText();
        SetUpPanels();
    }
    
    void UpdateMoneyText() {
        moneyDisp.text = FormatMoney(coms.money);
    }

    public static string FormatMoney(float money) {
        return "$" + String.Format("{0:0,0}", money).Replace(',',' ') + "M";
    }

    public GameObject BuyMenuPrefab;
    public Transform BuyMenuParent;

    private MiniGUI_BuySellMenu buymenu;
    private MiniGUI_BuySellMenu sellmenu;
    void SetUpPanels () {
        buymenu = Instantiate(BuyMenuPrefab, BuyMenuParent).GetComponent<MiniGUI_BuySellMenu>();
        buymenu.SetUp(this, DataHolder.s.GetAllItems(),true, coms.availableShipCount);
        buymenu.ClosePanel();
        sellmenu = Instantiate(BuyMenuPrefab, BuyMenuParent).GetComponent<MiniGUI_BuySellMenu>();
        sellmenu.SetUp(this, DataHolder.s.GetAllItems(),false, coms.availableShipCount);
        sellmenu.ClosePanel();
    }

    [Space]
    public GameObject RocketJourneyPrefab;
    public Transform RocketJourneyParent;

    public BuildingData rocketData;
    public void BuyShipWithMaterial(List<InventoryItemSlot> items) {
        var myShip = Instantiate(RocketJourneyPrefab, RocketJourneyParent);
        myShip.GetComponent<MiniGUI_BuildingBarSlot>().ChangeBuilding(rocketData, true,true, true, items,true,20);
    }
    
    public void SellShipWithMaterial(List<InventoryItemSlot> items) {
        
    }
    
    public void OpenBuyPanel() {
        buymenu.OpenPanel();
    }

    public void OpenSellPanel() {
        sellmenu.OpenPanel();
    }
}
