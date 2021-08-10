using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class GameQuitter {

	public static bool  didQuit = false;

	public static void QuitGame() {
		if (!didQuit) {
			DataSaver.s.SaveGame();
			didQuit = true;
		}
	}

}
