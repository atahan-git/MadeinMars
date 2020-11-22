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

    Vector3 originalPos;
    // Start is called before the first frame update
    void Start () {
        /*if (rend == null)
            CreateShadow();*/
    }

    public void SetGraphics(Sprite sprite) {
        SetGraphics(sprite,sprite, height);
    }
    
    public void SetGraphics(Sprite sprite, Sprite shadowSprite) {
        SetGraphics(sprite, shadowSprite, 0);
    }
    
    void SetGraphics(Sprite sprite, Sprite shadowSprite, float height) {
        Clear();
        if (rend == null || myShadow == null)
            CreateShadow(height);
        rend.sprite = sprite;
        myShadow.GetComponent<SpriteRenderer>().sprite = shadowSprite;
        originalPos = transform.localPosition;
    }

    public void SetGraphics(SpriteAnimationHolder animationHolder, bool isShadowAnimated) {
        Clear();
        if (rend == null || myShadow == null)
            CreateShadow(height);
        if (anim == null)
            anim = gameObject.AddComponent<AnimatedSpriteController>();
        
        anim.SetAnimation(animationHolder);
        if(!isShadowAnimated)
            myShadow.GetComponent<SpriteRenderer>().sprite = animationHolder.sprites[0];
        else {
            var shadAnim = myShadow.AddComponent<AnimatedSpriteController>();
            shadAnim.SetAnimation(animationHolder);
            shadAnim.syncWith = anim;
        }
        originalPos = transform.localPosition;
    }

    public void SetGraphics(GameObject prefab) {
        Clear();
        instantiatedPrefab = Instantiate(prefab, transform);
        originalPos = transform.localPosition;
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

    public delegate void SpaceLandingCallback();
    public void DoSpaceLanding(SpaceLandingCallback callback) {
        StartCoroutine(SpaceLanding(true, callback));
    }
    
    public void DoSpaceLiftoff(SpaceLandingCallback callback) {
        StartCoroutine(SpaceLanding(false, callback));
    }

    private const float SpaceLandingHeightMultiplier = 2f/3f;
    private const float SpaceLandingHeight = 100f * SpaceLandingHeightMultiplier * SpaceLandingHeightMultiplier;
    private const float SpaceLandingStartSpeed = -10f * SpaceLandingHeightMultiplier;
    private const float SpaceLandingDeceleration = (SpaceLandingStartSpeed*SpaceLandingStartSpeed)/(2f*(SpaceLandingHeight));
    //0 = u^2 + 2as
    //u^2 = 2as
    //u^2/2s = a

    private bool launchInProgress = false;
    
    IEnumerator SpaceLanding(bool isLanding, SpaceLandingCallback callback) {
        while (launchInProgress) {
            yield return null;
        }

        launchInProgress = true;

        var landingFx = Instantiate(spaceLandingPrefab, transform);
        landingFx.transform.localPosition = Vector3.zero;
        var landingFloor = landingFx.transform.Find("Landing Floor").gameObject;
        landingFloor.transform.SetParent(transform.parent);
        landingFloor.transform.localPosition = originalPos + Vector3.down*2;
        
        // If we are landing, start from the space and go down.
        // If we are lifting off, start from zero and go up.
        float curHeight = isLanding ? SpaceLandingHeight : 0;
        float curSpeed = isLanding? SpaceLandingStartSpeed : 0;
        float acceleration = isLanding ? SpaceLandingDeceleration : SpaceLandingDeceleration;
        while (isLanding ? curHeight > 0 : curHeight < SpaceLandingHeight) {
            SetHeight(originalPos, curHeight);
            curHeight += curSpeed * Time.deltaTime;
            curSpeed += acceleration * Time.deltaTime;
            
            yield return null;
        }
        
        SetHeight(originalPos, isLanding ? 0 : SpaceLandingHeight);

        landingFx.transform.Find("Landing Rocket").GetComponent<ParticleSystem>().Stop();
        StartCoroutine(DestroyLandingFx(landingFx, landingFloor));
        
        
        yield return new WaitForSeconds(1f);

        launchInProgress = false;
        callback?.Invoke();
    }

    IEnumerator DestroyLandingFx(GameObject fx, GameObject floor) {
        yield return new WaitForSeconds(10f);
        
        Destroy(fx);
        Destroy(floor);
    }

    void SetHeight(Vector3 originalPos, float offset) {
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
