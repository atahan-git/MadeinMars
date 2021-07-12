using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryMasterMono : MonoBehaviour {
	public FactoryMaster myFactoryMaster;

    private void Awake () {
		if (FactoryMaster.s != null) {
			Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
		}
		FactoryMaster.s = myFactoryMaster;
    }

    private void Start() {
	    FactoryMaster.s.RegisterLoad();
    }

    private void OnDestroy() {
	    FactoryMaster.s.UnregisterLoad();
	    FactoryMaster.s = null;
    }
}