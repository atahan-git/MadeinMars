using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ShipDataMaster : MonoBehaviour {
    public static ShipDataMaster s;

    public List<BuildingData> playerBuildableBuildingsSetByShipCards = new List<BuildingData>();

    public List<InventoryItemSlot> shipStarterInventory = new List<InventoryItemSlot>();

    public int droneCount = 0;

    public delegate void CardListUpdated(List<IShipCard> newCards);

    public static CardListUpdated activeCardsUpdatedEvent;
    public static CardListUpdated availableCardsUpdatedEvent;

    public void Awake() {
        if (s != null) {
            Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
        }

        s = this;
        GameMaster.CallWhenLoaded(SetupActiveShipCards);
    }

    public void OnDestroy() {
        GameMaster.RemoveFromCall(SetupActiveShipCards);
        s = null;
    }

    void SetupActiveShipCards(bool isLoadingSuccess) {
        ApplyShipCardEffects();
        activeCardsUpdatedEvent?.Invoke(GetActiveCards());
        availableCardsUpdatedEvent?.Invoke(GetAvailableCards());
    }



    public List<BuildingData> GetPlayerBuildableBuildings() {
        return playerBuildableBuildingsSetByShipCards;
    }


    public void ApplyShipCardEffects() {
        ShipCardApplicableObjects data = new ShipCardApplicableObjects(this);
        playerBuildableBuildingsSetByShipCards.Clear();
        shipStarterInventory.Clear();
        
        var activeShipCards = GetActiveCards();

        for (int i = 0; i < activeShipCards.Count; i++) {
            activeShipCards[i].ApplyEffect(data);
        }
    }

    public bool AddActiveCard(IShipCard card) {
        if (DataSaver.s.mySave.activeShipCards.Count < 8) {
            DataSaver.s.mySave.activeShipCards.Add(new DataSaver.ShipCardData(card as ShipCard));
            ApplyShipCardEffects();
            activeCardsUpdatedEvent?.Invoke(GetActiveCards());
            return true;
        } else {
            return false;
        }
    }

    public void RemoveActiveCard(int index) {
        DataSaver.s.mySave.activeShipCards.RemoveAt(index);
            ApplyShipCardEffects();
        activeCardsUpdatedEvent?.Invoke(GetActiveCards());
    }

    public List<IShipCard> GetActiveCards() {
        var activeCards = new List<IShipCard>();
        var savedCards = DataSaver.s.mySave.activeShipCards;

        for (int i = 0; i < savedCards.Count; i++) {
            var card = DataHolder.s.GetShipCard(savedCards[i].myUniqueName);
            if(card != null)
                activeCards.Add(card as IShipCard);
        }
        
        
        return activeCards;
    }

    
    public bool AddAvailableCard(IShipCard card) {
        DataSaver.s.mySave.availableShipCards.Add(new DataSaver.ShipCardData(card as ShipCard));
        availableCardsUpdatedEvent?.Invoke(GetAvailableCards());
        return true;
    }

    /*public bool RemoveAvailableCard(IShipCard card) {
        var result  = DataSaver.s.mySave.availableShipCards.Remove(new DataSaver.ShipCardData(card as ShipCard));
        availableCardsUpdatedEvent?.Invoke(GetAvailableCards());
        return result;
    }*/

    public List<IShipCard> GetAvailableCards() {
        var availableCards = new List<IShipCard>();
        var savedCards = DataSaver.s.mySave.availableShipCards;

        for (int i = 0; i < savedCards.Count; i++) {
            var card = DataHolder.s.GetShipCard(savedCards[i].myUniqueName);
            if(card != null)
                availableCards.Add(card as IShipCard);
        }
        
        
        return availableCards;
    }
}


public class ShipCardApplicableObjects {
    public ShipDataMaster master;

    public ShipCardApplicableObjects(ShipDataMaster master) {
        this.master = master;
    }
}


public interface IShipCard {
    void ApplyEffect(ShipCardApplicableObjects data);
}