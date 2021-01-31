using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterDebug : MonoBehaviour {

    public static bool changeColorOnPlacedItemOnTop = false;
    public bool _changeColorOnPlacedItemOnTop = false;
    // Start is called before the first frame update
    void Start () {
        _changeColorOnPlacedItemOnTop = PlayerPrefs.GetInt("_changeColorOnPlacedItemOnTop", 0) == 1;
    }

    // Update is called once per frame
    void Update () {
        if (changeColorOnPlacedItemOnTop != _changeColorOnPlacedItemOnTop) {
            changeColorOnPlacedItemOnTop = _changeColorOnPlacedItemOnTop;
            PlayerPrefs.SetInt("_changeColorOnPlacedItemOnTop", _changeColorOnPlacedItemOnTop ? 1 : 0);
        }
    }
}
