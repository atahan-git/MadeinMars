using UnityEngine;
using System.Collections;

public class MovingObject : MonoBehaviour {

	public int type = 0;

	Vector3 randomOffset = Vector3.zero;

	Vector3 _placeToBe = Vector3.zero;
	public Vector3 placeToBe{
		set{
			//transform.position = _placeToBe;
			_placeToBe = value;
		}
		get{
			return _placeToBe;
		}
	}


	// Use this for initialization
	void Start () {

		randomOffset = new Vector3(
			Random.Range(-BeltSpeed.s.randomValue, BeltSpeed.s.randomValue),
			Random.Range(-BeltSpeed.s.randomValue, BeltSpeed.s.randomValue),
			0
		);
	
	}
	
	// Update is called once per frame
	void Update () {
		if (BeltSpeed.s.mode)
			transform.position = Vector3.Lerp (transform.position, placeToBe + randomOffset, BeltSpeed.s.speed * Time.deltaTime);
		else
			transform.position = Vector3.MoveTowards (transform.position, placeToBe  + randomOffset, BeltSpeed.s.speed * Time.deltaTime);
	}

	public void SelfDestruct(Vector3 offset){
		placeToBe = placeToBe + offset;
		Invoke ("DestroySelf", 0.2f);
	}

	public void DestroySelf(){
		Destroy (gameObject);
	}
}
