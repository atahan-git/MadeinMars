using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Position {
	public int x;
	public int y;

	public static float defaultPositionVector3Z = 0;

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

	public Vector3 vector3 {
		get {
			return new Vector3(x, y, defaultPositionVector3Z);
		}
	}

	public override string ToString () {
		return string.Format("pos({0}, {1})", x, y);
	}
}
