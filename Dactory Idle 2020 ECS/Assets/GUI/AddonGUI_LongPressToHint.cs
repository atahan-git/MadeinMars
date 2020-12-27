using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AddonGUI_LongPressToHint : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {


	[TextArea] public string infoText;

	private bool isPressing = false;

	public void OnPointerDown(PointerEventData eventData) {
		if (!isPressing) {
			isPressing = true;
			Invoke("DisplayHint", 0.5f);
		}
	}


	public void OnPointerUp(PointerEventData eventData) {
		CancelInvoke();
		isPressing = false;
	}


	void DisplayHint() {
		GUI_HintScreen.ShowHint(infoText);
	}
}
