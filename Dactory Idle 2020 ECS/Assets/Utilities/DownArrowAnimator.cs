using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DownArrowAnimator : MonoBehaviour {
    
    private const string downArrow = "▼";

    private Text myText;
    // Start is called before the first frame update
    void Start() {
        myText = GetComponent<Text>();
    }


    private float timer = 0;
    public float animTime = 2f;
    public int arrowCount = 4;
    // Update is called once per frame
    void Update() {
        int curArrowCount = Mathf.FloorToInt((timer / animTime) * arrowCount);

        myText.text = "";
        for (int i = 0; i < curArrowCount; i++) {
            myText.text += ("\n" + downArrow);
        }

        timer += Time.deltaTime;
        if (timer > animTime) {
            timer = 0f;
        }
    }
}
