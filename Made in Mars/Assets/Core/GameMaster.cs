using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;


/// <summary>
/// Controls the main game flow, with loading the game and starting various systems.
/// Ideally any long term process should start from here.
/// Also deals with quitting/saving the game
/// </summary>
public class GameMaster : MonoBehaviour {
	public static GameMaster s;
	public bool loadingDone = false;

	public enum  GameState {
		planet, stars
	}

	public static GameState currentState = GameState.planet;

	public void Awake () {
		if (s != null) {
			Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
		}
		s = this;
		GameLoader.isGameLoadingDone = false;
		GameQuitter.didQuit = false;
	}


	private void Start() {
		GameLoader.LoadGame();
		loadingDone = true;
		
		var isGameLoadingSuccessful = GameLoader.isGameLoadingSuccessful;

		loadCompleteEventEarly?.Invoke(isGameLoadingSuccessful);
		loadCompleteEvent?.Invoke(isGameLoadingSuccessful);
		
		if (currentState == GameState.planet) {
			StartPlanetSession();
		}
	}

	public void OnDestroy() {
		s = null;
	}
	

	public void NewLocationInPlanet() {
		ClearPlanetSession();

		var isGameLoadingSuccessful = GameLoader.isGameLoadingSuccessful;
		loadCompleteEventEarly?.Invoke(isGameLoadingSuccessful);
		loadCompleteEvent?.Invoke(isGameLoadingSuccessful);
		newPlanetEarlyEvent?.Invoke();
		newPlanetEvent?.Invoke();
		var currentPlanet = DataSaver.s.GetSave().currentPlanet.newPlanet = false;
		
		startFactorySimulationEvent?.Invoke();
	}

	public void LeavePlanet() {
		ClearPlanetSession();
		SceneChangeMaster.s.LoadStarsLevel();
	}

	private void ClearPlanetSession() {
		stopFactorySimulationEvent?.Invoke();
		clearPlanetEvent?.Invoke();

		var save = DataSaver.s.GetSave();
		save.currentPlanet = new DataSaver.LocalPlanetData(save.currentPlanet.planetData);
	}

	public void StartPlanetSession() {
		var isGameLoadingSuccessful = GameLoader.isGameLoadingSuccessful;
		var currentPlanet = DataSaver.s.GetSave().currentPlanet;
		if (currentPlanet.newPlanet) {
			newPlanetEarlyEvent?.Invoke();
			newPlanetEvent?.Invoke();
			currentPlanet.newPlanet = false;
		}
		startFactorySimulationEvent?.Invoke();
	}
	
	
	private void OnDisable() {
		GameQuitter.QuitGame();
	}


	private void OnApplicationPause () {
		if(loadingDone)
			GameQuitter.QuitGame();
	}

	

	private void OnApplicationQuit () {
		GameQuitter.QuitGame();
	}


	public void TriggerPlayerInventoryChangedEvent() {
		playerInventoryChangedEvent?.Invoke();
	}

	public delegate void LoadingCompleteDelegate(bool isLoadSuccess);

	public static event LoadingCompleteDelegate loadCompleteEventEarly;
	public static event LoadingCompleteDelegate loadCompleteEvent;
	public static event GenericCallback startFactorySimulationEvent;
	public static event GenericCallback stopFactorySimulationEvent;
	
	public static event GenericCallback newPlanetEarlyEvent;
	public static event GenericCallback newPlanetEvent;
	public static event GenericCallback clearPlanetEvent;
	public static event GenericCallback playerInventoryChangedEvent;
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
	
	public static void CallWhenNewPlanetEarly(GenericCallback callback) {
		newPlanetEarlyEvent += callback;
	}
	
	public static void CallWhenClearPlanet(GenericCallback callback) {
		clearPlanetEvent += callback;
	}
	
	
	/// <summary>
	/// This must be called from "Awake"
	/// Remember to add the "OnDestroy" pair > RemoveFromCall
	/// </summary>
	public static void CallWhenPlayerInventoryChanged(GenericCallback callback) {
		playerInventoryChangedEvent += callback;
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
		playerInventoryChangedEvent -= callback;
		newPlanetEarlyEvent -= callback;
	}
}
