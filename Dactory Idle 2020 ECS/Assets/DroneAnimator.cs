using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DroneAnimator : MonoBehaviour {

    public GameObject animTarget;

    private bool isLookingRight = false;
    
    public AnimationCurve updowncurve;

    public float speed = 0.2f;
    public float magnitude = 0.5f;

    public Vector3 gfxOffset;

    public LineRenderer miningLaser;
    private bool isLaser = false;
    
    public AnimationCurve maCurvy = new AnimationCurve ();
    Keyframe[] animCurvys = new Keyframe[10];

    public GameObject plusone;

    public ParticleSystem miningParticles;
    private void Start() {
        DisablePlusOne();
        SetMiningLaser(false);
        for (int i = 0; i < 10; i++) {
            animCurvys[i].time = Mathf.Lerp (0, 1, i/10f);
        }
    }

    void Update() {
        animTarget.transform.localPosition =
            new Vector3(0, magnitude * updowncurve.Evaluate((Time.time * speed) % 1), 0) + gfxOffset;

        if (isLaser) {
            for (int i = 0; i < 10; i++) {
                animCurvys[i].value = 1f + Mathf.Sin(((i*2f) + Time.time * 5f)) / 4f - i / 50f;
            }

            maCurvy.keys = animCurvys;
            miningLaser.widthCurve = maCurvy;
        }
    }

    public void SetDirection(bool isLookingRight) {
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
            miningLaser.enabled = true;
            if (!miningParticles.isPlaying) {
                miningParticles.Play();
            }

            Vector3 targetPos = new Vector3(Random.Range(rMin.x, rMax.x) * (isLookingRight ? 1 : -1) + rOff.x, Random.Range(rMin.y, rMax.y) + rOff.y, 0);
            
            miningParticles.transform.position = targetPos+transform.position;
            miningLaser.SetPosition(1, targetPos) ;
        } else {
            miningLaser.enabled = false;
            if (miningParticles.isPlaying) {
                miningParticles.Stop();
            }
        }
    }

    public void ShowPlusOne() {
        plusone.SetActive(true);
        Invoke("DisablePlusOne",0.5f);
    }

    void DisablePlusOne() {
        plusone.SetActive(false);
    }
}
