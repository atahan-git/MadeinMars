using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Handles the drone controls
/// </summary>
public class Player_DroneController : MonoBehaviour {
	public GameObject drone;
	public GameObject droneCommandOverlay;
	
	public bool givingDroneCommand = false;

	public Vector3 droneTargetLocation = new Vector3(100, 100, DataHolder.droneLayer);
	public Vector3 droneTargetSmoothingLocation = new Vector3(100, 100, DataHolder.droneLayer);

	public float droneSpeed = 6f;
	public float droneAcceleration = 1f;
	float droneCurSpeed = 0f;

	private DroneAnimator myAnim;
	
	private void Start() {
		myAnim = drone.GetComponent<DroneAnimator>();
		myAnim.SetMiningLaser(false);
		drone.transform.position = droneTargetLocation;
	}

	private TileData myTile;

	public bool isInTargetLocation = false;

	public GameObject droneTargetMarker;

	private float droneMaxSpeed = 0;

	public int curOreId = -1;
	Item curItem;
	public float miningTimer = 0f;
	public float miningTimeReq = 2f;
	public bool isMiningMission = false;
	public float totalDistance;
	private void Update() {
		if (givingDroneCommand) {
			if (Input.GetMouseButton(0)) {
				myTile = Grid.s.GetTileUnderPointer();

				droneTargetLocation = myTile.position.Vector3(Position.Type.drone);
				
				droneTargetMarker.transform.position = droneTargetLocation + new Vector3(0.5f,0.5f);
				droneTargetMarker.SetActive(true);

				myAnim.SetDirection((droneTargetLocation - drone.transform.position).x > 0);
				if ((droneTargetLocation - drone.transform.position).x > 0) {
					droneTargetLocation += Vector3.left;
				} else {
					droneTargetLocation += Vector3.right;
				}

				miningTimer = 0;
				totalDistance = Vector3.Distance(drone.transform.position, droneTargetLocation);

				myAnim.SetMiningLaser(false);
				if (myTile.oreAmount > 0) {
					isMiningMission = true;
					curOreId = myTile.oreType;
					if (DataHolder.s.OreIdtoUniqueName(curOreId, out string oreName)) {
						curItem = DataHolder.s.GetItem(oreName);
					} else {
						curItem = null;
					}
					if (curItem == null)
						isMiningMission = false;
					
					droneTargetMarker.GetComponentInChildren<SpriteRenderer>().color = Color.yellow;
				} else {
					curOreId = -1;
					isMiningMission = false;
					
					droneTargetMarker.GetComponentInChildren<SpriteRenderer>().color = Color.green;
				}
				
				ToggleDroneCommandMode();
			}
		}

		float distance = Vector3.Distance(drone.transform.position, droneTargetLocation);

		isInTargetLocation = distance < 0.1f;

		drone.transform.position =
			Vector3.MoveTowards(drone.transform.position, droneTargetSmoothingLocation, droneCurSpeed * Time.deltaTime);

		Vector3 direction = droneTargetLocation - drone.transform.position;
		direction = Vector3.Cross(direction, Vector3.back).normalized;
		droneTargetSmoothingLocation =
			Vector3.MoveTowards(droneTargetSmoothingLocation, droneTargetLocation + direction*(Vector3.Distance(droneTargetSmoothingLocation,droneTargetLocation)/2f), 12f * Time.deltaTime);
		Debug.DrawLine(droneTargetSmoothingLocation, droneTargetSmoothingLocation+Vector3.up);
		
		if (totalDistance > 4) {
			if (distance > 2) {
				droneCurSpeed = Mathf.Lerp(droneCurSpeed, droneSpeed, droneAcceleration * Time.deltaTime);
				droneMaxSpeed = droneCurSpeed;
			} else {
				droneCurSpeed = Mathf.Lerp(droneCurSpeed, 0.2f, (droneMaxSpeed / 2) * Time.deltaTime);
			}
		} else {
			if (distance > 0.5f) {
				droneCurSpeed = Mathf.Lerp(droneCurSpeed, droneSpeed, droneAcceleration * Time.deltaTime);
				droneMaxSpeed = droneCurSpeed;
			} else {
				droneCurSpeed = Mathf.Lerp(droneCurSpeed, 0.2f, (droneMaxSpeed / 0.5f) * Time.deltaTime);
			}
		}

		if (isInTargetLocation) {
			droneTargetMarker.SetActive(false);
		}

		if (isMiningMission && isInTargetLocation) {
			if (miningTimer == 0) {
				myAnim.SetMiningLaser(true);
			}

			miningTimer += Time.deltaTime;

			if (miningTimer >= miningTimeReq) {
				myAnim.ShowPlusOne();
				Player_InventoryController.s.TryAddItem(curItem);
				miningTimer = 0;
				myTile.oreAmount -= 1;

				if (myTile.oreAmount <= 0) {
					isMiningMission = false;
					myAnim.SetMiningLaser(false);
				}
			}
		}
	}

	public void ToggleDroneCommandMode() {
		givingDroneCommand = !givingDroneCommand;
		droneCommandOverlay.SetActive(givingDroneCommand);
	}
}
