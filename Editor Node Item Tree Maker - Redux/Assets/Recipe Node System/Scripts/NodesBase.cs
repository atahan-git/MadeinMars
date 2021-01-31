using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Node {
	public int id; // A unique ID
	public Vector3 pos; // the position of the node

	public List<Port> leftPorts = new List<Port>();
	public List<Port> rightPorts = new List<Port>();

	protected Node (int id) {
		this.id = id;
	}

	public abstract List<Port> GetRightPorts();
	public abstract int GetRightPortType();
	public abstract bool GetRightPortConnectivity(int type);
	public abstract List<Port> GetLeftPorts();

}


[Serializable]
public abstract class Port {
	public int connectedNodeId = -1; // which node are we connected to?
	public int connectedNodePortIndex = -1; // which port in that node are we connected to?
	public bool connectedNodePortSideisLeft; // which side are we connected to?
	
	// A value of -1 means not connected to anything

	public Port () { }
}