using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGUI_BuySellMenu : MonoBehaviour {

    public GameObject ItemSelectionBoxPrefab;

    public Transform ItemSelectionParent;


    public Text shipCountText;
    public MiniGUI_ItemSelectionBox[] itemSelectors;
    public void SetUp(Item[] items, bool isBuy, int shipCount) {
        itemSelectors = new MiniGUI_ItemSelectionBox[items.Length];
        
        for (int i = 0; i < items.Length; i++) {
            itemSelectors[i] = Instantiate(ItemSelectionBoxPrefab, ItemSelectionParent).GetComponent<MiniGUI_ItemSelectionBox>();
            itemSelectors[i].SetUp(items[i], isBuy);
        }

        shipCountText.text = "Available Ships: " + shipCount.ToString();
    }

    public void OpenPanel() {
        gameObject.SetActive(true);
    }

    public void Request() {
        
    }

    public void ClosePanel() {
        gameObject.SetActive(false);
    }
}
