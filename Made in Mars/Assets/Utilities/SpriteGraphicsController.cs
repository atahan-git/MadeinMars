﻿using System.Collections;
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
    void Start() {
        /*if (rend == null)
            CreateShadow();*/
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
        }

        if (anim && anim.isPlaying)
            anim.Stop();
        if (rend)
            rend.sprite = null;
    }

    public GameObject spaceLandingPrefab;

    public delegate void SpaceLandingCallback();

    public void DoSpaceLanding(SpaceLandingCallback callback, float xDisp = 0) {
        StopCoroutine("SpaceLanding");
        StartCoroutine(SpaceLanding(true, callback, xDisp));
    }

    public void DoSpaceLiftoff(SpaceLandingCallback callback, float xDisp = 0) {
        StopCoroutine("SpaceLanding");
        StartCoroutine(SpaceLanding(false, callback, xDisp));
    }

    private const float SpaceLandingHeightMultiplier = 2f / 3f;
    private const float SpaceLandingHeight = 100f * SpaceLandingHeightMultiplier * SpaceLandingHeightMultiplier;
    private const float SpaceLandingStartSpeed = -50f * SpaceLandingHeightMultiplier;

    private const float SpaceLandingDeceleration = (SpaceLandingStartSpeed * SpaceLandingStartSpeed) / (2f * (SpaceLandingHeight));
    public const float SpaceLandingTime = -SpaceLandingStartSpeed / SpaceLandingDeceleration;
//0 = u^2 + 2as
//u^2 = 2as
//u^2/2s = a

// 0 = u + at
// t = -u/a


    private float landingLegOpenDistance = 2f;

    IEnumerator SpaceLanding(bool isLanding, SpaceLandingCallback callback, float xDisp) {
        var landingFx = Instantiate(spaceLandingPrefab, transform);
        landingFx.transform.localPosition = Vector3.zero;
        var landingFloor = landingFx.transform.Find("Landing Floor").gameObject;
        landingFloor.transform.SetParent(transform.parent);
        landingFloor.transform.localPosition = originalPos + Vector3.down * 2;

// If we are landing, start from the space and go down.
// If we are lifting off, start from zero and go up.
        float curHeight = isLanding ? SpaceLandingHeight : 0;
        float curDisp = isLanding ? xDisp : -xDisp;
        float dispSpeed = -(curDisp / SpaceLandingTime );
        float curSpeed = isLanding ? SpaceLandingStartSpeed : 0;
        float acceleration = isLanding ? SpaceLandingDeceleration : SpaceLandingDeceleration;
        while (isLanding ? curHeight >= 0 : curHeight < SpaceLandingHeight) {
            SetHeight(originalPos + Vector3.left*curDisp, curHeight);
            curHeight += curSpeed * Time.deltaTime;
            curSpeed += acceleration * Time.deltaTime;
            curDisp += dispSpeed * Time.deltaTime;

            yield return null;
        }

        SetHeight(isLanding? originalPos : originalPos+ Vector3.left*curDisp, isLanding ? 0 : SpaceLandingHeight);

        landingFx.transform.Find("Landing Rocket").GetComponent<ParticleSystem>().Stop();
        StartCoroutine(DestroyLandingFx(landingFx, landingFloor));


        yield return new WaitForSeconds(1f);

        
        callback?.Invoke();
    }

    IEnumerator DestroyLandingFx(GameObject fx, GameObject floor) {
        yield return new WaitForSeconds(2f);

        Destroy(fx);
        Destroy(floor);
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
