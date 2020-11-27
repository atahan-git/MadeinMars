using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodeFocusCatcher : MonoBehaviour, IPointerClickHandler {
	private RecipeTreeViewer myMaster;

	public void Setup(RecipeTreeViewer master) {
		myMaster = master;
	}

	public void OnPointerClick(PointerEventData data) {
		myMaster.FocusOnNode(gameObject);
	}
}
