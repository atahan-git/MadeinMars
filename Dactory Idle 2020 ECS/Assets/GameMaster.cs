using UnityEngine;
using System.Collections;

public class GameMaster : MonoBehaviour {

	public bool loadingDone = false;

	private void Awake () {
		GameLoader.isGameLoadingDone = false;
	}

	// Use this for initialization
	void Start () {
		GetComponent<GameLoader>().LoadGame();
		loadingDone = true;
		BeltMaster.s.StartBeltSystem();
		BuildingMaster.s.StartBuildingSystem();
	}

	private void OnApplicationPause () {
		if (loadingDone)
			DataSaver.s.SaveGame();
	}


	private void OnApplicationQuit () {
		if (loadingDone)
			DataSaver.s.SaveGame();
	}

}
