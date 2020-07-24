using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteGraphicsController : MonoBehaviour {

    public Sprite sprite {
        set {
            ChangeSprite(value);
        }
    }

    public GameObject ShadowPrefab;
    GameObject myShadow;
    public float height = 0.1f;

    SpriteRenderer rend = null;

    public static Vector3 shadowOffset = new Vector3(0.46f, 0.72f, 0.5f);
    // Start is called before the first frame update
    void Start () {
        if (rend == null)
            CreateShadow();
    }

    void CreateShadow () {
        myShadow = Instantiate(ShadowPrefab, transform);
        rend = GetComponent<SpriteRenderer>();
        myShadow.GetComponent<SpriteRenderer>().sprite = rend.sprite;
        myShadow.transform.localPosition = new Vector3(shadowOffset.x * height, shadowOffset.y * height, shadowOffset.z);
    }



    void ChangeSprite (Sprite sp) {
        if (rend == null || myShadow == null)
            CreateShadow();
        rend.sprite = sp;
        myShadow.GetComponent<SpriteRenderer>().sprite = rend.sprite;
    }
}
