using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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

	TileData[,] myTiles;
	PlanetSchematic scheme;

	private TileData dummyTile = new TileData(-1,-1);
	
	public void RegisterEvents ( MapGenerator _mapGen) {
		mapGen = _mapGen;

		DataSaver.saveEvent += SaveTiles;
		GameMaster.CallWhenLoadedEarly(GameLoadingComplete);
		GameMaster.CallWhenNewPlanetEarly(GenerateTiles);
	}
	
	public void UnRegisterEvents () {
		s = null;
		DataSaver.saveEvent -= SaveTiles;
		GameMaster.RemoveFromCall(GameLoadingComplete);
		GameMaster.RemoveFromCall(GenerateTiles);
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
		var currentPlanet = DataSaver.s.GetSave().currentPlanet;
		bool loadDataExists = currentPlanet != null && currentPlanet.tileData != null;
		if (loadDataExists) {
			var previousPlanetExists = currentPlanet.planetData != null && currentPlanet.tileData != null && currentPlanet.tileData.Length > 0;
			if (previousPlanetExists) {
				scheme = DataHolder.s.GetPlanetSchematic(currentPlanet.planetData.planetSchematicUniqueName);
				int xLength = currentPlanet.tileData.GetLength(0);
				int yLength = currentPlanet.tileData.GetLength(0);
				TileSet[,] materials = new TileSet [xLength, yLength];
				int[,] height = new int[xLength, yLength];
				int[][,] oreAmounts = new int[DataHolder.s.GetAllOres().Length][,];
				for (int i = 0; i < DataHolder.s.GetAllOres().Length; i++) {
					oreAmounts[i] = new int[xLength, yLength];
				}

				myTiles = new TileData[xLength, yLength];
				for (int x = 0; x < currentPlanet.tileData.GetLength(0); x++) {
					for (int y = 0; y < currentPlanet.tileData.GetLength(1); y++) {
						DataSaver.TileData dat = currentPlanet.tileData[x, y];
						myTiles[x, y] = new TileData(dat, x, y);
						materials[x, y] = DataHolder.s.IdToTileSet(dat.material);
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
			} /*else {
				GenerateTiles();
			}*/
		} 
	}
	
	public Planet MakeRandomPlanet() {
		int seed = Random.Range(0, 1000);
		var myPlanet = new Planet(seed);
        
		Random.InitState(seed);
        
		SetPlanetOres(myPlanet);

		var planetType = Random.Range(0, DataHolder.s.allPlanetSchematics.Length);

		myPlanet.schematic = DataHolder.s.allPlanetSchematics[planetType];
		
		SetPlanetCards(myPlanet);

		return myPlanet;
	}

	private static void SetPlanetCards(Planet myPlanet) {
		HashSet<string> existingCanOnlyHaveOneCards = new HashSet<string>();

		var activeCards = ShipDataMaster.s.GetActiveCards().Cast<ShipCard>();
		foreach (var card in activeCards) {
			if (card.canOnlyHaveOne) {
				existingCanOnlyHaveOneCards.Add(card.uniqueName);
			}
		}

		var cardCount = Random.Range(1, 3);
		myPlanet.myPresentCardNames = new string[cardCount];

		for (int i = 0; i < cardCount; i++) {
			ShipCard randomCard;
			do {
				randomCard = DataHolder.s.allShipCards[Random.Range(0, DataHolder.s.allShipCards.Length)];
			} while (!randomCard.isSpawnableOnPlanet ||existingCanOnlyHaveOneCards.Contains(randomCard.uniqueName));

			if (randomCard.canOnlyHaveOne) {
				existingCanOnlyHaveOneCards.Add(randomCard.uniqueName);
			}

			myPlanet.myPresentCardNames[i] = randomCard.uniqueName;
		}
	}

	private static void SetPlanetOres(Planet myPlanet) {
		var oreCount = Random.Range(2, DataHolder.s.allOreDensities.Length);

		var ores = new List<OreDensityLevelsHolder>(DataHolder.s.allOreDensities);

		myPlanet.oreNames = new string[oreCount];
		myPlanet.oreDensities = new int[oreCount];
		for (int i = 0; i < oreCount; i++) {
			int index = Random.Range(0, ores.Count);

			var ore = ores[index];
			var myDensity = Random.Range(0, ore.oreLevels.Length);

			myPlanet.oreNames[i] = ore.GetUniqueName();
			myPlanet.oreDensities[i] = myDensity;

			ores.RemoveAt(index);
		}
	}


	public DataLogger _logger;
	public void GenerateTiles () {
		var time = 0f;
		time = Time.realtimeSinceStartup;
		
		var planet = MakeRandomPlanet();
		DataSaver.s.GetSave().currentPlanet.planetData = new DataSaver.PlanetData(planet.planetGenerationInt, planet.schematic.uniqueName, planet.oreDensities, planet.oreNames, planet.myPresentCardNames);
		scheme = planet.schematic;
		
		mapGen.Clear();
		TileSet[,] materials;
		int[,] height;
		mapGen.mapSize = new Vector2Int(mapSize, mapSize);
		var map = mapGen.GenerateMap(scheme);
		materials = map.Item1;
		height = map.Item2;

		var oreSettings = new List<OreSpawnSettings>();
		for (int i = 0; i < planet.oreDensities.Length; i++) {
			oreSettings.Add(DataHolder.s.GetOreSpawnSettings(planet.oreDensities[i], planet.oreNames[i]));
		}
		int[,] oreType = new int[mapSize,mapSize];
		int[,] oreAmount = new int[mapSize, mapSize];
		int[][,] oreAmountsPrepass = new int[oreSettings.Count][,];
		int[] oreIds = new int[oreSettings.Count];
		for (int i = 0; i < oreSettings.Count; i++) {
			oreAmountsPrepass[i] = mapGen.GenerateResources(oreSettings[i]);
			DataHolder.s.UniqueNameToOreId(oreSettings[i].oreUniqueName, out int oreId);
			oreIds[i] = oreId;
		}

		myTiles = new TileData[mapSize, mapSize];

		for (int x = 0; x < materials.GetLength(0); x++) {
			for (int y = 0; y < materials.GetLength(1); y++) {
				for (int i = 0; i < oreAmountsPrepass.Length; i++) {
					if (oreAmountsPrepass[i][x, y] > 0) {
						oreType[x, y] = oreIds[i];
						oreAmount[x, y] = oreAmountsPrepass[i][x, y] * 10000;
						break;
					}
				}


				myTiles[x, y] = new TileData(x,y);
				myTiles[x, y].material = materials[x, y].tileSetId;
				myTiles[x, y].height = height[x, y];
				myTiles[x, y].oreType = oreType[x,y];
				myTiles[x, y].oreAmount = oreAmount[x, y]; 
			}
		}
		
		if(_logger)
			_logger.mapGenerationTime.Add(Time.realtimeSinceStartup - time);
	}

	public void SaveTiles () {
		if (myTiles != null) {
			DataSaver.s.GetSave().currentPlanet.tileData = new DataSaver.TileData[myTiles.GetLength(0), myTiles.GetLength(1)];
			for (int x = 0; x < myTiles.GetLength(0); x++) {
				for (int y = 0; y < myTiles.GetLength(1); y++) {
					TileData ct = myTiles[x, y];
					DataSaver.s.GetSave().currentPlanet.tileData[x, y] = new DataSaver.TileData(ct.height, ct.material, ct.oreType, ct.oreAmount);
				}
			}
		}
	}
}

[System.Serializable]
public class Planet {
	public int planetGenerationInt;
	public PlanetSchematic schematic;
	public int[] oreDensities =new int[0];
	public string[] oreNames = new string[0];
	public string[] myPresentCardNames = new string[0];

	public Planet(DataSaver.PlanetData data) {
		planetGenerationInt = data.planetId;
		schematic = DataHolder.s.GetPlanetSchematic(data.planetSchematicUniqueName);
		oreDensities = data.oreDensities;
		oreNames = data.oreUniqueNames;
		myPresentCardNames = data.shipCardUniqueNames;
	}

	public Planet(int planetGenerationInt) {
		this.planetGenerationInt = planetGenerationInt;
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

