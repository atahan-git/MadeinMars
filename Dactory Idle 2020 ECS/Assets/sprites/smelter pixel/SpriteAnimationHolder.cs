using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "SpriteAnimation", menuName = "SpriteAnimation", order = 3)]
public class SpriteAnimationHolder : ScriptableObject {
	public Sprite [] sprites;
    public float spritePerSecond = 24;

    public float waitSeconds {
        get {
            return 1f / spritePerSecond;
        }
    }
}
