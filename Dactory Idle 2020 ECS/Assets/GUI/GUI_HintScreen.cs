using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_HintScreen : MonoBehaviour {

    static GUI_HintScreen s;

    public GameObject gfxs;
    public Text description;
    private void Awake() {
        if (s != null) {
            Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
        }
        s = this;
    }


    private void Start() {
        if (PlayerPrefs.GetInt("FirstInfo", 0) == 1) {
            gfxs.SetActive(true);
            PlayerPrefs.SetInt("FirstInfo", 1);
        } else {
            gfxs.SetActive(false);
        }
    }


    public static void ShowHint(string hinttext) {
        s.description.text = hinttext;
        s.gfxs.SetActive(true);
    }

    public void HideHint() {
        gfxs.SetActive(false);
    }
}
