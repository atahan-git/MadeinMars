using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class GameLoader  {

    public static bool isGameLoadingDone = false;
    public static bool isGameLoadingSuccessful = false;

    public static void LoadGame() {
        if (DataSaver.s.Load()) {
            isGameLoadingSuccessful = true;
        } else {
            isGameLoadingSuccessful = false;
        }

        isGameLoadingDone = true;
    }
}
