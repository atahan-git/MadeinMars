using Boo.Lang.Environments;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
	float deltaTime = 0.0f;

	Text myText;

	Queue<float> msec = new Queue<float>();
	Queue<float> fps = new Queue<float>();

	public int avgCount = 10;

	private void Start () {
		myText = GetComponent<Text>();
	}

	void Update () {
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
		msec.Enqueue(deltaTime * 1000.0f);
		fps.Enqueue(1.0f / deltaTime);
		string text = string.Format("{0:000.0} ms ({1:000.} fps)", msec.Average(), fps.Average());
		myText.text = text;

		if (msec.Count > avgCount) {
			msec.Dequeue();
			fps.Dequeue();
		}
	}
}
