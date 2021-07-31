using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxCamera : MonoBehaviour {
    public static Vector3 speedDelta;
    
    public Vector3 lastFramePos = Vector3.zero;
    // Update is called once per frame
    void Update() {
        speedDelta = transform.position - lastFramePos;
        lastFramePos = transform.position;
    }
}
