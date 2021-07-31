using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedObjectDestructor : MonoBehaviour {

    public float time = 2f;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroySelf", time);
    }

    void DestroySelf() {
        Destroy(gameObject);
    }
}
