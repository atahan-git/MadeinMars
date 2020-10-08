using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteVibrate : MonoBehaviour {
    Vector3 originalPos;
    public Vector2 vibrationSize = new Vector2(0.05f,0.05f);
    public float vibrationSpeed = 0.5f;
    // Start is called before the first frame update
    void Start() {
        originalPos = transform.localPosition;
    }

    private float timer = 0;
    
    void Update() {
        if (timer > vibrationSpeed) {
            timer = 0;
            transform.localPosition = originalPos + new Vector3(Random.Range(-vibrationSize.x,vibrationSize.x),Random.Range(-vibrationSize.y,vibrationSize.y), 0);
        }

        timer += Time.deltaTime;
    }
}
