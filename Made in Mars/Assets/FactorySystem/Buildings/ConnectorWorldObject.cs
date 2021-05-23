using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// The main building object. Should work to store data for the other components, and deal with placing/removing the building.
/// </summary>
public class ConnectorWorldObject : MonoBehaviour
{

	[SerializeField] bool isConstruction;
	[SerializeField] Connector myConnector;
	[SerializeField] Construction myConstruction;
	[SerializeField] TileData myTile;
	[SerializeField] Position location;
	[SerializeField] int direction;

	SpriteGraphicsController myRend;

	[SerializeField] float width;
	[SerializeField] float height;

	public void UpdateSelf(Position _location, Connector _connector) {
		RemoveSelfFromTile();
		myConnector = _connector;
		direction = myConnector.direction;
		isConstruction = false;
		
		/*if (isSpaceLanding)
			GetComponentInChildren<SpriteGraphicsController>().DoSpaceLanding(null);*/

		myRend = GetComponentInChildren<SpriteGraphicsController>();
		GenericUpdateSelf(_location);
		myRend.SetBuildState(SpriteGraphicsController.BuildState.built);

		myConnector.ConnectorInputsUpdatedCallback -= UpdateConnectorGraphics;
		myConnector.ConnectorInputsUpdatedCallback += UpdateConnectorGraphics;
		UpdateConnectorGraphics();
	}
	
	public void UpdateSelf(Position _location, Construction _construction) {
		RemoveSelfFromTile();
		myConstruction = _construction;
		direction = myConstruction.direction;
		isConstruction = true;

		/*if (isSpaceLanding)
			GetComponentInChildren<SpriteGraphicsController>().DoSpaceLanding(null);*/
		
		myRend = GetComponentInChildren<SpriteGraphicsController>();
		GenericUpdateSelf(_location);
		if (myConstruction.isConstruction) {
			myRend.SetBuildState(SpriteGraphicsController.BuildState.construction);
		} else {
			myRend.SetBuildState(SpriteGraphicsController.BuildState.destruction);
		}

	}
	
	void GenericUpdateSelf(Position _location) {
		myTile = Grid.s.GetTile(_location);
		location = _location;
		
		myTile.worldObject = this.gameObject;
		myTile.objectUpdatedCallback -= TileUpdated; // we want to make sure we get the update callback only once
		myTile.objectUpdatedCallback += TileUpdated;
		
		transform.position = _location.Vector3(Position.Type.belt) + Vector3.up / 2f + Vector3.right / 2f;
		myRend.SetGraphics(FactoryVisuals.s.connectorSprites[direction]);
	}
	
	void TileUpdated() {
		if (isConstruction) {
			if (myTile.areThereConstruction) {
				myConstruction = myTile.myConstruction;
				direction = myConstruction.direction;
					
				myRend.SetGraphics(FactoryVisuals.s.connectorSprites[myConstruction.direction]);
			} else {
				DestroyYourself();
			}
		} else {
			if (myTile.areThereConnector) {
				myConnector = myTile.myConnector;
				direction = myConnector.direction;
			
				myConnector.ConnectorInputsUpdatedCallback -= UpdateConnectorGraphics;
				myConnector.ConnectorInputsUpdatedCallback += UpdateConnectorGraphics;
				UpdateConnectorGraphics();
			} else {
				DestroyYourself();
			}
		}
	}
	
	public void RemoveSelfFromTile() {
		myTile.worldObject = null;
		myTile.objectUpdatedCallback -= TileUpdated;
	}

	public void DestroyYourself() {
		RemoveSelfFromTile();
		GetComponent<PooledGameObject>().DestroyPooledObject();
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
				if (myConnector.inputs[i].position == location) {
					myConnections.Add(myConnector.inputs[i]);
				}
			}
			for (int i = 0; i < myConnector.outputs.Count; i++) {
				if (myConnector.outputs[i].position == location) {
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
