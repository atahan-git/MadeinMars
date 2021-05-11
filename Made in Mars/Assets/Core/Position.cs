using System;
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

	public static bool operator ==(Position a, Position b) {
		return (a.x == b.x) && (a.y == b.y);
	}
	
	public static bool operator !=(Position a, Position b) {
		return !((a.x == b.x) && (a.y == b.y));
	}
	
	public static Position operator * (Position a, int b) {
		return new Position(a.x*b , a.y*b);
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
	
	public Vector3 Vector3 (float zPos){
		return new Vector3(x, y, zPos);
	}

	public override string ToString () {
		return string.Format("pos({0}, {1})", x, y);
	}
	
	
	public static int Distance(Position a, Position b) {
		return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
	}

	public static Position MoveTowards(Position start, Position end, int amount) {
		return new Position(start.x + Mathf.Clamp(end.x - start.x, -amount, amount), start.y + Mathf.Clamp(end.y - start.y, -amount, amount));
	}

	public static Position MoveTowardsDiagonalAware(Position start, Position end, int amount) {
		var pos = start;
		for (int i = 0; i < amount; i++) {
			pos = MoveTowardsDiagonalAware(pos, end);
		}

		return pos;
	}
	
	static Position MoveTowardsDiagonalAware(Position start, Position end) {
		if (Mathf.Abs(start.x - end.x) > Mathf.Abs(start.y - end.y)) {
			return new Position(start.x + Mathf.Clamp(end.x - start.x, -1, 1), start.y);
		} else {
			return new Position(start.x, start.y + Mathf.Clamp(end.y - start.y, -1, 1));
		}
	}

	public static int ParallelDirection(Position start, Position end) {
		if (end.y  > start.y) {
			return 1;
		} else if(end.y < start.y) {
			return 1;
		}else if (end.x > start.x) {
			return 2;
		}else if (end.x < start.x) {
			return 2;
		} else {
			return 0;
		}
	}

	public static int CardinalDirection(Position start, Position end) {
		if (end.y  > start.y) {
			return 1;
		} else if(end.y < start.y) {
			return 3;
		}else if (end.x > start.x) {
			return 2;
		}else if (end.x < start.x) {
			return 4;
		} else {
			return 0;
		}
	}
	public static Position GetCardinalDirection(int direction) {
		switch (direction) {
			case 0:
				return new Position(0,0);
			case 1:
				return new Position(0,1);
			case 2:
				return new Position(1,0);
			case 3:
				return new Position(0,-1);
			case 4:
				return new Position(-1,0);
			default:
				return new Position(0,0);

		}
	}
	
	public static Position MoveCardinalDirection(Position start, int direction, int amount) {
		return start + GetCardinalDirection(direction)*amount;
	}

	public bool isValid() {
		return x > 0 && y > 0;
	}

	public static Position InvalidPosition() {
		return new Position(-1, -1);
	}
}
