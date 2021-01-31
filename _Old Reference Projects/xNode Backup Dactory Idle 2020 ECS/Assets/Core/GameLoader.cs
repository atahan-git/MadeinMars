using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour {

    public static bool isGameLoadingDone = false;
    public static bool isGameLoadingSuccessfull = false;

    public delegate void LoadYourself();

    static event LoadYourself loadCompleteEventEarly;
    static event LoadYourself loadCompleteEvent;

    public void LoadGame() {
        if (DataSaver.s.Load()) {
            isGameLoadingSuccessfull = true;
        } else {
            isGameLoadingSuccessfull = false;
        }

        loadCompleteEventEarly?.Invoke();
            loadCompleteEvent?.Invoke();
        

        isGameLoadingDone = true;
    }
    
    /// <summary>
    /// This must be called from "Awake"
    /// </summary>
    public static void CallWhenLoaded(LoadYourself callback, int order) {
        loadCompleteEventEarly += callback;
    }

    /// <summary>
    /// This must be called from "Awake"
    /// </summary>
    public static void CallWhenLoaded(LoadYourself callback) {
            loadCompleteEvent += callback;
    }

    public static void RemoveFromCall(LoadYourself callback) {
        loadCompleteEventEarly -= callback;
            loadCompleteEvent -= callback;
    }
}
