using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShipCardWorldObject : MonoBehaviour {
    public UnityEvent OnNewCard; // Attach a minigui_card to this even in unity to correctly display the card.
    public ShipCard myCard;

    public GameObject destinationReachedEffectPrefab;
    
    public void SetUp(ShipCard myCard, Position location) {
        this.myCard = myCard;
        transform.position = location.Vector3(Position.Type.shipCard);
        OnNewCard?.Invoke();

        StartCoroutine(FlyToShipAndDisappear());
    }


    
    IEnumerator FlyToShipAndDisappear() {
        var speed = 0f;
        var acceleration = 10f;

        while (Vector3.Distance(transform.position, GetShipPos()) > 0.01f) {
            transform.position = Vector3.MoveTowards(transform.position, GetShipPos(), speed*Time.deltaTime);
            speed += acceleration * Time.deltaTime;

            yield return null;
        }

        Instantiate(destinationReachedEffectPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    Vector3 GetShipPos() {
        return FactoryMaster.s.GetShip().center.Vector3(transform.position.z);
    }
}
