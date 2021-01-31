using UnityEngine;
using System.Collections;

public class BeltPulseControl : MonoBehaviour {

	/*public delegate void ForwardsPulse();
	public static event ForwardsPulse forwardsPulse;*/

	public delegate void BackwardsPulse(Object sender);
	public static event BackwardsPulse pulseEvent;

	public float pulseTime = 0.3f;

	// Use this for initialization
	void Start () {
		InvokeRepeating ("Pulsate", pulseTime, pulseTime);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Pulsate () {

		//print ("------------------PulseStarts-----------------");
		/*if (forwardsPulse != null) {
			forwardsPulse ();
		}*/
		if (pulseEvent != null) {
			//print ("checksss");
			pulseEvent (this);
		}


		//print ("------------------PulseEnds------------------");
	}
}
