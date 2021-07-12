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

    void SetUpPanels () {
       
    }

    [Space]
    public GameObject RocketJourneyPrefab;
    public Transform RocketJourneyParent;

    public BuildingData rocketData;
    public void BuyShipWithMaterial(List<InventoryItemSlot> items) {
        var myShip = Instantiate(RocketJourneyPrefab, RocketJourneyParent);
        myShip.GetComponent<MiniGUI_BuildingBarSlot>().ChangeBuilding(rocketData, true, items,true,3);
    }
    
    public void SellShipWithMaterial(List<InventoryItemSlot> items) {
        
    }
    
}
