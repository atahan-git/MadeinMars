using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;



/// <summary>
/// A lightweight animated sprite controller.
/// Unity's own animation system is too heavy for simple sprite cycles on every object, so a lightweight controller like this is necessary.
/// </summary>
public class AnimatedSpriteController : MonoBehaviour {

	[SerializeField]
	SpriteAnimationHolder myAnim;

	[SerializeField]
	bool startAtPlay = true;
	public bool randomIndex = true;
	public bool loop = true;

	public bool isPlaying {
		get { return _isPlaying; }
	}
	bool _isPlaying = false;
	bool _smoothStop = false;
	bool _isReverse = false;
	[SerializeField]
    float index = 0;

	SpriteRenderer myRend;
	Image myImg;
	Sprite mySprite;

	public AnimatedSpriteController syncWith;

	bool isSprite;
	// Use this for initialization
	void Awake () {
		myRend = GetComponent<SpriteRenderer> ();
		myImg = GetComponent<Image> ();

		if (startAtPlay && myAnim != null) {
			if (isSprite)
				return;

			_isPlaying = true;
			if (syncWith == null) {
				if (randomIndex) {
					if (myAnim != null) {
						index = Random.Range(0, myAnim.sprites.Length);
					}
				}
			} else {
				index = syncWith.index;
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
		if (_isPlaying) {
			if (myAnim != null) {
				var delta = Time.deltaTime / myAnim.waitSeconds;
				if (syncWith == null) {
					index += _isReverse ? -delta : delta;
				} else {
					index = syncWith.index;
				}

				if (index >= myAnim.sprites.Length || index < 0) {
					index = 0;

					if (!loop) {
						_isPlaying = false;
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

					if (_smoothStop) {
						Stop();
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
		isSprite = false;

		if (syncWith == null) {
			if (randomIndex) {
				if (myAnim != null) {
					index = Random.Range(0, myAnim.sprites.Length);
				}
			}
		} else {
			index = syncWith.index;
		}

		Play();

		Update ();
	}

	public void SetSprite (Sprite _sprite) {
		myRend = GetComponent<SpriteRenderer> ();
		myImg = GetComponent<Image> ();
		_isPlaying = false;
		isSprite = true;

		if (myRend) {
			myRend.sprite = _sprite;
		} else if (myImg) {
			myImg.sprite = _sprite;
		}
	}

	public void Play () {
		_isPlaying = true;
		_smoothStop = false;
	}
	
	public void PlayOnce () {
		_isPlaying = true;
		_smoothStop = true;
	}
	
	public void PlayOnceReverse () {
		_isPlaying = true;
		_smoothStop = true;
	}

	public void Stop() {
		_isPlaying = false;
		_smoothStop = false;

		if (myAnim.stopSprite) {
			myRend.sprite = myAnim.stopSprite;
			index = 0;
		}
	}

	public void SmoothStop() {
		if(index == 0 || !_isPlaying)
			Stop();
		else
			_smoothStop = true;
	}
}
