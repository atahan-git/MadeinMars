using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BeltGfx : MonoBehaviour {

	public BeltGfxLookupTable myTable;

	/*public GameObject inputObj;
	public GameObject outputObj;
	public GameObject baseObj;*/

	bool[] oldInputs = new bool[4];
	bool[] oldOutputs = new bool[4];

	SpriteRenderer myRend;

	const bool autoUpdate = false;

#if UNITY_EDITOR
	void Update () {
		if(autoUpdate)
		if (Application.isEditor && !Application.isPlaying)
			UpdateGraphics(GetComponent<BeltObject>().beltInputs, GetComponent<BeltObject>().beltOutputs);

	}
#endif

	public void UpdateGraphics (bool[] inputs, bool[] outputs) {
		bool isDirty = false;
		for (int i = 0; i < 4; i++) {
			if (inputs[i] != oldInputs[i] || outputs[i] != oldOutputs[i]) {
				isDirty = true;
				break;
			}
		}

		if (isDirty) {
			while (transform.childCount != 0) {
				if (Application.isEditor)
					DestroyImmediate(transform.GetChild(0).gameObject);
				else
					Destroy(transform.GetChild(0).gameObject);
			}

			if (myRend == null) {
				myRend = GetComponent<SpriteRenderer>();
				if (myRend == null) {
					gameObject.AddComponent<SpriteRenderer>();
				}
			}

			if (myTable != null) {
				Sprite mySprite = null;
				if (myTable.mapping.TryGetValue(BeltGfxLookupTable.beltToKey(GetComponent<BeltObject>()), out mySprite)) {
					myRend.sprite = mySprite;
				} 
			}

			inputs.CopyTo(oldInputs, 0);
			outputs.CopyTo(oldOutputs, 0);
		}
	}

	// The old way
	/*while (transform.childCount != 0) {
				if (Application.isEditor)
					DestroyImmediate(transform.GetChild(0).gameObject);
				else
					Destroy(transform.GetChild(0).gameObject);
			}


			((GameObject)Instantiate(baseObj, transform)).transform.localPosition = new Vector3(0, 0, 0);

			for (int i = 0; i < 4; i++) {
				Vector2 offSet = new Vector2(0, 0);
				float off = 0.25f;

				switch (i) {
				case 0:
					offSet.y = off;
					break;
				case 1:
					offSet.x = off;
					break;
				case 2:
					offSet.y = -off;
					break;
				case 3:
					offSet.x = -off;
					break;
				}

				if (inputs[i])
					((GameObject)Instantiate(inputObj, transform)).transform.localPosition = new Vector3(offSet.x, offSet.y, 0.1f);
				if (outputs[i])
					((GameObject)Instantiate(outputObj, transform)).transform.localPosition = new Vector3(offSet.x, offSet.y, 0.1f);
			}*/
}
