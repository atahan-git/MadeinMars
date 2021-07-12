using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Eventually, when a new game would need fancier things like a starter area, this script will deal with it.
/// </summary>
public class NewGameWorldSetup : MonoBehaviour
{
	public static NewGameWorldSetup s;

	public BuildingData mine;

	public OreSpawnSettings ironOre;
	public OreSpawnSettings copperOre;
	public OreSpawnSettings concreteOre;
	
	public BuildingData furnace;
	public BuildingData storage;
	public BuildingData press;

	private void Awake () {
		if (s != null) {
			Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
		}
		s = this;
	}

	/*private void Start() {
		GameLoader.CallWhenNewPlanet(SetUpNewGameWorld);
	}
	
	private void OnDestroy() {
		GameLoader.RemoveFromCall(SetUpNewGameWorld);
	}*/
	

	void LayBelt(Position start, int direction, int amount) {
		for (int i = 0; i < amount; i++) {
			FactoryBuilder.CreateBelt(Position.MoveCardinalDirection(start,direction,i), direction);
		}
	}
	
	void LayConnector(Position start, int direction, int amount) {
		for (int i = 0; i < amount; i++) {
			FactoryBuilder.CreateConnector(Position.MoveCardinalDirection(start,direction,i), direction);
		}
	}
	
	public void SetUpNewPlanet() {
		
		Debug.Log("------------------- Starting new planet -------------------");

		var shipPosition = FactoryMaster.s.GetBuildings()[0].center;
		FactoryBuilder.CreateDrone(shipPosition + new Position(-2,-1));
		FactoryBuilder.CreateDrone(shipPosition + new Position(-1,-1));
		FactoryBuilder.CreateDrone(shipPosition + new Position(2,-1));

		SetUpStarterFactory();
	}


	public void SetUpStarterFactory() {
		//SetUpStressTest();
		// Basic Iron System
		var ironMinePosition = new Position(120, 120);
		DataHolder.s.UniqueNameToOreId(ironOre.oreUniqueName, out int ironOreId);
		Grid.s.GetTile(ironMinePosition).oreType = ironOreId;
		Grid.s.GetTile(ironMinePosition).oreAmount = 10000;
		var ironMiner = FactoryBuilder.CreateBuilding(mine, ironMinePosition, null);
		ironMiner.isDestructable = false;
		FactoryBuilder.CreateConnector(ironMinePosition + new Position(0, -2), 0);
		FactoryBuilder.CreateBelt(ironMinePosition + new Position(0, -3), 3);
		FactoryBuilder.CreateBelt(ironMinePosition + new Position(0, -4), 3);
		FactoryBuilder.CreateConnector(ironMinePosition + new Position(0, -5), 2);
		FactoryBuilder.CreateConnector(ironMinePosition + new Position(1, -5), 2);
		FactoryBuilder.CreateBuilding(furnace, ironMinePosition + new Position(-1, -7), null);
		FactoryBuilder.CreateBuilding(furnace, ironMinePosition + new Position(2, -7), null);
		FactoryBuilder.CreateConnector(ironMinePosition + new Position(-1, -8), 2);
		FactoryBuilder.CreateConnector(ironMinePosition + new Position(0, -8), 2);
		FactoryBuilder.CreateConnector(ironMinePosition + new Position(1, -8), 2);
		FactoryBuilder.CreateBuilding(press, ironMinePosition + new Position(-1, -10), null);
		FactoryBuilder.CreateBuilding(storage, ironMinePosition + new Position(2, -10), null);


		// Copper Mine
		var copperMinePosition = new Position(80, 100);
		DataHolder.s.UniqueNameToOreId(copperOre.oreUniqueName, out int copperOreId);
		Grid.s.GetTile(copperMinePosition).oreType = copperOreId;
		Grid.s.GetTile(copperMinePosition).oreAmount = 10000;
		var copperMine = FactoryBuilder.CreateBuilding(mine, copperMinePosition, null);
		copperMine.isDestructable = false;
		
		
		// Concrete Mine
		var concreteMinePosition = new Position(140, 140);
		DataHolder.s.UniqueNameToOreId(concreteOre.oreUniqueName, out int concreteOreId);
		Grid.s.GetTile(concreteMinePosition).oreType = concreteOreId;
		Grid.s.GetTile(concreteMinePosition).oreAmount = 10000;
		var concreteMine = FactoryBuilder.CreateBuilding(mine, concreteMinePosition, null);
		concreteMine.isDestructable = false;
		
		
		// Create starter drones
		/*FactoryBuilder.CreateDrone(new Position(90,90));
		FactoryBuilder.CreateDrone(new Position(92,90));
		FactoryBuilder.CreateDrone(new Position(94,90));*/
	}

	void SetUpStressTest() {
		Debug.Log("------------------- Setting up stress test -------------------");
		for (int x = 12; x < 180; x += 12) {
			for (int y = 12; y < 180; y += 12) {
				var offset = new Position(x, y);
				DataHolder.s.UniqueNameToOreId(ironOre.oreUniqueName, out int ironOreId);
				Grid.s.GetTile(offset).oreType = ironOreId;
				Grid.s.GetTile(offset).oreAmount = 10000;
				var ironMiner = FactoryBuilder.CreateBuilding(mine, offset, null);
				ironMiner.isDestructable = false;
				FactoryBuilder.CreateConnector(offset + new Position(0, -2), 0);
				LayBelt(offset + new Position(0, -3), 3, 5);
				LayConnector(offset + new Position(0, -8), 2, 6);
				FactoryBuilder.CreateBuilding(furnace, offset + new Position(2, -7), null);
				FactoryBuilder.CreateBuilding(press, offset + new Position(6, -7), null);
				FactoryBuilder.CreateBuilding(storage, offset + new Position(3, -5), null);
				FactoryBuilder.CreateBuilding(storage, offset + new Position(8, -8), null);
				LayBelt(offset + new Position(4, -7), 1, 8);
				LayBelt(offset + new Position(3, 0), 3, 3);
				LayBelt(offset + new Position(5, 0), 3, 4);
				LayBelt(offset + new Position(7, -3), 1, 4);
				LayBelt(offset + new Position(8, 0), 3, 6);
				LayConnector(offset + new Position(2, -3), 2, 2);
				LayConnector(offset + new Position(3, 1), 2, 3);
				LayConnector(offset + new Position(5, -4), 2, 3);
				LayConnector(offset + new Position(7, 1), 2, 2);
				LayConnector(offset + new Position(8, -6), 0, 1);
			}
		}
	}

}
