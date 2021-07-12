using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Controls the main game flow, with loading the game and starting various systems.
/// Ideally any long term process should start from here.
/// Also deals with quitting/saving the game
/// </summary>
public class GameMaster : MonoBehaviour {

	public static GameMaster s;
	public static bool loadingDone = false;
	public enum  GameState {
		planet, stars
	}

	public static GameState currentState = GameState.planet;

	private void Awake () {
		if (s != null) {
			Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
		}
		s = this;
		GameLoader.isGameLoadingDone = false;
	}

	private void OnDestroy() {
		s = null;
	}

	// Use this for initialization
	void Start () {
		StartPlaySession();
	}

	public void NewLocationInPlanet() {
		stopFactorySimulationEvent?.Invoke();
		clearPlanetEvent?.Invoke();

		DataSaver.s.mySave.beltData = new List<DataSaver.BeltSaveData>();
		DataSaver.s.mySave.connectorData = new List<DataSaver.ConnectorSaveData>();
		DataSaver.s.mySave.constructionData = new List<DataSaver.ConstructionSaveData>();
		DataSaver.s.mySave.buildingData = new List<DataSaver.BuildingSaveData>();
		DataSaver.s.mySave.droneData = new List<DataSaver.DroneSaveData>();
		
		var isGameLoadingSuccessful = GameLoader.isGameLoadingSuccessful;
		loadCompleteEventEarly?.Invoke(isGameLoadingSuccessful);
		loadCompleteEvent?.Invoke(isGameLoadingSuccessful);
		newPlanetEvent?.Invoke();
		
		startFactorySimulationEvent?.Invoke();
	}

	void StartPlaySession() {
		GameLoader.LoadGame();
		loadingDone = true;

		var isGameLoadingSuccessful = GameLoader.isGameLoadingSuccessful;

		loadCompleteEventEarly?.Invoke(isGameLoadingSuccessful);
		loadCompleteEvent?.Invoke(isGameLoadingSuccessful);
		if (!isGameLoadingSuccessful || (isGameLoadingSuccessful && !DataSaver.s.mySave.isSpaceshipLanded)) newPlanetEvent?.Invoke();

		startFactorySimulationEvent?.Invoke();
	}



	public static void StartSavingGameProcess() {
		if (loadingDone)
			DataSaver.s.SaveGame();
	}

	private void OnApplicationPause () {
		StartSavingGameProcess();
	}


	private void OnApplicationQuit () {
		StartSavingGameProcess();
	}

	public delegate void LoadingCompleteDelegate(bool isLoadSuccess);

	public static event LoadingCompleteDelegate loadCompleteEventEarly;
	public static event LoadingCompleteDelegate loadCompleteEvent;
	public static event GenericCallback startFactorySimulationEvent;
	public static event GenericCallback stopFactorySimulationEvent;
	public static event GenericCallback newPlanetEvent;
	public static event GenericCallback clearPlanetEvent;
	/// <summary>
	/// This must be called from "Awake"
	/// Remember to add the "OnDestroy" pair > RemoveFromCall
	/// </summary>
	public static void CallWhenLoadedEarly(LoadingCompleteDelegate callback) {
		loadCompleteEventEarly += callback;
	}

	/// <summary>
	/// This must be called from "Awake"
	/// Remember to add the "OnDestroy" pair > RemoveFromCall
	/// </summary>
	public static void CallWhenLoaded(LoadingCompleteDelegate callback) {
		loadCompleteEvent += callback;
	}

	/// <summary>
	/// This must be called from "Awake"
	/// Remember to add the "OnDestroy" pair > RemoveFromCall
	/// </summary>
	public static void CallWhenFactorySimulationStart(GenericCallback callback) {
		startFactorySimulationEvent += callback;
	}
	
	public static void CallWhenFactorySimulationStop(GenericCallback callback) {
		stopFactorySimulationEvent += callback;
	}

	/// <summary>
	/// This must be called from "Awake"
	/// Remember to add the "OnDestroy" pair > RemoveFromCall
	/// </summary>
	public static void CallWhenNewPlanet(GenericCallback callback) {
		newPlanetEvent += callback;
	}
	
	public static void CallWhenClearPlanet(GenericCallback callback) {
		clearPlanetEvent += callback;
	}

	/// <summary>
	/// This should always be called "OnDestroy" to make things work if you ever delete an object and/or reload a scene
	/// </summary>
	/// <param name="callback"></param>
	public static void RemoveFromCall(LoadingCompleteDelegate callback) {
		loadCompleteEventEarly -= callback;
		loadCompleteEvent -= callback;
	}

	/// <summary>
	/// This should always be called "OnDestroy" to make things work if you ever delete an object and/or reload a scene
	/// </summary>
	/// <param name="callback"></param>
	public static void RemoveFromCall(GenericCallback callback) {
		newPlanetEvent -= callback;
		startFactorySimulationEvent -= callback;
		stopFactorySimulationEvent -= callback;
		clearPlanetEvent -= callback;
	}
}
