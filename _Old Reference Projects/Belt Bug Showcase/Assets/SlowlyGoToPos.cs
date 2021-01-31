using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowlyGoToPos : MonoBehaviour
{
    public bool isProcessed = false;
    public bool isMarked = false;

    public GameObject target;
    public float speed = 20f;

    public GameObject marking;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = target.transform.position;
        marking.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        marking.SetActive(isMarked);
        transform.position = Vector3.Lerp(transform.position, target.transform.position, speed * Time.deltaTime);
    }
}
