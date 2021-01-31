using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltBugShowcaser : MonoBehaviour
{


    public GameObject[] beltSlots;
    public GameObject[] items;

    public float fasterSpeed = 0.1f;
    public float updateSpeed = 0.2f;
    public float updateSpeedLong = 0.7f;
    public GameObject updaterBox;


    public Material activeItem;
    public Material nonActiveItem;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = beltSlots.Length - 1; i >= 0; i--) {
            if (items[i] != null) {
                items[i].GetComponent<Renderer>().material = activeItem;
                items[i].GetComponent<SlowlyGoToPos>().target = beltSlots[i];
            }
            }
        StartCoroutine(UpdateBelts());
    }

    IEnumerator UpdateBelts () {
        yield return new WaitForSeconds(1f);

        while (true) {
            for (int i = beltSlots.Length - 1; i >= 0; i--) {
                updaterBox.transform.position = beltSlots[i].transform.position;

                int nextIndex = i + 1;
                if (nextIndex == 9)
                    nextIndex = 1;
                if (items[nextIndex] == null && items[i] != null) {
                    if (items[i].GetComponent<SlowlyGoToPos>().isProcessed == false) {
                        items[nextIndex] = items[i];
                        items[i] = null;
                        items[nextIndex].GetComponent<SlowlyGoToPos>().target = beltSlots[nextIndex];
                        items[nextIndex].GetComponent<SlowlyGoToPos>().isProcessed = true;
                        items[nextIndex].GetComponent<Renderer>().material = nonActiveItem;
                    }
                } else if (items[i] != null) {
                    items[i].GetComponent<SlowlyGoToPos>().isMarked = true;
                }

                yield return new WaitForSeconds(updateSpeed);
            }

            yield return new WaitForSeconds(updateSpeedLong/2f);

            for (int i = beltSlots.Length - 1; i >= 0; i--) {
                updaterBox.transform.position = beltSlots[i].transform.position;

                int nextIndex = i + 1;
                if (nextIndex == 9)
                    nextIndex = 1;
                if (items[nextIndex] == null && items[i] != null) {
                    if (items[i].GetComponent<SlowlyGoToPos>().isMarked) {
                        items[nextIndex] = items[i];
                        items[i] = null;
                        items[nextIndex].GetComponent<SlowlyGoToPos>().target = beltSlots[nextIndex];
                        items[nextIndex].GetComponent<SlowlyGoToPos>().isProcessed = true;
                        items[nextIndex].GetComponent<Renderer>().material = nonActiveItem;
                    }
                } 

                yield return new WaitForSeconds(updateSpeed);
            }


            yield return new WaitForSeconds(updateSpeedLong / 2f);

            for (int i = 0; i < beltSlots.Length; i++) {
                updaterBox.transform.position = beltSlots[i].transform.position;
                if (items[i] != null) {
                    items[i].GetComponent<Renderer>().material = activeItem;
                    items[i].GetComponent<SlowlyGoToPos>().isProcessed = false;
                    items[i].GetComponent<SlowlyGoToPos>().isMarked = false;
                }
                yield return new WaitForSeconds(fasterSpeed);
            }

            yield return new WaitForSeconds(updateSpeedLong);
        }
    }
}
