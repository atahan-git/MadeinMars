using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogDecisionMaster : MonoBehaviour {
	public static DialogDecisionMaster s;

	int decisionSaveId = -1;
	bool isDecisionDone = false;

	public GameObject decisionScreen;
	public Text questionText;

	public GameObject decisionButtonsParent;
	public GameObject decisionButtonPrefab;

	private void Start () {
		s = this;
		decisionScreen.SetActive (false);
	}

	//example: <triggerChoice=["Kervana napcan Reiz?";0;1,"Yardım Et":2,"Görev Rezi Daha Önemli"]>
	//data = ["Question Text";<decisionSaveId>;<decisionNumber>,"Decision Button Text":<other Decision>,"etc"]
	public void ShowDecisionScreen (string data) {
		isDecisionDone = false;
		data = data.Substring (1, data.Length - 2);
		try {
			string[] parts = data.Split (';');

			questionText.text = parts[0].Substring (1, parts[0].Length - 2);
			decisionSaveId = int.Parse (parts[1]);

			string[] buttonData = parts[2].Split (':');

			for (int i = 0; i < buttonData.Length; i++) {
				string[] curButtonData = buttonData[i].Split (',');

				GameObject myBut = Instantiate (decisionButtonPrefab, decisionButtonsParent.transform);

				myBut.GetComponentInChildren<Button> ().onClick.AddListener (delegate { Choose (float.Parse (curButtonData[0])); });
				myBut.GetComponentInChildren<Text> ().text = curButtonData[1].Substring (1, curButtonData[1].Length - 2);
			}
		} catch (System.Exception e) {
			Debug.LogError ("Cant Parse data for Decicion Master: " + data + e);
			isDecisionDone = true;
			decisionScreen.SetActive (false);
			return;
		}

		decisionScreen.SetActive (true);
	}

	public void Choose (float decision) {
		/*try {
			//SaveMaster.s.mySave.questDecisions[decisionSaveId] = decision;
			
		} catch (System.Exception e) {
			Debug.LogError ("Cant save decision", e);
		}*/

		throw new NotImplementedException();

		isDecisionDone = true;
		decisionScreen.SetActive (false);
	}

	public bool IsDecisionDone () {
		return isDecisionDone;
	}
}
