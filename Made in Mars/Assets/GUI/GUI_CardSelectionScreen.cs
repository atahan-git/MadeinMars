using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class GUI_CardSelectionScreen : MonoBehaviour {

    public MiniGUI_CardPanel activeCardsPanel;

    public MiniGUI_CardPanel availableCardsPanel;


    private void Awake() {
        ShipDataMaster.activeCardsUpdatedEvent += RedrawActiveCardsPanel;
        ShipDataMaster.availableCardsUpdatedEvent += RedrawAvailableCardsPanel;
    }

    private void OnDestroy() {
        ShipDataMaster.activeCardsUpdatedEvent -= RedrawActiveCardsPanel;
        ShipDataMaster.availableCardsUpdatedEvent -= RedrawAvailableCardsPanel;
    }

    private void  RedrawActiveCardsPanel(List<IShipCard> cards) {
        activeCardsPanel.RedrawListOfCards(cards.Cast<ShipCard>().ToList());
    }



    void RedrawAvailableCardsPanel(List<IShipCard> cards) {
        availableCardsPanel.RedrawListOfCards(cards.Cast<ShipCard>().ToList());
    }


    IShipCard GetCardFromPointerData(PointerEventData data) {
        var originalObj = data.pointerDrag;
        if (originalObj == null)
            return null;

        var miniGUICard = originalObj.GetComponent<MiniGUI_Card>();
        if (miniGUICard == null)
            return null;

        var myCard = miniGUICard.myCard;
        if (myCard == null)
            return null;

        return (IShipCard) myCard;
    }
    
    int GetCardIndexFromPointerData(PointerEventData data) {
        var originalObj = data.pointerDrag;
        if (originalObj == null)
            return -1;

        var index = originalObj.transform.GetSiblingIndex();

        return index;
    }

    public void OnCardDropToActivePanel(PointerEventData data) {
        var myCard = GetCardFromPointerData(data);

        if (myCard != null) {
            ShipDataMaster.s.AddActiveCard(myCard);
        }
    }

    public void OnCardDropToAvailablePanel(PointerEventData data) {
        var myCardIndex = GetCardIndexFromPointerData(data);

        if (myCardIndex != -1) {
            ShipDataMaster.s.RemoveActiveCard(myCardIndex);
            
            activeCardsPanel.RedrawListOfCards(ShipDataMaster.s.GetActiveCards().Cast<ShipCard>().ToList());
        }
    }
}
