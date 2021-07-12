using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Deals with starting up the game. Runs all the starting functionality, in the wanted order.
/// </summary>
public class GameLoader : MonoBehaviour {

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
