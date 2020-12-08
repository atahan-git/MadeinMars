using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// A helper class to hold positional information
/// Basically a sort of Vector2 specifically made for this game
/// </summary>
[System.Serializable]
public struct Position {
	public int x;
	public int y;

	public static float defaultPositionVector3Z = 0;

	public enum Type { world, belt, item, building, drone };

	public Position (int _x, int _y) {
		x = _x;
		y = _y;
	}

	public static Position operator + (Position a, Position b) {
		return new Position(a.x + b.x, a.y + b.y);
	}

	public static Position operator - (Position a, Position b) {
		return new Position(a.x - b.x, a.y - b.y);
	}

	public static Position operator - (Position a, Vector2 b) {
		return new Position(a.x - (int)b.x, a.y - (int)b.y);
	}

	public Vector3 Vector3 (Type type){
		float zPos = defaultPositionVector3Z;
		switch (type) {
		case Type.world:
			zPos = DataHolder.worldLayer;
			break;
		case Type.belt:
			zPos = DataHolder.beltLayer;
			break;
		case Type.item:
			zPos = DataHolder.itemLayer;
			break;
		case Type.building:
			zPos = DataHolder.buildingLayer;
			break;
		case Type.drone :
			zPos = DataHolder.droneLayer;
			break;
		}
		return new Vector3(x, y, zPos);
	}

	public override string ToString () {
		return string.Format("pos({0}, {1})", x, y);
	}
}
