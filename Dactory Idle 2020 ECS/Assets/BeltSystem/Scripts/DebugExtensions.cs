using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Some helpful methods for drawing nice debug things.
/// </summary>
public static class DebugExtensions {
	
	public static void DrawSquare (Vector3 pos, Vector3 siz, Color col, bool isTimed) {
		if (!isTimed) {
			pos -= Vector3.forward;
			Vector3 xOff = new Vector3(siz.x, 0, 0);
			Vector3 yOff = new Vector3(0, siz.y, 0);

			Debug.DrawLine(pos - xOff + yOff, pos + xOff + yOff, col);
			Debug.DrawLine(pos + xOff + yOff, pos + xOff - yOff, col);
			Debug.DrawLine(pos + xOff - yOff, pos - xOff - yOff, col);
			Debug.DrawLine(pos - xOff - yOff, pos - xOff + yOff, col);
		} else {
			DrawSquare(pos, siz, col);
		}
	}

	public static void DrawSquare (Vector3 pos, Vector3 siz, Color col) {
		DrawSquare(pos, siz, col, 500f);
	}

		public static void DrawSquare (Vector3 pos, Vector3 siz, Color col, float time) {
		pos -= Vector3.forward;
		Vector3 xOff = new Vector3(siz.x, 0, 0);
		Vector3 yOff = new Vector3(0, siz.y, 0);

		Debug.DrawLine(pos - xOff + yOff, pos + xOff + yOff, col, time);
		Debug.DrawLine(pos + xOff + yOff, pos + xOff - yOff, col, time);
		Debug.DrawLine(pos + xOff - yOff, pos - xOff - yOff, col, time);
		Debug.DrawLine(pos - xOff - yOff, pos - xOff + yOff, col, time);
	}


	public static void DrawArrow (Vector3 start, Vector3 end, Color col) {
		start -= Vector3.forward;
		end -= Vector3.forward;

		Debug.DrawLine(start, end, col, 500f);
		Debug.DrawLine(end, end + (start - end) / 3f + Vector3.Cross(end - start, Vector3.forward) / 3f, col, 500f);
		Debug.DrawLine(end, end + (start - end) / 3f - Vector3.Cross(end - start, Vector3.forward) / 3f, col, 500f);
	}

	public static void DrawNumber (Vector3 pos, int num) {
		pos -= Vector3.forward;
		/*for (int i = 0; Mathf.Abs(i) < Mathf.Abs(num); i += num / Mathf.Abs(num)) {
			if (i < 0)
				Debug.Log(i);
			Debug.DrawLine(pos + (Vector3.up / 20f) - (Vector3.forward / 30f * i), pos - (Vector3.up / 20f) - (Vector3.forward / 30f * i), Color.white);
		}*/

		Debug.DrawLine(pos , pos - (Vector3.forward / 30f * num), Color.white, 500f);
	}
}