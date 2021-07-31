using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGUI_CardPanel : MonoBehaviour {

    public GameObject cardPrefab;

    public List<ShipCard> activeCards = new List<ShipCard>();

    public Transform ParentTransform;
    public void RedrawListOfCards(List<ShipCard> cardsToDraw) {
        activeCards = cardsToDraw;
        
        ParentTransform.DeleteAllChildren();

        for (int i = 0; i < cardsToDraw.Count; i++) {
            var card = Instantiate(cardPrefab, ParentTransform);
            card.GetComponent<MiniGUI_Card>().SetUp(cardsToDraw[i]);
        }
    }
}
