using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneWorldObject : MonoBehaviour {

    public Drone myDrone;
    public DroneAnimator myAnim;
    
    public bool isInventorySetup = false;
    public GenericCallback buildingInventoryUpdatedCallback;
    
    public void SetUp(Drone _drone) {
	    myDrone = _drone;
	    isInventorySetup = true;
	    buildingInventoryUpdatedCallback?.Invoke();
	    myAnim = GetComponent<DroneAnimator>();

	    transform.position = myDrone.curPosition.Vector3(Position.Type.drone);
	    if(!myDrone.targetPosition.isValid())
		    myAnim.FlyToLocation(myDrone.targetPosition);
	    else
		    myAnim.FlyToLocation(myDrone.curPosition);

	    myAnim.SetMiningLaser(myDrone.isLaser);
	    myDrone.dronePositionUpdatedCallback += CurrentPositionChanged;
	    myDrone.droneLaserStateUpdatedCallback += LaserStateChanged;
	    myDrone.droneTargetPositionUpdatedCallback += MovementTargetChanged;

	    myAnim.SetUp();
    }


    void MovementTargetChanged() {
	    myAnim.FlyToLocation(myDrone.targetPosition);
    }

    void CurrentPositionChanged() {
	    Debug.DrawLine(myDrone.curPosition.Vector3(Position.Type.drone), myDrone.curPosition.Vector3(Position.Type.drone) + Vector3.up, Color.green, 1f);
    }

    void LaserStateChanged() {
	    myAnim.SetMiningLaser(myDrone.isLaser);
    }
}
