using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class GUI_PopulationDisplay : MonoBehaviour {

    public Text popDisplay;
    // Update is called once per frame
    void Update() {
        popDisplay.text = FactoryMaster.s.housed + "/" + FactoryMaster.s.population + " - " + FactoryMaster.s.workers + "/" + FactoryMaster.s.jobs;
    }
}
