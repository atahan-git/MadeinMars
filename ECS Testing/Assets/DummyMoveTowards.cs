using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyMoveTowards : MonoBehaviour
{

    public Transform target;

    public float speed = 0.4f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float deltaMovementWithDeltaTime = speed * Time.deltaTime;
        Vector3 direction = target.position - transform.position;
        float magnitude = (direction.magnitude);
        if (magnitude <= deltaMovementWithDeltaTime || magnitude == 0f) {
            transform.position = target.position;
        } else {
            transform.position = transform.position + direction / magnitude * deltaMovementWithDeltaTime;
        }
    }
}
