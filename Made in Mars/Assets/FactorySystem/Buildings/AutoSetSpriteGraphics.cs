﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This is just for the menu, to set the sprites of the assets in the menu manually.
/// </summary>
[RequireComponent(typeof(SpriteGraphicsController))]
public class AutoSetSpriteGraphics : MonoBehaviour {

    public BuildingData myData;

    public bool SpaceLanding = false;
    // Start is called before the first frame update
    private SpriteGraphicsController myRend;
    void Start() {

        myRend = GetComponent<SpriteGraphicsController>();
        switch (myData.gfxType) {
            case BuildingData.BuildingGfxType.SpriteBased:
                myRend.SetGraphics(myData.gfxSprite, myData.gfxShadowSprite != null? myData.gfxShadowSprite : myData.gfxSprite);
                break;
            case BuildingData.BuildingGfxType.AnimationBased:
                if (myData.gfxShadowAnimation == null) {
                    myRend.SetGraphics(myData.gfxSpriteAnimation, myData.copySpriteAnimationToShadow);
                } else {
                    myRend.SetGraphics(myData.gfxSpriteAnimation, myData.gfxShadowAnimation);
                }
				
                break;
            case BuildingData.BuildingGfxType.PrefabBased:
                myRend.SetGraphics(myData.gfxPrefab);
                break;
        }

        if (SpaceLanding)
            _Landing();
    }


    void Landing() {
        Invoke("_Landing",1f);
    }

    void _Landing() {
        myRend.DoSpaceLanding(LiftOff, myData.spaceLandingXDisp/3f, myData.gfxSpriteAnimation);
    }
    
    void LiftOff() {
        Invoke("_LiftOff",5f);
    }

    void _LiftOff() {
        myRend.DoSpaceLiftoff(Landing, myData.spaceLandingXDisp/3f, myData.gfxSpriteAnimation);
    }

}
