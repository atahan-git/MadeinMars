using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


/// <summary>
/// Controls the animations of the drone.
/// </summary>
public class DroneAnimator : MonoBehaviour {

    public GameObject animTarget;

    private bool isLookingRight = false;

    public float updownOffset = 0;
    public AnimationCurve updowncurve;

    public float speed = 0.2f;
    public float magnitude = 0.5f;

    public Vector3 gfxOffset;

    public LineRenderer miningLaser;
    private bool isLaser = false;
    
    public AnimationCurve maCurvy = new AnimationCurve ();
    Keyframe[] animCurvys = new Keyframe[10];

    public GameObject plusOne;

    public ParticleSystem miningParticles;
    private void Start() {
        DisablePlusOne();
        SetMiningLaser(false);
        for (int i = 0; i < 10; i++) {
            animCurvys[i].time = Mathf.Lerp (0, 1, i/10f);
        }
        droneTargetMarker.transform.SetParent(null);

        updownOffset = Random.Range(0, 10f);
    }

    void Update() {
        animTarget.transform.localPosition =
            new Vector3(0, magnitude * updowncurve.Evaluate(((Time.time+updownOffset) * speed) % 1), 0) + gfxOffset;

        if (isLaser) {
	        UpdateMiningLaser();
        }

        if (!isInTargetLocation) {
	        UpdateLocation();
        }
    }

	void SetDirection(bool isLookingRight) {
        this.isLookingRight = isLookingRight;
        if (isLookingRight) {
            animTarget.transform.localScale = new Vector3(1,1,1);
        } else {
            animTarget.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public Vector3 rMax = new Vector3(1.8f,0.8f,0);
    public Vector3 rMin = new Vector3(1.2f,0.2f,0);
    public Vector3 rOff = new Vector3(0.5f,0.5f,0);
    public void SetMiningLaser(bool isOpen) {
        isLaser = isOpen;
        if (isOpen) {
            OpenLaser();
        } else {
            CloseLaser();
        }
    }

    public void ShowMinusOne() {
	    var texts = plusOne.GetComponentsInChildren<TextMesh>();
	    foreach (var text in texts) {
		    text.text = "-1";
	    }
	    ShowText();
    }
    
    public void ShowPlusOne() {
	    var texts = plusOne.GetComponentsInChildren<TextMesh>();
	    foreach (var text in texts) {
		    text.text = "+1";
	    }
	    ShowText();
    }

    void ShowText() {
	    plusOne.transform.localPosition = Vector3.zero;
	    plusOne.SetActive(true);
        
	    StopAllCoroutines();
	    StartCoroutine(TextLoop());
    }

    public float plusOneTimer = 0.5f;
    IEnumerator TextLoop() {
	    var time = 0f;

	    while (time < plusOneTimer) {
		    plusOne.transform.localPosition += Vector3.up *  (Time.deltaTime/10f);

		    time += Time.deltaTime;
		    yield return 0;
	    }

	    DisablePlusOne();
    }

    void DisablePlusOne() {
	    plusOne.SetActive(false);
    }
    
	public Vector3 droneTargetLocation = new Vector3(100, 100, DataHolder.droneLayer);
	public Vector3 droneTargetSmoothingLocation = new Vector3(100, 100, DataHolder.droneLayer);

	float droneSpeed { get { return FactoryDrones.DroneWorldSpeed; } }

	float droneAcceleration { get { return Mathf.Min(FactoryDrones.DroneWorldSpeed * 0.6f, 0.8f); } }

	float droneCurSpeed = 0f;

	public bool isInTargetLocation = false;

	public GameObject droneTargetMarker;

	private float droneMaxSpeed = 0;

	public float miningTimer = 0f;
	public float laserActiveTime = 0.2f;
	public float laserBreakTime = 0.3f;
	public float totalDistance;

	public void FlyToLocation(Position location) {

		droneTargetLocation = location.Vector3(Position.Type.drone);

		droneTargetMarker.transform.position = droneTargetLocation + new Vector3(0.5f, 0.5f);
		droneTargetMarker.SetActive(true);

		SetDirection((droneTargetLocation - transform.position).x > 0);
		if ((droneTargetLocation - transform.position).x > 0) {
			droneTargetLocation += Vector3.left;
		} else {
			droneTargetLocation += Vector3.right;
		}

		totalDistance = Vector3.Distance(transform.position, droneTargetLocation);

		//droneTargetMarker.GetComponentInChildren<SpriteRenderer>().color = Color.yellow;
		isInTargetLocation = false;
	}

	private void UpdateLocation() {

		float distance = Vector3.Distance(transform.position, droneTargetLocation);

		isInTargetLocation = distance < 0.1f;

		transform.position = Vector3.MoveTowards(transform.position, droneTargetSmoothingLocation, droneCurSpeed * Time.deltaTime);

		Vector3 direction = droneTargetLocation - transform.position;
		direction = Vector3.Cross(direction, Vector3.back).normalized;
		droneTargetSmoothingLocation =
			Vector3.MoveTowards(droneTargetSmoothingLocation, droneTargetLocation + direction * (Vector3.Distance(droneTargetSmoothingLocation, droneTargetLocation) / 2f), droneSpeed*2 * Time.deltaTime);
		Debug.DrawLine(droneTargetSmoothingLocation, droneTargetSmoothingLocation + Vector3.up);

		if (totalDistance > 4) {
			if (distance > 2) {
				droneCurSpeed = Mathf.Lerp(droneCurSpeed, droneSpeed, droneAcceleration * Time.deltaTime);
				droneMaxSpeed = droneCurSpeed;
			} else {
				droneCurSpeed = Mathf.Lerp(droneCurSpeed, 0.2f, (droneMaxSpeed / 2) * Time.deltaTime);
			}
		} else {
			if (distance > 0.5f) {
				droneCurSpeed = Mathf.Lerp(droneCurSpeed, droneSpeed, droneAcceleration * Time.deltaTime);
				droneMaxSpeed = droneCurSpeed;
			} else {
				droneCurSpeed = Mathf.Lerp(droneCurSpeed, 0.2f, (droneMaxSpeed / 0.5f) * Time.deltaTime);
			}
		}

		if (isInTargetLocation) {
			droneTargetMarker.SetActive(false);
		}

	}

	private bool laserState = true;
	private float timer = 0f;
	void UpdateMiningLaser() {
		if (laserState) {
			for (int i = 0; i < 10; i++) {
				animCurvys[i].value = 1f + Mathf.Sin(((i * 2f) + Time.time * 5f)) / 4f - i / 50f;
			}

			maCurvy.keys = animCurvys;
			miningLaser.widthCurve = maCurvy;
			

			timer += Time.deltaTime;

			if (timer >= laserActiveTime) {
				CloseLaser();
			}
		} else {
			timer += Time.deltaTime;

			if (timer >= laserBreakTime) {
				OpenLaser();
			}
		}
	}

	void OpenLaser() {
		miningLaser.enabled = true;
		if (!miningParticles.isPlaying) {
			miningParticles.Play();
		}
		
		Vector3 targetPos = new Vector3(Random.Range(rMin.x, rMax.x) * (isLookingRight ? 1 : -1) + rOff.x, Random.Range(rMin.y, rMax.y) + rOff.y, 0);

		miningParticles.transform.position = targetPos + transform.position;
		miningLaser.SetPosition(1, targetPos);

		timer = 0;
		laserState = true;
	}

	void CloseLaser() {
		miningLaser.enabled = false;
		if (miningParticles.isPlaying) {
			miningParticles.Stop();
		}
		
		timer = 0;
		laserState = false;
	}
}
