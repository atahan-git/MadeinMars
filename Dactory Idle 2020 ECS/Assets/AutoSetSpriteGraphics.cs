using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                myRend.SetGraphics(myData.gfxSpriteAnimation, myData.isAnimatedShadow);
				
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
        myRend.DoSpaceLanding(LiftOff);
    }
    
    void LiftOff() {
        Invoke("_LiftOff",5f);
    }

    void _LiftOff() {
        myRend.DoSpaceLiftoff(Landing);
    }

}
