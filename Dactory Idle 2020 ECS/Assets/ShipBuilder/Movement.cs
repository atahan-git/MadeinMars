using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

	public int forwardForce = 100;
	public int rotationForce = 20;
	public Rigidbody2D myrigidbody2D = new Rigidbody2D();
	public GameObject cursor;

	// Use this for initialization
	void Start () {
		myrigidbody2D = GetComponent <Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		//forward movement
		float forward = Input.GetAxis ("Vertical");
		forward *= forwardForce;
		myrigidbody2D.AddRelativeForce (new Vector2(0, forward));

		float rotate = Input.GetAxis ("Horizontal");
		rotate *= rotationForce;
		myrigidbody2D.AddTorque(rotate);

		Cursor ();
		//rotate toward cursor
		/*Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
		Vector3 dir = Input.mousePosition - pos;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward); */
	}

	void Cursor(){
		int distanceFromCamera = 10;
		Vector3 pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceFromCamera);
		pos = Camera.main.ScreenToWorldPoint(pos);
		cursor.transform.position = pos;
	}
}
