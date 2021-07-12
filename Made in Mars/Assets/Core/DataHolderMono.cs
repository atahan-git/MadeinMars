using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ExceptionServices;
using UnityEngine;
using Random = UnityEngine.Random;


public class DataHolderMono : MonoBehaviour{
	public DataHolder myDataHolder;
	
	private void Awake () {
		if (DataHolder.s != null) {
			Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
		}
		DataHolder.s = myDataHolder;
		
		
		DataHolder.s.Setup();
	}

	private void OnDestroy() {
		DataHolder.s = null;
	}
}