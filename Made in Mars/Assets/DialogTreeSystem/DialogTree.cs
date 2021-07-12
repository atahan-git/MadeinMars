using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;

[ExecuteInEditMode]
public class DialogTree : MonoBehaviour {

	public static DialogTree s;

	public string dialogName = "New Dialog";

	public Dialog[] dialogs = new Dialog[0];

	public GameObject dialogPrefab;

	[HideInInspector]
	public bool updateAssetRealtime = false;
	public DialogTreeAsset myAsset;
	DialogTreeAsset lastAsset;

	[System.Serializable]
	public class MyEventType : UnityEngine.Events.UnityEvent { }
	public MyEventType callWhenBegin;
	public MyEventType callWhenDone;

	public delegate void CustomTriggerDelegate ();
	public CustomTriggerDelegate[] myCustomTriggers = new CustomTriggerDelegate[5];
	public bool[] myCustomWaits = new bool[5];

	// Use this for initialization
	void Awake () {
		s = this;
		/*if(!Application.isEditor)
		if (dialogs.Length > 0)
			dialogs [dialogs.Length - 1].callWhenDone.AddListener (EndDialog);*/
	}

	private void OnDestroy() {
		s = null;
	}

	// Update is called once per frame
	public void Update () {
#if UNITY_EDITOR
		if (Application.isEditor && !Application.isPlaying) {
			dialogs = GetComponentsInChildren<Dialog> ();

			gameObject.name = "-"+ dialogName + "- Dialog";

			int n = 0;
			foreach (Dialog myPoint in dialogs) {
				//PrefabUtility.UnpackPrefabInstance (myPoint.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
				if (!(myPoint.myTag == "" || myPoint.myTag == " ")) {
					myPoint.gameObject.name = "Dialog " + n + " - " + myPoint.myTag;
				} else {
					myPoint.gameObject.name = "Dialog " + n + " - " + myPoint.text;
				}
				n++;
				myPoint.myMaster = this;
				if (n < dialogs.Length) {
					myPoint.NextInChain = dialogs [n];
				} else {
					myPoint.NextInChain = null;
					//print ("Added listener");
				}

				int min = Mathf.Min (myPoint.bigSpriteAction.Length, myPoint.bigSpriteSlot.Length, myPoint.bigSprite.Length);
				int max = Mathf.Max (myPoint.bigSpriteAction.Length, myPoint.bigSpriteSlot.Length, myPoint.bigSprite.Length);
				if (min != max) {
					int size = myPoint.bigSpriteAction.Length;
					SetArraySize (ref myPoint.bigSpriteSlot, size);
					SetArraySize (ref myPoint.bigSprite, size);
				}
			}

			if (lastAsset != myAsset) {
				lastAsset = myAsset;
				updateAssetRealtime = false;
			}
			
			if(updateAssetRealtime)
				UpdateMyAsset ();
		}
#endif
	}

	void SetArraySize<T> (ref T[] array, int size) {
		T[] temp = new T[array.Length];
		array.CopyTo (temp,0);

		array = new T[size];
		for (int i = 0; i < array.Length; i++) {
			if(i < temp.Length)
				array[i] = temp[i];
		}
	}

	void UpdateMyAsset () {
		return;
		if (myAsset != null) {
			myAsset.name = dialogName;
			myAsset.dialogs = new DialogObject[dialogs.Length];

			for (int i = 0; i < dialogs.Length; i++) {
				if (dialogs[i] != null) {
					myAsset.dialogs[i] = new DialogObject {
						tag = dialogs[i].myTag,
						text = dialogs[i].text,
						clearImage = dialogs[i].clearImage,
						image = dialogs[i].image,
						bigSpriteAction = dialogs[i].bigSpriteAction,
						bigSpriteSlot = dialogs[i].bigSpriteSlot,
						bigSprite = dialogs[i].bigSprite,
						delay = dialogs[i].delay,
						breakAutoChain = dialogs[i].breakAutoChain
					};


				}
			}
		}
	}

	public void TriggerCommand (string command) {
		try {
			int id = int.Parse (command);
			if (myCustomTriggers[id] != null) {
				myCustomTriggers[id].Invoke ();
			} else {
				Debug.LogError ("Trying to trigger empty command: " + command);
			}
		} catch (System.Exception e) {
			Debug.LogError ("Problem triggering command: " + command +  e);
		}
	}

	public void StartDialog () {
		print ("Dialog -" + gameObject.name + "- started");
		dialogs [0].StartDialog ();
		DialogDisplayer.s.SetDialogScreenState (true);

		callWhenBegin.Invoke ();

	}

	public void EndDialog () {
		print ("Dialog -" + gameObject.name + "- ended");
		DialogDisplayer.s.SetDialogScreenState (false);
		//print ("note that this doesnt actually stop the dialog, this is just a reminder that the dialogue ended");
		callWhenDone.Invoke ();
	}

	public void LoadFromAsset (DialogTreeAsset asset) {
		updateAssetRealtime = false;
		if (asset == null)
			return;

		myAsset = asset;
		dialogName = myAsset.name;
		gameObject.name = "-" + dialogName + "- Dialog";

		Dialog[] myChild = GetComponentsInChildren<Dialog> ();
		for (int i = 0; i < myChild.Length; i++) {
			myChild[i].StopAllCoroutines ();
			DestroyImmediate (myChild[i].gameObject);
		}
		DialogDisplayer.s.StopAllCoroutines ();
		DialogDisplayer.s.Clear ();

		foreach (DialogObject dia in myAsset.dialogs) {
			GameObject myPoint = (GameObject)Instantiate (dialogPrefab, transform.position, transform.rotation);
			myPoint.transform.parent = transform;
			Dialog myDia = myPoint.GetComponent<Dialog> ();
			myDia.myTag = dia.tag;
			myDia.text = dia.text;
			myDia.clearImage = dia.clearImage;
			myDia.image = dia.image;
			myDia.bigSpriteAction = dia.bigSpriteAction;
			myDia.bigSpriteSlot = dia.bigSpriteSlot;
			myDia.bigSprite = dia.bigSprite;
			myDia.delay = dia.delay;
			myDia.breakAutoChain = dia.breakAutoChain;
		}


		dialogs = GetComponentsInChildren<Dialog> ();


		int n = 0;
		foreach (Dialog myPoint in dialogs) {
			if (!(myPoint.myTag == "" || myPoint.myTag == " ")) {
				myPoint.gameObject.name = "Dialog " + n + " - " + myPoint.myTag;
			} else {
				myPoint.gameObject.name = "Dialog " + n + " - " + myPoint.text;
			}
			n++;
			myPoint.myMaster = this;
			if (n < dialogs.Length) {
				myPoint.NextInChain = dialogs[n];
			} else {
				myPoint.NextInChain = null;
				//print ("Added listener");
			}
		}
	}
}
