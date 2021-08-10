using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Controls the graphics for all the world objects.
/// Creates the shadows, and sets the correct sprite/animation mode accordingly
/// Also does the orbital landing thing.
/// </summary>
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
    void Awake() {
        if (rend == null)
            rend = GetComponent<SpriteRenderer>();
    }

    // 00C8FF
    private Color buildingPreviewColor = new Color(0 , 200/255f, 255/255f, 150/255f);
    private Color buildingMarkedForDestructionColor = new Color(255/255f , 6/255f, 0, 150/255f);

    public enum  BuildState {
        construction, built, destruction
    }
    public void SetBuildState(BuildState state) {
        switch (state) {
            case BuildState.construction:
                rend.color = buildingPreviewColor;
                break;
            case BuildState.built:
                rend.color = Color.white;
                break;
            case BuildState.destruction:
                rend.color = buildingMarkedForDestructionColor;
                break;
        }
    }

    public void SetGraphics(Sprite sprite) {
        SetGraphics(sprite, sprite, height);
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
        if (!isShadowAnimated)
            myShadow.GetComponent<SpriteRenderer>().sprite = animationHolder.sprites[0];
        else {
            var shadAnim = myShadow.AddComponent<AnimatedSpriteController>();
            shadAnim.SetAnimation(animationHolder);
            shadAnim.syncWith = anim;
        }

        originalPos = transform.localPosition;
    }
    
    public void SetGraphics(SpriteAnimationHolder animationHolder, SpriteAnimationHolder shadowAnimation) {
        Clear();
        if (rend == null || myShadow == null)
            CreateShadow(height);
        if (anim == null)
            anim = gameObject.AddComponent<AnimatedSpriteController>();

        anim.SetAnimation(animationHolder);
        
        var shadAnim = myShadow.AddComponent<AnimatedSpriteController>();
        shadAnim.SetAnimation(shadowAnimation);
        shadAnim.syncWith = anim;
        

        originalPos = transform.localPosition;
    }

    public void SetGraphics(GameObject prefab) {
        Clear();
        instantiatedPrefab = Instantiate(prefab, transform);
        rend = instantiatedPrefab.GetComponentInChildren<SpriteRenderer>();
        originalPos = transform.localPosition;
    }

    public void Clear() {
        if (instantiatedPrefab) {
            Destroy(instantiatedPrefab);
            rend = GetComponent<SpriteRenderer>();
        }

        if (myShadow != null)
            Destroy(myShadow);

        var animatedSprites = GetComponentsInChildren<AnimatedSpriteController>();

        for (int i = 0; i < animatedSprites.Length; i++) {
            Destroy(animatedSprites[i]);
        }
        
        
        if (rend)
            rend.sprite = null;
        
        
        if(landingFx != null)
            Destroy(landingFx);
        if(landingFloor != null)
            Destroy(landingFloor);
    }

    public GameObject spaceLandingPrefab;

    public delegate void SpaceLandingCallback();

    public void DoSpaceLanding(SpaceLandingCallback callback, float xDisp, SpriteAnimationHolder landingLegsAnim) {
        StopCoroutine("SpaceLanding");
        StartCoroutine(SpaceLanding(true, callback, xDisp, landingLegsAnim));
    }

    public void DoSpaceLiftoff(SpaceLandingCallback callback, float xDisp, SpriteAnimationHolder landingLegsAnim) {
        StopCoroutine("SpaceLanding");
        StartCoroutine(SpaceLanding(false, callback, xDisp, landingLegsAnim));
    }

    private const float SpaceLandingHeightMultiplier = 5f / 3f; //2f/3f
    private const float SpaceLandingHeight = 100f * SpaceLandingHeightMultiplier * SpaceLandingHeightMultiplier;
    private const float SpaceLandingStartSpeed = -50f * SpaceLandingHeightMultiplier;

    private const float SpaceLandingDeceleration = (SpaceLandingStartSpeed * SpaceLandingStartSpeed) / (2f * (SpaceLandingHeight));
    public const float SpaceLandingTime = -SpaceLandingStartSpeed / SpaceLandingDeceleration;
//v = u^2 + 2as
//v = 0
//u^2 = 2as
//u^2/2s = a

// v = u + at
// v = 0
// t = -u/a

    private const float landingLegOpenDistance = 15f;


    private GameObject landingFx;
    private GameObject landingFloor;
    IEnumerator SpaceLanding(bool isLanding, SpaceLandingCallback callback, float xDisp, SpriteAnimationHolder landingLegsAnim) {
        var startTime = Time.realtimeSinceStartup;
        
        if(landingFx != null)
            Destroy(landingFx);
        landingFx = Instantiate(spaceLandingPrefab, transform);
        landingFx.transform.localPosition = Vector3.zero;
        if(landingFloor != null)
            Destroy(landingFloor);
        landingFloor = landingFx.transform.Find("Landing Floor").gameObject;
        landingFloor.transform.SetParent(transform.parent);
        landingFloor.transform.localPosition = originalPos + Vector3.down * 2;

        Sprite originalSprite = rend.sprite;
        var spriteIndex = 0f;
        if (landingLegsAnim != null) {
            spriteIndex = !isLanding ? 0 : landingLegsAnim.sprites.Length-1;
            rend.sprite = landingLegsAnim.sprites[(int)spriteIndex];
        }
        

// If we are landing, start from the space and go down.
// If we are lifting off, start from zero and go up.
        float curHeight = isLanding ? SpaceLandingHeight : 0;
        float curDisp = isLanding ? xDisp : 0;
        float dispSpeed = isLanding ? -(xDisp / (SpaceLandingTime/2) ) : 0;
        float curSpeed = isLanding ? SpaceLandingStartSpeed : 0;
        float acceleration = isLanding ? SpaceLandingDeceleration : SpaceLandingDeceleration;
        float dispAcceleration = 0;
        if (xDisp != 0) {
            dispAcceleration = ((xDisp / (SpaceLandingTime/2f) ) * (xDisp / (SpaceLandingTime/2f) )) / (2 * xDisp);
            dispAcceleration *= isLanding ? 1 : -1;
        } 

        while (isLanding ? curHeight >= 0 : curHeight < SpaceLandingHeight) {
            SetHeight(originalPos + Vector3.left*curDisp, curHeight);
            curHeight += curSpeed * Time.deltaTime;
            curSpeed += acceleration * Time.deltaTime;
            curDisp += dispSpeed * Time.deltaTime;
            dispSpeed += dispAcceleration * Time.deltaTime;

            if (landingLegsAnim != null) {
                if (curHeight < landingLegOpenDistance) {
                    var delta = Time.deltaTime / landingLegsAnim.waitSeconds;
                    spriteIndex += delta * (!isLanding ? +1 : -1);
                    if (spriteIndex > landingLegsAnim.sprites.Length) {
                        spriteIndex = landingLegsAnim.sprites.Length - 1;
                    } else if (spriteIndex <= 0) {
                        spriteIndex = 0;
                    }

                    rend.sprite = landingLegsAnim.sprites[(int) spriteIndex];
                }
            }

            yield return null;
        }

        SetHeight(isLanding? originalPos : originalPos+ Vector3.left*curDisp, isLanding ? 0 : SpaceLandingHeight);

        if(landingFx != null)
            landingFx.transform.Find("Landing Rocket").GetComponent<ParticleSystem>().Stop();
        StartCoroutine(DestroyLandingFx(landingFx, landingFloor));

        rend.sprite = originalSprite;

        var realLandingTime = Time.realtimeSinceStartup - startTime;
        Debug.Log($"Real Landing time {realLandingTime}, CalculatedLandingTime {SpaceLandingTime}");

        yield return new WaitForSeconds(1f);

        
        callback?.Invoke();
    }

    IEnumerator DestroyLandingFx(GameObject landingFx, GameObject landingFloor) {
        yield return new WaitForSeconds(15f);

        Destroy(landingFx);
        Destroy(landingFloor);
    }

    void SetHeight(Vector3 originalPos, float offset) {
        transform.localPosition = originalPos + new Vector3(0, 1f, -1) * offset;
        if (myShadow)
            myShadow.transform.localPosition = new Vector3(shadowOffset.x * (offset + (height / 2)), shadowOffset.y * height, shadowOffset.z);
    }

    void CreateShadow(float height) {
        myShadow = Instantiate(ShadowPrefab, transform);
        rend = GetComponent<SpriteRenderer>();
        myShadow.GetComponent<SpriteRenderer>().sprite = rend.sprite;
        myShadow.transform.localPosition = new Vector3(shadowOffset.x * (height / 2f), shadowOffset.y * height, shadowOffset.z);
    }
}
