using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_CommsController : MonoBehaviour {

    public ShopItem[] buyItems;
    public ShopItem[] sellItems;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    [Serializable]
    public class ShopItem {
        public string uniqueName = "";
        public int rewardAmount = 1;
        public float cost = 100;
    }
}


