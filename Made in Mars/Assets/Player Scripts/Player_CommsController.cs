using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Handles the background buy/sell operations of the comms menu.
/// Will also handle the objectives
/// </summary>
public class Player_CommsController : MonoBehaviour {
    public static Player_CommsController s;

    public ShopItem[] buyItems;
    public ShopItem[] sellItems;
    
    [Tooltip("Money is in units of Millions of dollars")]
    public float money = 1000;

    public ObjectiveHolder[] objectives;
    [Serializable]
    public class ShopItem {
        public string uniqueName = "";
        public int rewardAmount = 1;
        public float cost = 100;
        public float weight = 1;
    }

    public int availableShipCount = 3;

    private void Awake() {
        if (s != null) {
            Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
        }

        s = this;
    }
}


