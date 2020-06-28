using UnityEngine;
using System.Collections;

public class GameMaster : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<GameLoader>().LoadGame();
		BeltMaster.s.StartBeltSystem();
		BuildingMaster.s.StartBuildingSystem();
	}

	private void OnApplicationPause () {
		DataSaver.s.SaveGame();
	}

}
