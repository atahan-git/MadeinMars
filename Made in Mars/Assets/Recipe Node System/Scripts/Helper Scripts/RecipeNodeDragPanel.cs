using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;


/// <summary>
/// A helper script that add the draggable functionality to the nodes.
/// </summary>
public class RecipeNodeDragPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {
	
	private Vector2 originalLocalPointerPosition;
	private Vector3 originalPanelLocalPosition;
	private RectTransform panelRectTransform;
	private RectTransform parentRectTransform;

	private NodeGfx myNode;
	
	void Awake () {
		panelRectTransform = transform as RectTransform;
		parentRectTransform = panelRectTransform.parent as RectTransform;
		myNode = GetComponent<NodeGfx>();
	}
	
	public void OnPointerDown (PointerEventData data) {
		originalPanelLocalPosition = panelRectTransform.localPosition;
		RectTransformUtility.ScreenPointToLocalPointInRectangle (parentRectTransform, data.position, data.pressEventCamera, out originalLocalPointerPosition);
	}
	
	public void OnDrag (PointerEventData data) {
		if (panelRectTransform == null || parentRectTransform == null)
			return;
		
		Vector2 localPointerPosition;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle (parentRectTransform, data.position, data.pressEventCamera, out localPointerPosition)) {
			Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;
			panelRectTransform.localPosition = SnapToGrid(originalPanelLocalPosition + offsetToOriginal,NodeGfx.snapMultUI);
		}

		if (data.position.y < NodeItemTreeMakerMaster.s.DeadZone) {
			myNode.ShowDeleteColors(true);
		} else {
			myNode.ShowDeleteColors(false);
		}

		//print(data.position.y.ToString() + " - " + NodeItemTreeMakerMaster.s.DeadZone.ToString());

		myNode.OnDraggingNode();
		//ClampToWindow ();
	}

	Vector3 SnapToGrid(Vector3 input, float gridSize) {
		//print("snapping");
		return new Vector3(SnapToGrid(input.x,gridSize),SnapToGrid(input.y,gridSize),SnapToGrid(input.z,gridSize));
	}

	float SnapToGrid(float input, float gridSize) {
		//print(input.ToString() +" - " + gridSize.ToString() + " - " + ((Mathf.RoundToInt(input * gridSize))/gridSize).ToString());
		
		return (Mathf.RoundToInt(input * gridSize))/gridSize;
	}

	public void OnPointerUp(PointerEventData eventData) {
		if (eventData.position.y < NodeItemTreeMakerMaster.s.DeadZone) {
			myNode.DeleteNode();
		}
		myNode.PositionUpdated();
	}
	
	// Clamp panel to area of parent
	void ClampToWindow () {
		Vector3 pos = panelRectTransform.localPosition;
		
		Vector3 minPosition = parentRectTransform.rect.min - panelRectTransform.rect.min;
		Vector3 maxPosition = parentRectTransform.rect.max - panelRectTransform.rect.max;
		
		pos.x = Mathf.Clamp (panelRectTransform.localPosition.x, minPosition.x, maxPosition.x);
		pos.y = Mathf.Clamp (panelRectTransform.localPosition.y, minPosition.y, maxPosition.y);

		
		panelRectTransform.localPosition = pos;
	}
}
