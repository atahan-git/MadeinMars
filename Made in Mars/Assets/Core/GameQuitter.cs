using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class GameQuitter 
{
	public static void QuitGame() {
		if (GameLoader.isGameLoadingSuccessful)
			DataSaver.s.SaveGame();
	}
	
}
