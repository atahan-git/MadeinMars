using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSaverMono : MonoBehaviour
{
    public DataSaver myDataSaver;

    private void Awake () {
        // Unlike most of the "xxxMono" pattern scripts, we want to keep our copy of the datasaver alive across scenes
        // So that we don't lose our copy of the save file.
        if (DataSaver.s == null) {
            DataSaver.s = myDataSaver;
        } else {
            myDataSaver = DataSaver.s;
        }
    }

    private void Start() {
        print("Save Location:" + Application.persistentDataPath);
    }
}
