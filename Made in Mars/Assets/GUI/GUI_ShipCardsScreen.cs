using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GUI_ShipCardsScreen : MonoBehaviour {

    public MiniGUI_CardPanel activeCardsPanel;
    
    
    void Awake() {
        ShipDataMaster.activeCardsUpdatedEvent += ReDrawActiveCards;
    }

    private void OnDestroy() {
        ShipDataMaster.activeCardsUpdatedEvent -= ReDrawActiveCards;
    }

    private void ReDrawActiveCards(List<IShipCard> newcards) {
        activeCardsPanel.RedrawListOfCards(newcards.Cast<ShipCard>().ToList());
    }

}
