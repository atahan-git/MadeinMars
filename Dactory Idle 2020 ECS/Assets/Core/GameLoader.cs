using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour
{

    public static bool isGameLoadingDone = false;
    public static bool isGameLoadingSuccessfull = false;
    public delegate void LoadYourself ();
    static event LoadYourself loadCompleteEvent;

    public void LoadGame () {
        if (DataSaver.s.Load()) {
            isGameLoadingSuccessfull = true;
        } else {
            isGameLoadingSuccessfull = false;
        }

        loadCompleteEvent?.Invoke();
        isGameLoadingDone = true;
    }

    public static void CallWhenLoaded (LoadYourself callback) {
        if (isGameLoadingDone)
            callback();
        else
            loadCompleteEvent += callback;
    }

    public static void RemoveFromCall (LoadYourself callback) {
        loadCompleteEvent -= callback;
    }
}
