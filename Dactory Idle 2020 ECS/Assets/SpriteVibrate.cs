using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This was briefly used for some sprite animations. I will keep it in case I need it again
/// Basically makes a sprite vibrate
/// Check \Media\Issue 7\Different art styles.gif
/// </summary>
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
