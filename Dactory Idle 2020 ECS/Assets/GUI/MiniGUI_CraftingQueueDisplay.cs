using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// A helper class for the individual Crafting queue's in the crafting ui panel
/// </summary>
public class MiniGUI_CraftingQueueDisplay : MonoBehaviour
{
    public float progress = 0;
    public float timeReq = -1f;
    public int index = -1;
    public Item myItem;
    public CraftingNode myCraftingNode;

    public Image myImg;

    public Slider progressSlider;
    public Slider deleteSlider;

    GUI_CraftingController cont;

    public void SetUp(CraftingNode _cp, GUI_CraftingController c) {
        myCraftingNode = _cp;
        progress = 0;
        timeReq = myCraftingNode.timeCost;
        myItem =  DataHolder.s.GetItem(myCraftingNode.outputs[0].itemUniqueName);
        myImg.sprite = myItem.GetSprite();
        cont = c;
    }

    public void UpdateDisplay() {
        GetComponentInChildren<Slider>().value = progress;
    }

    public void PointerDownCancel() {
        isDeleting = true;
        StartCoroutine(DeleteSliderGo());
    }

    private bool isDeleting = false;

    IEnumerator DeleteSliderGo() {
        while (isDeleting) { 
            
            deleteSlider.value = Mathf.MoveTowards(deleteSlider.value, 1f, 2f * Time.deltaTime);
            
            if (deleteSlider.value >= 1) {
                CancelCrafting();
                break;
            }

            yield return 0;
        }
        
        if (!isDeleting) {
            deleteSlider.value = 0;
        }
    }

    public void PointerUpCancel() {
        isDeleting = false;
    }
    
    void CancelCrafting() {
        cont.CancelCraftItem(this);
    }

    public void DestroySelf() {
        /*var allImgs = GetComponentsInChildren<Image>();
        foreach (var img in allImgs) {
            img.color = new Color(0,0,0,0);
        }*/
        if (isActiveAndEnabled)
            StartCoroutine(ThinnerToDestroy());
        else
            Destroy(gameObject);

    }

    IEnumerator ThinnerToDestroy() {
        var rectTrans = GetComponent<RectTransform>();
        while (true) {
            rectTrans.sizeDelta = Vector2.Lerp(rectTrans.sizeDelta,Vector2.zero, 10f*Time.deltaTime);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTrans.parent.GetComponent<RectTransform>());

            if (rectTrans.sizeDelta.x < 0.1f) {
                Destroy(gameObject);
                break;
            }

            yield return 0;
        }
    }
}
