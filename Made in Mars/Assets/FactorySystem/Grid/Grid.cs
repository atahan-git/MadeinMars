using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Holds the game's grid data These grids are more lightweight than the actual tiles underneath. The graphics are controlled by the build in "Grid"
/// Handles all the access to the tiles/getting data out operations, and works as an abstraction. Edit this to add chunking functionality
/// </summary>
[Serializable]
public class Grid 
{
	public static Grid s;
	public MapGenerator mapGen;

	public int mapSize = 200;

	public TileData[,] myTiles;

	public PlanetSchematic scheme;

	private TileData dummyTile = new TileData(-1,-1);
	
	public void RegisterEvents ( MapGenerator _mapGen) {
		mapGen = _mapGen;

		DataSaver.s.saveEvent += SaveTiles;
		GameMaster.CallWhenLoadedEarly(GameLoadingComplete);
		GameMaster.CallWhenNewPlanet(GenerateTiles);
	}

	public void DummySetup(int _mapSize) {
		mapSize = _mapSize;
		myTiles = new TileData[mapSize, mapSize];
		for (int x = 0; x < mapSize; x++) {
			for (int y = 0; y < mapSize; y++) {
				myTiles[x, y] = new TileData(x, y);
			}
		}
	}


	public TileData GetTile(Position pos) {
		if (pos.x < 0 || pos.y < 0 || pos.x > mapSize || pos.y > mapSize) {
			return dummyTile;
		}
		return myTiles[pos.x, pos.y];
	}

	void GameLoadingComplete (bool isSucces) {
		if (isSucces) {
			LoadTiles();
		} else {
			// We only generate new tiles when we see the "new planet" event so that we dont generate tiles twice when the loading fails
			//GenerateTiles();
		}
	}


	Camera mycam;
	public TileData GetTileUnderPointer () {
		if (mycam == null) {
			mycam = Camera.main;
		}

		Vector3 pos = mycam.ScreenToWorldPoint(SmartInput.inputPos);
		int x = (int)pos.x;
		int y = (int)pos.y;
		if (x >= 0 && x < myTiles.GetLength(0) && y >= 0 && y < myTiles.GetLength(1)) {
			return myTiles[x, y];
		} else {
			return null;
		}
	}

	public void LoadTiles() {
		mapGen.Clear();
		bool loadDataExists = DataSaver.s.mySave.tileData != null;
		if (loadDataExists) {
			if (DataSaver.s.mySave.tileData != null && DataSaver.s.mySave.tileData.Length > 0) {
				int xLength = DataSaver.s.mySave.tileData.GetLength(0);
				int yLength = DataSaver.s.mySave.tileData.GetLength(0);
				int[,] materials = new int [xLength, yLength];
				int[,] height = new int[xLength, yLength];
				int[][,] oreAmounts = new int[DataHolder.s.GetAllOres().Length][,];
				for (int i = 0; i < DataHolder.s.GetAllOres().Length; i++) {
					oreAmounts[i] = new int[xLength, yLength];
				}

				myTiles = new TileData[xLength, yLength];
				for (int x = 0; x < DataSaver.s.mySave.tileData.GetLength(0); x++) {
					for (int y = 0; y < DataSaver.s.mySave.tileData.GetLength(1); y++) {
						DataSaver.TileData dat = DataSaver.s.mySave.tileData[x, y];
						myTiles[x, y] = new TileData(dat, x, y);
						materials[x, y] = dat.material;
						height[x, y] = dat.height;
						if (dat.oreType > 0 && dat.oreType <= oreAmounts.GetLength(0))
							oreAmounts[dat.oreType - 1][x, y] = Mathf.CeilToInt(dat.oreAmount/10000f);
					}
				}

				mapGen.LoadMap(materials, height, scheme);
				for (int i = 0; i < DataHolder.s.GetAllOres().Length; i++) {
					mapGen.LoadResources(oreAmounts[i], DataHolder.s.GetAllOres()[i]);
				}

				Debug.Log("Map successfully loaded");
			} else {
				GenerateTiles();
			}
		}
	}

	public DataLogger _logger;
	public void GenerateTiles () {
		var time = 0f;
		time = Time.realtimeSinceStartup;
		
		mapGen.Clear();
		int[,] materials;
		int[,] height;
		mapGen.mapSize = new Vector2Int(mapSize, mapSize);
		var map = mapGen.GenerateMap(scheme);
		materials = map.Item1;
		height = map.Item2;

		int[,] oreType = new int[mapSize,mapSize];
		int[,] oreAmount = new int[mapSize, mapSize];
		int[][,] oreAmountsPrepass = new int[DataHolder.s.GetAllOres().Length][,];
		for (int i = 0; i < DataHolder.s.GetAllOres().Length; i++) {
			oreAmountsPrepass[i] = mapGen.GenerateResources(DataHolder.s.GetAllOres()[i]);
		}

		myTiles = new TileData[mapSize, mapSize];

		for (int x = 0; x < materials.GetLength(0); x++) {
			for (int y = 0; y < materials.GetLength(1); y++) {
				for (int i = 0; i < oreAmountsPrepass.Length; i++) {
					if (oreAmountsPrepass[i][x, y] > 0) {
						oreType[x, y] = i+1;
						oreAmount[x, y] = oreAmountsPrepass[i][x, y] * 10000;
						break;
					}
				}


				myTiles[x, y] = new TileData(x,y);
				myTiles[x, y].material = materials[x, y];
				myTiles[x, y].height = height[x, y];
				myTiles[x, y].oreType = oreType[x,y];
				myTiles[x, y].oreAmount = oreAmount[x, y]; 
			}
		}
		
		_logger.mapGenerationTime.Add(Time.realtimeSinceStartup - time);
	}

	public void SaveTiles () {
		DataSaver.s.mySave.tileData = new DataSaver.TileData[myTiles.GetLength(0), myTiles.GetLength(1)];
		for (int x = 0; x < myTiles.GetLength(0); x++) {
			for (int y = 0; y < myTiles.GetLength(1); y++) {
				TileData ct = myTiles[x, y];
				DataSaver.s.mySave.tileData[x, y] = new DataSaver.TileData(ct.height, ct.material, ct.oreType, ct.oreAmount);
			}
		}
	}

	public void OnDestroy () {
		s = null;
		DataSaver.s.saveEvent -= SaveTiles;
		GameMaster.RemoveFromCall(GameLoadingComplete);
	}
}



[System.Serializable]
public class TileData {
	public string name { get { return location.ToString() + " tile"; } }

	public int x = -1;
	public int y = -1;
	public Position location { get { return new Position(x, y); } }


	//Building stuff
	public bool isEmpty { get { return !areThereVisualObject && !areThereSimObject;  } }

	public bool areThereVisualObject {get{ return visualObject != null; } }
	public GameObject visualObject;
	public bool areThereSimObject {get{ return simObject != null; } }
	private SimGridObject _simObject;

	public SimGridObject simObject {
		get {
			return _simObject;
		}
		set {
			_simObject = value; objectUpdatedCallback?.Invoke();
		}
	}

	const int maxHeight = 2;
	public bool buildingPlaceable { get { return height <= maxHeight; } }
	public bool beltPlaceable { get { return height <= maxHeight; } }

	// For map generation
	public int height = 0;
	public int material = 0;
	public int oreType = 0; // Ore types start counting from 1, 0 means no ore
	public int oreAmount = 0;

	public TileData (int _x, int _y) {
		x = _x;
		y = _y;
	}

	public TileData (int _height, int _material, int _oreType, int _oreAmount, int _x, int _y) {
		height = _height;
		material = _material;
		oreType = _oreType;
		oreAmount = _oreAmount;
		x = _x;
		y = _y;
	}

	public TileData (DataSaver.TileData dat, int _x, int _y) {
		height = dat.height;
		material = dat.material;
		oreType = dat.oreType;
		oreAmount = dat.oreAmount;
		x = _x;
		y = _y;
	}

	public GenericCallback objectUpdatedCallback;
}

