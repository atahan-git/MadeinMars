using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardScroll : MonoBehaviour {

    public float xSpeed = 200f;
    public float ySpeed = 100f;
    

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        transform.Translate(-new Vector3(xSpeed*horizontal, ySpeed*vertical, 0) * Time.deltaTime);
    }
}
