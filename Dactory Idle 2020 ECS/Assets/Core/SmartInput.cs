using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SmartInput
{
    public static Vector2 inputPos {
		get {
			if (Input.touchCount > 0)
				return Input.GetTouch(0).position;
			else
				return Input.mousePosition.vector2();
		}
	}
}
