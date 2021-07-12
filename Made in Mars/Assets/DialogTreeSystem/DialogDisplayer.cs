using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogDisplayer : MonoBehaviour {

	public static DialogDisplayer s;

	public Text myText;
	public Image myImg;
	public GameObject nextDisp;
	public GameObject holdToSkip;
	public GameObject waitOverlay;

	public GameObject dialogParent;

	public float dialogShowSpeed = 0.03f;
	float curDialogShowSpeed = 0.03f; //used to set 0 to fastforward text
	[SerializeField]
	bool isShowing = false;
	[SerializeField]
	bool isLocked = false;

	public float timeSinceLastSkip = 0;
	public int skipCounter = 0;
	public bool canHold = false;


	// Use this for initialization
	void Awake () {
		s = this;
		Clear ();
		myImg.enabled = false;

		DisplayBigSprite (BigSpriteAction.Hide, BigSpriteSlots.left, null);
		DisplayBigSprite (BigSpriteAction.Hide, BigSpriteSlots.middle, null);
		DisplayBigSprite (BigSpriteAction.Hide, BigSpriteSlots.right, null);

		holdToSkip.SetActive (false);
		isLocked = true;
		isShowing = false;

		waitOverlay.SetActive (false);

		SetDialogScreenState(false);
	}

	private void OnDestroy() {
		s = null;
	}

	private void OnEnable () {
		holdToSkip.SetActive (false);
	}

	private void Update () {
		timeSinceLastSkip += Time.deltaTime;

		if (timeSinceLastSkip > 1.5f) {
			timeSinceLastSkip = 0;
			skipCounter = 0;
		}

		if (skipCounter >= 3) {
			holdToSkip.SetActive (true);
			canHold = true;
		}
	}


	public void SetDialogScreenState (bool state) {
		dialogParent.SetActive (state);
	}

	public Image[] bigSpriteSlots;
	public enum BigSpriteAction { Show, Hide };
	public enum BigSpriteSlots { left = 0, middle = 1, right = 2 };

	public void DisplayBigSprite (BigSpriteAction action, BigSpriteSlots slot, Sprite img) {
		try {
			switch (action) {
			case BigSpriteAction.Show:
				bigSpriteSlots[(int)slot].sprite = img;
				bigSpriteSlots[(int)slot].color = new Color (1, 1, 1, 1);
				break;
			case BigSpriteAction.Hide:
				bigSpriteSlots[(int)slot].sprite = null;
				bigSpriteSlots[(int)slot].color = new Color (1, 1, 1, 0);
				break;
			default:

				break;
			}
		} catch (System.Exception e) {
			Debug.LogError (this.name + e);
		}
	}

	public void SetDialog (Sprite image, string text,float delay, bool clearImage) {

		if (clearImage) {
			myImg.sprite = null;
			myImg.enabled = false;
		} else {
			if (image != null) {
				myImg.sprite = image;
				myImg.enabled = true;
			}
		}

		StartCoroutine (ShowDialog(text,delay));
	}


	//Commands cheat sheet: <delay='waitSeconds'>, <wait='click/enabled'>, <give='itemType'-'itemId'>, <trigger='commandID'>, <triggerChoice='questChoiceID'>
	public string curText;
	List<string> itemsToGive;
	IEnumerator ShowDialog (string text, float delay) {
		isLocked = false;
		isShowing = true;
		itemsToGive = new List<string> ();
		curText = RemoveCustomCommands(text);
		string _text = "";

		yield return new WaitForSeconds (delay);

		for (int i = 0; i < text.Length; i++) {
			if (text[i] == '<') {
				string command = "";
				while (text[i + 1] != '>') {
					i++;
					command += text[i];
				}
				i += 1;
				string[] values = command.Split ('=');
				if (values.Length > 1)
					print ("Found custom command: " + values[0] + " = " + values[1]);


				switch (values[0]) {
				case "delay":
					float waitSeconds = 0f;
					try {
						waitSeconds = float.Parse (values[1]);
					} catch {
						Debug.LogError ("Can't parse delay value: " + values[1]);
					}

					curDialogShowSpeed = dialogShowSpeed;
					float timer = 0;
					while (timer < waitSeconds) {
						timer += Time.deltaTime;
						if (curDialogShowSpeed == 0f)
							break;
						yield return null;
					}
					curDialogShowSpeed = dialogShowSpeed;
					break;
				case "wait":
					switch (values[1]) {
					case "click":
						waitOverlay.SetActive (true);
						nextDisp.SetActive (true);
						yield return new WaitUntil (() => Input.GetMouseButtonDown (0));
						waitOverlay.SetActive (false);
						nextDisp.SetActive (false);
						break;
					case "enabled":
						yield return new WaitUntil (() => dialogParent.activeSelf);
						break;
					default:
						Debug.LogError ("Unknown wait value: " + values[1]);
						break;
					}
					break;
				case "give":
					GiveItem (values[1]);
					itemsToGive.Remove (values[1]);
					break;
				case "trigger":
					DialogTree.s.TriggerCommand (values[1]);
					break;
				case "triggerChoice":
					DialogDecisionMaster.s.ShowDecisionScreen (values[1]);
					yield return new WaitUntil (() => DialogDecisionMaster.s.IsDecisionDone());
					break;
				default:
					//this is not a command we recognize, but a command TextMeshPro recognizes, so do add it to the text
					_text += "<" + command + ">";
					break;
				}
			} else {
				if (i < text.Length)
					_text += text[i];

				myText.text = _text;

				if(curDialogShowSpeed != 0f)
					yield return new WaitForSeconds (curDialogShowSpeed);
			}
			
		}

		yield return new WaitForSeconds (0.1f);

		FinishShowingText ();
	}

	void FinishShowingText () {
		curDialogShowSpeed = dialogShowSpeed;
		StopAllCoroutines ();
		myText.text = curText;
		isShowing = false;
		nextDisp.SetActive (true);

		foreach (string str in itemsToGive) {
			if (str != null)
				if (str != "")
					GiveItem (str);
		}
	}

	public bool canSkip = true;
	string RemoveCustomCommands (string _str) {
		canSkip = true;
		string[] myParts = _str.Split ('=');

		string myStr = "";
		if (myParts.Length > 1) {
			for (int i = 0; i+1 < myParts.Length; i ++) {
				bool shouldRemove = false;
				if (myParts[i].Contains ("delay"))
					shouldRemove = true;
				if (myParts[i].Contains ("wait"))
					shouldRemove = true;
				if (myParts[i].Contains ("give")) {
					shouldRemove = true;
					itemsToGive.Add (myParts[i + 1].Remove (myParts[i + 1].IndexOf ('>')));
				}
				if (myParts[i].Contains ("trigger")) {
					shouldRemove = true;
					//canSkip = false;
				}

				if (shouldRemove) {
					myParts[i] = myParts[i].Remove (myParts[i].LastIndexOf ('<'));
					myParts[i + 1] = myParts[i + 1].Substring (myParts[i+1].IndexOf ('>')+1);
				} else {
					myParts[i] += "=";
				}
			}
		}

		for (int i = 0; i < myParts.Length; i++) {
			myStr += myParts[i];
		}

		return myStr;
	}

	void GiveItem (string itemCode) {
		string[] id = itemCode.Split ('-');
		int myClass = -1;
		int myItem = -1;
		try {
			myClass = int.Parse (id[0]);
			myItem = int.Parse (id[1]);
		} catch {
			Debug.LogError ("Can't parse give: " + id[0] + " - " + id[1]);
		}
		switch (myClass) {
		case 0:
			/*InventoryMaster.s.Add (InventoryMaster.s.allEquipments[myItem], 1);*/
			break;
		case 1:
			/*InventoryMaster.s.Add (InventoryMaster.s.allIngredients[myItem], 1);*/
			break;
		case 2:
			/*InventoryMaster.s.Add (InventoryMaster.s.allPotions[myItem], 1);*/
			break;
		default:
			Debug.LogError ("Unknown item class " + id[0]);
			break;
		}
	}

	public void Clear () {
		nextDisp.SetActive (false);
		myText.text = "";
		isLocked = true;
		isShowing = false;
	}

	public delegate void Callback ();
	public Callback nextDialogCallback;
	public void NextDialog () {
		if (isShowing) {
			if (canSkip) {
				//FinishShowingText ();
				curDialogShowSpeed = 0f;
				skipCounter++;
				timeSinceLastSkip = 0;
			}
		} else {
			if (!isLocked) {
				isLocked = true;
				if (nextDialogCallback != null)
					nextDialogCallback.Invoke ();
			}
		}
	}


	public void PointerDown () {
		if (DialogTree.s.myAsset.isSkipable)
			Invoke ("SkipAllDialog", 1f);
	}

	public void PointerUp () {
		CancelInvoke ("SkipAllDialog");
	}

	void SkipAllDialog () {
		nextDialogCallback = null;
		Clear ();
		DialogTree.s.EndDialog ();
	}

	public void DialogEnd () {
		DisplayBigSprite (BigSpriteAction.Hide, BigSpriteSlots.left, null);
		DisplayBigSprite (BigSpriteAction.Hide, BigSpriteSlots.middle, null);
		DisplayBigSprite (BigSpriteAction.Hide, BigSpriteSlots.right, null);
	}
}
