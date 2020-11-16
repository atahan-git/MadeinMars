using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteGraphicsController : MonoBehaviour {

    public GameObject ShadowPrefab;
    GameObject myShadow;
    public float height = 0.2f;

    SpriteRenderer rend = null;

    public static Vector3 shadowOffset = new Vector3(0.72f, 0.72f, 0.5f);

    private AnimatedSpriteController anim = null;

    private GameObject instantiatedPrefab = null;
    // Start is called before the first frame update
    void Start () {
        /*if (rend == null)
            CreateShadow();*/
    }

    public void SetGraphics(Sprite sprite) {
        SetGraphics(sprite,sprite, height);
    }
    
    public void SetGraphics(Sprite sprite, Sprite shadowSprite) {
        SetGraphics(sprite,shadowSprite, 0);
    }
    
    void SetGraphics(Sprite sprite, Sprite shadowSprite, float height) {
        Clear();
        if (rend == null || myShadow == null)
            CreateShadow(height);
        rend.sprite = sprite;
        myShadow.GetComponent<SpriteRenderer>().sprite = shadowSprite;
    }

    public void SetGraphics(SpriteAnimationHolder animationHolder) {
        Clear();
        if (rend == null || myShadow == null)
            CreateShadow(height);
        if (anim == null)
            anim = gameObject.AddComponent<AnimatedSpriteController>();
        
        anim.SetAnimation(animationHolder);
        myShadow.GetComponent<SpriteRenderer>().sprite = animationHolder.sprites[0];
    }

    public void SetGraphics(GameObject prefab) {
        Clear();
        instantiatedPrefab = Instantiate(prefab, transform);
    }

    public void Clear() {
        if (instantiatedPrefab) {
            Destroy(instantiatedPrefab);
        }
        if(anim && anim.isPlaying)
            anim.Stop();
        if (rend)
            rend.sprite = null;
    }

    public GameObject spaceLandingPrefab;
    private GameObject landingFx;
    private GameObject landingFloor;
    
    private Vector3 originalPos;
    public void DoSpaceLanding() {
        originalPos = transform.localPosition;

        landingFx = Instantiate(spaceLandingPrefab, transform);
        landingFx.transform.localPosition = Vector3.zero;
        landingFloor = landingFx.transform.Find("Landing Floor").gameObject;
        landingFloor.transform.SetParent(transform.parent);
        
        StartCoroutine(SpaceLanding());
    }

    private const float SpaceLandingHeight = 100;
    private const float SpaceLandingStartSpeed = -10;
    private const float SpaceLandingDeceleration = (SpaceLandingStartSpeed*SpaceLandingStartSpeed)/(2*(SpaceLandingHeight));
    //0 = u^2 + 2as
    //u^2 = 2as
    //u^2/2s = a
    
    IEnumerator SpaceLanding() {
        float curHeight = SpaceLandingHeight;
        float curSpeed = SpaceLandingStartSpeed;
        float acceleration = SpaceLandingDeceleration;
        while (curHeight > 0) {
            SetHeight(curHeight);
            curHeight += curSpeed * Time.deltaTime;
            curSpeed += acceleration * Time.deltaTime;
            
            yield return 0;
        }
        
        SetHeight(0);

        landingFx.transform.Find("Landing Rocket").GetComponent<ParticleSystem>().Stop();
        Invoke("DestroyLandingFx",10f);
        Invoke("DestroyLandingFloor",1f);
        yield break;
    }

    void DestroyLandingFloor() {
        Destroy(landingFloor);
    }
    void DestroyLandingFx() {
        Destroy(landingFx);
        Destroy(landingFloor);
    }

    void SetHeight(float offset) {
        transform.localPosition = originalPos + new Vector3(0,1f,-1) * offset;
        if(myShadow)
            myShadow.transform.localPosition =  new Vector3(shadowOffset.x*(offset+(height/2)), shadowOffset.y*height, shadowOffset.z);
    }

    void CreateShadow (float height) {
        myShadow = Instantiate(ShadowPrefab, transform);
        rend = GetComponent<SpriteRenderer>();
        myShadow.GetComponent<SpriteRenderer>().sprite = rend.sprite;
        myShadow.transform.localPosition = new Vector3(shadowOffset.x * (height/2f), shadowOffset.y * height, shadowOffset.z);
    }
}
