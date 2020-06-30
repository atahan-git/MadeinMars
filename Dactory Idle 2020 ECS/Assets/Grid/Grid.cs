using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
	public static Grid s;
	public MapGenerator mapGen;

	public int mapSize = 200;
	private void Awake () {
		if (s != null) {
			Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
		}
		s = this;

		myTiles = new TileData[mapSize, mapSize];

		mapGen = GetComponent<MapGenerator>();

		DataSaver.saveEvent += SaveTiles;
	}


	public TileData[,] myTiles;

	public TileData GetTile (Position pos) {
		return myTiles[pos.x, pos.y];
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

	public void LoadTiles () {
		mapGen.Clear();
		bool loadDataExists = DataSaver.mySave.tileData != null;
		if(loadDataExists)

		if (DataSaver.mySave.tileData != null && DataSaver.mySave.tileData.Length > 0) {
			int xLength = DataSaver.mySave.tileData.GetLength(0);
			int yLength = DataSaver.mySave.tileData.GetLength(0);
			int[,] materials = new int [xLength,yLength];
			int[,] height = new int[xLength, yLength];
			int[][,] oreAmounts = new int[DataHolder.s.GetAllOres().Length][,];
			for (int i = 0; i < DataHolder.s.GetAllOres().Length; i++) {
				oreAmounts[i] = new int[xLength, yLength];
			}
			myTiles = new TileData[xLength, yLength];
			for (int x = 0; x < DataSaver.mySave.tileData.GetLength(0); x++) {
				for (int y = 0; y < DataSaver.mySave.tileData.GetLength(1); y++) {
					DataSaver.TileData dat = DataSaver.mySave.tileData[x, y];
					myTiles[x, y] = new TileData(dat, x, y);
					materials[x, y] = dat.material;
					height[x, y] = dat.height;
					if(dat.oreType > 0 && dat.oreType <= oreAmounts.GetLength(0))
					oreAmounts[dat.oreType-1][x, y] = dat.oreAmount;
				}
			}

			mapGen.LoadMap(materials, height);
			for (int i = 0; i < DataHolder.s.GetAllOres().Length; i++) {
				mapGen.LoadResources(oreAmounts[i], DataHolder.s.GetAllOres()[i]);
			}

			print("Map successfully loaded");
		} else {
			GenerateTiles();
		}
	}

	public void GenerateTiles () {
		mapGen.Clear();
		int[,] materials;
		int[,] height;
		mapGen.mapSize = new Vector2(mapSize, mapSize);
		var map = mapGen.GenerateMap();
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
						oreAmount[x, y] = oreAmountsPrepass[i][x, y];
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
	}

	public void SaveTiles () {
		DataSaver.TileDataToBeSaved = new DataSaver.TileData[myTiles.GetLength(0), myTiles.GetLength(1)];
		for (int x = 0; x < myTiles.GetLength(0); x++) {
			for (int y = 0; y < myTiles.GetLength(1); y++) {
				TileData ct = myTiles[x, y];
				DataSaver.TileDataToBeSaved[x, y] = new DataSaver.TileData(ct.height, ct.material, ct.oreType, ct.oreAmount);
			}
		}
	}
}



[System.Serializable]
public class TileData {
	public string name { get { return position.ToString() + " tile"; } }

	public int x = -1;
	public int y = -1;
	public Position position { get { return new Position(x, y); } }


	//Building stuff
	public bool isEmpty { get { return !areThereBelt && !areThereBuilding; } }
	public bool areThereBuilding { get { return myBuilding != null; } }
	public GameObject myBuilding;
	public bool areThereBelt { get { return myBelt != null; } }
	public GameObject myBelt;

	const int maxHeight = 1;
	public bool buildingPlaceable { get { return height < maxHeight; } }
	public bool beltPlaceable { get { return height < maxHeight; } }

	// For map generation
	public int height = 0;
	public int material = 0;
	public int oreType = 0;
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
}

