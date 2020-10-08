using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedSpriteController : MonoBehaviour {

	[SerializeField]
	SpriteAnimationHolder myAnim;

    public bool startAtPlay = true;
	public bool randomIndex = true;
	public bool loop = true;
    bool isPlaying = false;
	[SerializeField]
    float index = 0;

	SpriteRenderer myRend;
	Image myImg;
	Sprite mySprite;

	bool isSprite;
	// Use this for initialization
	void Awake () {
		myRend = GetComponent<SpriteRenderer> ();
		myImg = GetComponent<Image> ();

		if (startAtPlay && myAnim != null) {
			if (isSprite)
				return;

			isPlaying = true;
			if (randomIndex) {
				if (myAnim != null) {
					index = Random.Range (0, myAnim.sprites.Length);
				}
			}

			mySprite = myAnim.sprites[(int)index];

			if (myRend) {
				myRend.sprite = mySprite;
			} else if (myImg) {
				myImg.sprite = mySprite;
			}

		} else {
			if (myRend)
				mySprite = myRend.sprite;
			else if (myImg)
				mySprite = myImg.sprite;
		}
	}

	void Update () {
		if (isPlaying) {
			if (myAnim != null) {
				index += Time.deltaTime / myAnim.waitSeconds;
				if (index >= myAnim.sprites.Length) {
					index = 0;

					if (!loop) {
						isPlaying = false;
						mySprite = null;
						if (myRend) {
							myRend.sprite = mySprite;
						} else if (myImg) {
							myImg.sprite = mySprite;
						}
						if (myImg) {
							myImg.enabled = myImg.sprite != null;
						}
						return;
					}
				}
				mySprite = myAnim.sprites [(int)index];
			}

			if (myRend) {
				myRend.sprite = mySprite;
			} else if (myImg) {
				myImg.sprite = mySprite;
			} 
		}

		if (myImg) {
			myImg.enabled = myImg.sprite != null;
		}
	}

	public void SetAnimation (SpriteAnimationHolder _anim) {
		myRend = GetComponent<SpriteRenderer> ();
		myImg = GetComponent<Image> ();
		myAnim = _anim;
		isPlaying = true;
		isSprite = false;

		if (randomIndex) {
			if (myAnim != null) {
				index = Random.Range (0, myAnim.sprites.Length);
			}
		}

		Update ();
	}

	public void SetSprite (Sprite _sprite) {
		myRend = GetComponent<SpriteRenderer> ();
		myImg = GetComponent<Image> ();
		isPlaying = false;
		isSprite = true;

		if (myRend) {
			myRend.sprite = _sprite;
		} else if (myImg) {
			myImg.sprite = _sprite;
		}
	}

	void Play () {
		isPlaying = true;
	}
}
