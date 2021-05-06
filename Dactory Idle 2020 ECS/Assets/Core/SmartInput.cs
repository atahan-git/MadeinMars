using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A smart input helper that gives the mouse location, or the touch input location based on your device.
/// </summary>
public static class SmartInput
{

	static Vector2 lastPointerLocation = new Vector2(-1, -1);
    public static Vector2 inputPos {
		get {
			#if UNITY_EDITOR
				return Input.mousePosition.vector2();
			#endif
			if (Input.touchSupported) {
				if (Input.touchCount > 0)
					lastPointerLocation = Input.GetTouch(0).position;
				return lastPointerLocation;
			} else {
				return Input.mousePosition.vector2();
			}
				
		}
	}
}
