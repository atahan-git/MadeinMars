using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxLayer : MonoBehaviour {
   
   [Header("The bigger this is 'closer' the object becomes")]
   public float speedMultiplier = 0.2f;

   private Renderer renderer;
   private Vector2 prevOffset;

   void Start () {
      renderer = GetComponent<Renderer> ();
   }

   void LateUpdate () {
      Vector2 offset = prevOffset + new Vector2 (ParallaxCamera.speedDelta.x, ParallaxCamera.speedDelta.y) * speedMultiplier;
      renderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
      prevOffset = offset;
   }
}
