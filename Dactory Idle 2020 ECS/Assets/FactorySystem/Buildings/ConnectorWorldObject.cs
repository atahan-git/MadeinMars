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

	public bool isBuilt = false;
	
	public void PlaceInWorld (int _direction, Position _location, TileData _myTile, bool isSpaceLanding, bool _isBuilt) {
		_PlaceInWorld(_direction, _location, _myTile, isSpaceLanding, _isBuilt);
	}

	public float width;
	public float height;

	void _PlaceInWorld(int _direction, Position _location, TileData _myTile, bool isSpaceLanding, bool _isBuilt) {
		myPos = _location;
		myDir = _direction;
		myTile = _myTile;
		isBuilt = _isBuilt;
		
		myTile.worldObject = this.gameObject;
		
		myTile.objectUpdatedCallback += TileUpdated;

		myRend = GetComponentInChildren<SpriteGraphicsController>();
		
		DataSaver.saveEvent += SaveYourself;
		transform.position = _location.Vector3(Position.Type.belt) + Vector3.up/2f + Vector3.right/2f;

		myRend.SetGraphics(FactoryVisuals.s.connectorSprites[myDir]);
		myRend.SetBuildState(false);
		
		if (isSpaceLanding)
			GetComponentInChildren<SpriteGraphicsController>().DoSpaceLanding(null);
		
		if (isBuilt)
			CompleteBuilding();
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
				floor.AddComponent<SpriteRenderer>().sprite = FactoryVisuals.s.connectorBase;
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
	
	public void CompleteBuilding() {
		myConnector = FactorySystem.s.CreateConnector(myPos, myDir);

		isBuilt = true;
		
		// This is called by TileUpdated Instead
		//myRend.SetGraphics(FactoryVisuals.s.beltSprites[myBelt.direction]);
		myRend.SetBuildState(true);
	}
	
	void TileUpdated() {
		if (myTile.areThereConnector) {
			myConnector = myTile.myConnector;
			myDir = myConnector.direction;
			myConnector.ConnectorInputsUpdatedCallback += UpdateConnectorGraphics;
			UpdateConnectorGraphics();
		} else {
			DestroyYourself();
		}
	}

	void SaveYourself () {
		DataSaver.ConnectorsToBeSaved.Add(new DataSaver.ConnectorData(myPos, myDir, isBuilt));
	}

	void OnDestroy () {
		DataSaver.saveEvent -= SaveYourself;
	}

	public void DestroyYourself () {
		if (myTile != null)
			myTile.worldObject = null;
		
		myTile.objectUpdatedCallback -= TileUpdated;
		FactorySystem.s.RemoveConnector(myPos);
		DroneSystem.s.RemoveDroneTask(myPos);
		
		Destroy(gameObject);
	}
}
