using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Empty DialogTree", menuName = "DialogTree", order = 4)]
public class DialogTreeAsset : ScriptableObject {
	public bool isSkipable = true;
	public DialogObject[] dialogs = new DialogObject[0];
}


[System.Serializable]
public class DialogObject {
	public string tag;

	[TextArea]
	[Tooltip("<delay='waitSeconds'>,\n <wait='click/enabled'>,\n <give='itemType'-'itemId'>,\n <trigger='commandID'>,\n <triggerChoice='questChoiceID'>")]
	public string text;
	public bool clearImage = true;
	public Sprite image;

	public DialogDisplayer.BigSpriteAction[] bigSpriteAction;
	public DialogDisplayer.BigSpriteSlots[] bigSpriteSlot;
	public Sprite[] bigSprite;

	public float delay = 0.5f;
	public bool breakAutoChain = false;
}