using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedText : MonoBehaviour {

	public string[] states;

	public int framesPerSecond = 6;
	float waitSeconds {
		get {
			return 1f / framesPerSecond;
		}
	}

	public bool isPlaying = true;

	[SerializeField]
	float index;

	Text myText;

	// Use this for initialization
	void Start () {
		myText = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (isPlaying) {
			if (myText != null) {
				index += Time.deltaTime / waitSeconds;
				if (index >= states.Length) {
					index = 0;
				}
				myText.text = states[(int)index];
			}
		}
	}
}
