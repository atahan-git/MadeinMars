using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// The main building object. Should work to store data for the other components, and deal with placing/removing the building.
/// </summary>
public class ConnectorWorldObject : MonoBehaviour, IBuildable
{

	public Connector myConnector;
	public Position myPos;
	public int myDir;
	public TileData myTile;

	SpriteGraphicsController myRend;
	
	
	BuildingInventoryController myInventory;

	public bool isBuilt = false;
	

	public float width;
	public float height;

	public void PlaceInWorld(int _direction, Position _location, TileData _myTile, bool isSpaceLanding, bool _isBuilt, List<InventoryItemSlot> inventory) {
		myPos = _location;
		myDir = _direction;
		myTile = _myTile;
		isBuilt = _isBuilt;
		
		CreateConstructionInventory(inventory);
		
		myTile.worldObject = this.gameObject;

		myRend = GetComponentInChildren<SpriteGraphicsController>();

		
		DataSaver.saveEvent += SaveYourself;
		transform.position = _location.Vector3(Position.Type.belt) + Vector3.up/2f + Vector3.right/2f;

		myRend.SetGraphics(FactoryVisuals.s.connectorSprites[myDir]);
		myRend.SetBuildState(SpriteGraphicsController.BuildState.construction);
		
		if (isSpaceLanding)
			GetComponentInChildren<SpriteGraphicsController>().DoSpaceLanding(null);
		
		if (isBuilt)
			CompleteBuilding();
	}
	
	public BuildingInventoryController GetConstructionInventory() {
		return myInventory;
	}
	public BuildingInventoryController CreateConstructionInventory(List<InventoryItemSlot> inventory) {
		myInventory = new BuildingInventoryController();
		myInventory.SetUpConstruction(myPos);
		myInventory.SetInventory(inventory);

		return myInventory;
	}
	
	public void CompleteBuilding() {
		myConnector = FactorySystem.s.CreateConnector(myPos, myDir);

		isBuilt = true;

		myTile.objectUpdatedCallback += TileUpdated;
		
		
		UpdateConnectorGraphics();
		myRend.SetBuildState(SpriteGraphicsController.BuildState.built);
	}
	
	void TileUpdated() {
		if (myTile.areThereConnector) {
			myConnector = myTile.myConnector;
			myDir = myConnector.direction;
			myConnector.ConnectorInputsUpdatedCallback += UpdateConnectorGraphics;
			UpdateConnectorGraphics();
		} else {
			MarkForDeconstruction();
		}
	}

	void SaveYourself () {
		DataSaver.ConnectorsToBeSaved.Add(new DataSaver.ConnectorData(myPos, myDir, isBuilt, myInventory.inventory));
	}

	void OnDestroy () {
		DataSaver.saveEvent -= SaveYourself;
	}


	public bool isMarkedForDestruction = false;
	public void MarkForDeconstruction() {
		if (!isMarkedForDestruction) {
			if (isBuilt) {
				isMarkedForDestruction = true;
				isBuilt = false;
				DroneSystem.s.AddDroneDestroyTask(myPos, FactoryBuilder.s.connectorBuildingData);
				myRend.SetBuildState(SpriteGraphicsController.BuildState.destruction);
				
				myTile.objectUpdatedCallback -= TileUpdated;
				FactorySystem.s.RemoveConnector(myPos);
				
			} else {
				DestroyYourself();
			}
		}
	}

	public void UnmarkDestruction() {
		if (isMarkedForDestruction) {
			isMarkedForDestruction = false;
			DroneSystem.s.RemoveDroneTask(myPos);
			DroneSystem.s.AddDroneBuildTask(myPos, FactoryBuilder.s.connectorBuildingData);
		}
	}
	

	public void DestroyYourself () {
		if (myTile != null)
			myTile.worldObject = null;

		if (isBuilt) {
			myTile.objectUpdatedCallback -= TileUpdated;
			FactorySystem.s.RemoveConnector(myPos);
		}

		DroneSystem.s.RemoveDroneTask(myPos);
		
		Destroy(gameObject);
	}
	
	

	private GameObject floor;
	private GameObject[] pullers = new GameObject[0];
	public void UpdateConnectorGraphics() {
		try {
			transform.position = new Vector3(transform.position.x, transform.position.y, DataHolder.connectorOverlayLayer);
			myRend.SetGraphics(FactoryVisuals.s.connectorSprites[myConnector.direction]);
			if (floor == null) {
				floor = new GameObject();
				floor.transform.position = new Vector3(transform.position.x, transform.position.y, DataHolder.connectorBaseLayer);
				floor.transform.SetParent(transform);
				floor.AddComponent<SpriteRenderer>().sprite = FactoryVisuals.s.connectorSpritesBases[myConnector.direction];
			}

			for (int i = 0; i < pullers.Length; i++) {
				Destroy(pullers[i]);
			}

			List<Connector.Connection> myConnections = new List<Connector.Connection>();

			for (int i = 0; i < myConnector.inputs.Count; i++) {
				if (myConnector.inputs[i].position == myPos) {
					myConnections.Add(myConnector.inputs[i]);
				}
			}
			for (int i = 0; i < myConnector.outputs.Count; i++) {
				if (myConnector.outputs[i].position == myPos) {
					myConnections.Add(myConnector.outputs[i]);
				}
			}

			pullers = new GameObject[myConnections.Count];

			for (int i = 0; i < pullers.Length; i++) {
				var connection = myConnections[i];
				pullers[i] = new GameObject();

				pullers[i].transform.position = new Vector3(transform.position.x, transform.position.y, DataHolder.connectorPullerLayer)
				                                + Position.GetCardinalDirection(connection.direction).Vector3(0f) / 2f;

				pullers[i].transform.SetParent(transform);
				pullers[i].AddComponent<SpriteRenderer>().sprite = FactoryVisuals.s.connectorPullerSprites[connection.direction];
				
			}
		} catch (System.Exception e) {
			Debug.LogError("Connector graphic creation failed!" + e + " - " + e.StackTrace);
		}
	}
}
