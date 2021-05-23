using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Dialog : MonoBehaviour {

	public DialogTree myMaster;

	public string myTag;

	/*public AudioClip soundFile;	
	AudioSource source;	//If left null will check master until finds one and uses it instead*/

	[TextArea]
	public string text;

	[Space]

	public bool clearImage = false;
	public Sprite image;

	[Space]

	public DialogDisplayer.BigSpriteAction[] bigSpriteAction;
	[Tooltip("Change 'Big Sprite Action' if you want to add or remove elements")]
	public DialogDisplayer.BigSpriteSlots[] bigSpriteSlot;
	[Tooltip ("Change 'Big Sprite Action' if you want to add or remove elements")]
	public Sprite[] bigSprite;

	[Space]

	/*[Header("Go for 0.5s offset and 4w/s for normal talking")]
	public float duration = 2f;*/
	public float delay = 0.3f;

	/*[System.Serializable]
	public class MyEventType : UnityEngine.Events.UnityEvent {}
	public MyEventType callWhenBegin;
	public MyEventType callWhenDone;*/

	public Dialog NextInChain;
	public bool breakAutoChain = false;

	bool isStarted = false;

	//bool audioEnabled = true;
	bool textEnabled = true;
	//string optionalPlayerprefSubtitleDisableOption = "Subtitles"; //set this value 1 or 0 based on setting if you want to

	// Use this for initialization
	void Start () {
		//Check audio system values
		/*if (soundFile != null) {
			if (source == null) {
				source = myMaster.mySource;
				if (source == null) {
					source = myMaster.myDialogMaster.mySource;
				}
				if (source == null) {
					audioEnabled = false;
				}
			}
		} else {
			audioEnabled = false;
		}*/
		//print (audioEnabled);

		//Check text system values
		/*if (!(text == "" || text == " ")) {
			if (displayArea.Length < 1) {
				displayArea = myMaster.myDisplayArea;
				if (displayArea.Length < 1) {
					displayArea = myMaster.myDialogMaster.myDisplayArea;
				}
				if (displayArea.Length < 1) {
					textEnabled = false;
				}
			}
		} else {
			textEnabled = false;
		}*/
		//print (textEnabled);
	}

	
	// Update is called once per frame
	public void StartDialog () {
		if (!isStarted) {
			if (textEnabled)
				//Invoke ("SetText", offset);
				SetText ();
			/*if (audioEnabled)
				Invoke ("SetAudio", offset);*/
		}
		isStarted = true;

		DialogDisplayer.s.nextDialogCallback += EndDialog;
		//callWhenBegin.Invoke ();
	}

	void SetText () {
		DialogDisplayer.s.SetDialog (image, text, delay, clearImage);
		for (int i = 0; i < bigSpriteAction.Length; i++) {
			DialogDisplayer.s.DisplayBigSprite (
				bigSpriteAction[i],
				bigSpriteSlot[i < bigSpriteSlot.Length ? i : bigSpriteSlot.Length - 1],
				bigSprite[i < bigSprite.Length ? i : bigSprite.Length - 1]);
		}
		//Invoke ("UnSetText", duration);
	}

	void UnSetText () {
		DialogDisplayer.s.Clear ();
		//EndDialog ();
	}

	/*void SetAudio () {
		source.clip = soundFile;
		source.Play ();
	}*/

	public void EndDialog () {
		isStarted = false;
		UnSetText ();
		DialogDisplayer.s.nextDialogCallback -= EndDialog;
		if (NextInChain != null && !breakAutoChain) {
			NextInChain.StartDialog ();
		} else {
			myMaster.EndDialog ();
		}
		/*if (callWhenDone != null) {
			callWhenDone.Invoke ();
		}*/
	}
}
