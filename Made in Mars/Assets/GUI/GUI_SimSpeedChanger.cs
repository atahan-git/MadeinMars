using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_SimSpeedChanger : MonoBehaviour {

    public Text simSpeed;
    // Start is called before the first frame update
    void Start() {
        UpdateSimSpeedText();
    }

    void UpdateSimSpeedText() {
        simSpeed.text = "UPS = " + FactoryMaster.SimUpdatePerSecond + "/s";
    }


    public void ChangeSimSpeed(float delta) {
        if (delta == 0) {
            FactoryMaster.SimUpdatePerSecond = 4;
        } else {
            FactoryMaster.SimUpdatePerSecond += delta;
        }

        FactoryMaster.SimUpdatePerSecond = Mathf.Clamp(FactoryMaster.SimUpdatePerSecond, 0.5f, 32f);
        UpdateSimSpeedText();
    }
}
