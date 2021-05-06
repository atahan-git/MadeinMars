using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Holds the sprites for the AnimatedSpriteController, with some relevant information
/// </summary>
[CreateAssetMenu (fileName = "SpriteAnimation", menuName = "SpriteAnimation", order = 3)]
public class SpriteAnimationHolder : ScriptableObject {
	public Sprite [] sprites;
    public float spritePerSecond = 24;

    public Sprite stopSprite;

    public float waitSeconds {
        get {
            return 1f / spritePerSecond;
        }
    }
}
