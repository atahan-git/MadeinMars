using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[System.Serializable]
public class PointerEvent : UnityEvent<PointerEventData> { }

public class MiniGUI_CardDropMe : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
    public Image containerImage;
    public Image receivingImage;
    private Color normalColor;
    public Color highlightColor = Color.yellow;

    public PointerEvent OnDropEvent;

    public void OnEnable() {
        if (containerImage != null)
            normalColor = containerImage.color;
    }

    public void OnDrop(PointerEventData data) {
        containerImage.color = normalColor;

        
        OnDropEvent?.Invoke(data);


        if (receivingImage == null)
            return;

        Sprite dropSprite = GetDropSprite(data);
        if (dropSprite != null)
            receivingImage.overrideSprite = dropSprite;
    }

    public void OnPointerEnter(PointerEventData data) {
        if (containerImage == null)
            return;

        Sprite dropSprite = GetDropSprite(data);
        if (dropSprite != null)
            containerImage.color = highlightColor;
    }

    public void OnPointerExit(PointerEventData data) {
        if (containerImage == null)
            return;

        containerImage.color = normalColor;
    }

    private Sprite GetDropSprite(PointerEventData data) {
        var originalObj = data.pointerDrag;
        if (originalObj == null)
            return null;

        var dragMe = originalObj.GetComponent<MiniGUI_CardDragMe>();
        if (dragMe == null)
            return null;

        var srcImage = originalObj.GetComponent<Image>();
        if (srcImage == null)
            return null;

        return srcImage.sprite;
    }
}