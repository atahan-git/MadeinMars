using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DialogMaster : MonoBehaviour {
	public static DialogMaster s;
	
	private void Awake () {
		if (s != null) {
			Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
		}
		s = this;
		dialogTreeDisplayer.gameObject.SetActive(false);
	}


	public DialogOptionPickedCallback dialogOptionPickedCallback;

	public DialogTreeAsset[] allDialogs = new DialogTreeAsset[0];
	public DialogTree dialogTreeDisplayer;


	public void StartDialog(string dialogName) {
		for (int i = 0; i < allDialogs.Length; i++) {
			if (allDialogs[i].name == dialogName) {
				dialogTreeDisplayer.LoadFromAsset(allDialogs[i]);
				return;
				
			}
		}

		
		Debug.LogError("Non-existent dialog was tried to be played.");
	}
	
	
}


public delegate void DialogOptionPickedCallback(string uniqueName, int value);
