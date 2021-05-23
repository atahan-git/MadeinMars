using UnityEngine;
using System.Collections;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(DialogTree))]
public class DialogTreeEditor : Editor {

	public override void OnInspectorGUI () {
		DrawDefaultInspector ();
		DialogTree myTarget = (DialogTree)target;


		if (GUILayout.Button ("Add Dialog")) {
			GameObject myPoint = (GameObject)Instantiate (myTarget.dialogPrefab, myTarget.transform.position, myTarget.transform.rotation);
			myPoint.transform.parent = myTarget.transform;
		}

		if (GUILayout.Button ("Save Dialog Tree Asset")) {
			DialogTreeAsset asset = myTarget.myAsset;
			bool shouldCreateNew = false;
			if (asset == null) {
				asset = ScriptableObject.CreateInstance<DialogTreeAsset> ();
				shouldCreateNew = true;
			}
			asset.dialogs = new DialogObject[myTarget.dialogs.Length];
			asset.name = myTarget.dialogName;

			for (int i = 0; i < myTarget.dialogs.Length; i++) {
				if (myTarget.dialogs[i] != null) {
					asset.dialogs[i] = new DialogObject {
						tag = myTarget.dialogs[i].myTag,
						text = myTarget.dialogs[i].text,
						clearImage = myTarget.dialogs[i].clearImage,
						image = myTarget.dialogs[i].image,
						bigSpriteAction = myTarget.dialogs[i].bigSpriteAction,
						bigSpriteSlot = myTarget.dialogs[i].bigSpriteSlot,
						bigSprite = myTarget.dialogs[i].bigSprite,
						delay = myTarget.dialogs[i].delay,
						breakAutoChain = myTarget.dialogs[i].breakAutoChain
					};
				}
			}

			if (shouldCreateNew) {
				AssetDatabase.CreateAsset (asset, "Assets/-" + myTarget.dialogName + "- Dialog.asset");
				AssetDatabase.SaveAssets ();
			}

			EditorUtility.FocusProjectWindow ();

			Selection.activeObject = asset;

			myTarget.myAsset = asset;
		}

		if (GUILayout.Button ("Load Dialog Tree Asset")) {
			if (myTarget.myAsset != null) {
				Dialog[] myChild = myTarget.GetComponentsInChildren<Dialog> ();
				for (int i = 0; i < myChild.Length; i++) {
					DestroyImmediate (myChild[i].gameObject);
				}

				foreach (DialogObject dia in myTarget.myAsset.dialogs) {
					GameObject myPoint = (GameObject)Instantiate (myTarget.dialogPrefab, myTarget.transform.position, myTarget.transform.rotation);
					myPoint.transform.parent = myTarget.transform;
					Dialog myDia = myPoint.GetComponent<Dialog> ();
					myDia.myTag = dia.tag;
					myDia.text = dia.text;
					myDia.clearImage = dia.clearImage;
					myDia.image = dia.image;
					myDia.bigSpriteAction = dia.bigSpriteAction;
					myDia.bigSpriteSlot = dia.bigSpriteSlot;
					myDia.bigSprite = dia.bigSprite;
					myDia.delay = dia.delay;
					myDia.breakAutoChain = dia.breakAutoChain;
				}

				//myTarget.myAsset = null;
				myTarget.dialogName = myTarget.myAsset.name;
				myTarget.updateAssetRealtime = true;
			}
		}


		if (GUILayout.Button ("Reset Dialog Tree")) {
			myTarget.myAsset = null;
			Dialog[] myChild = myTarget.GetComponentsInChildren<Dialog> ();
			for (int i = 0; i < myChild.Length; i++) {
				DestroyImmediate (myChild[i].gameObject);
			}
			myTarget.dialogName = "New Dialog";
			myTarget.updateAssetRealtime = false;
		}
	}
}
