using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteUpDown : MonoBehaviour {


	Vector3 originalPos;
	Vector3 animPos;

	public Vector3 downPos = new Vector3(0, -0.03f, 0);
	public float updownanimspeed = 0.005f;
	public float updownstoptime = 1f;

	public bool isGoingDown = true;


	float updowntimer = 0;

	// Start is called before the first frame update
	void Start() {
		originalPos = transform.localPosition;
		animPos = originalPos;
		updowntimer += Random.Range(0, updownstoptime * 5);
	}

	// Update is called once per frame
	void Update() {
		if (isGoingDown && updowntimer < 0) {
			animPos = Vector3.MoveTowards(animPos, downPos, updownanimspeed * Time.deltaTime);
			if (Mathf.Abs(animPos.y - downPos.y) < 0.001f) {
				updowntimer += updownstoptime;
				isGoingDown = false;
			}
		} else if (updowntimer < 0) {
			animPos = Vector3.MoveTowards(animPos, originalPos, updownanimspeed * Time.deltaTime);
			if (Mathf.Abs(animPos.y - originalPos.y) < 0.001f) {
				updowntimer += updownstoptime;
				isGoingDown = true;
			}
		}

		updowntimer -= Time.deltaTime;
	}
}
