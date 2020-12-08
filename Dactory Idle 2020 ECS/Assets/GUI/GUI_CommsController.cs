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
    }

    // Update is called once per frame
    void Update() {
        moneyDisp.text = FormatMoney(coms.money);
    }


    static string FormatMoney(float money) {
        return "$" + String.Format("{0:0,0}", money).Replace(',',' ') + "M";
    }
}
