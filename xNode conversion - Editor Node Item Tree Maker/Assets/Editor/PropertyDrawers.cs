using UnityEditor;
using UnityEngine;


// IngredientDrawerUIE
//[CustomPropertyDrawer(typeof(CountedItemNode))]
public class ShopItemUIE : PropertyDrawer {
    
	// Draw the property inside the given rect
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		EditorGUI.BeginProperty(position, label, property);

		// Draw label
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		// Don't make child fields be indented
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		// Calculate rects
		var itemRect = new Rect(position.x, position.y, position.width - 30, position.height);
		var countRect = new Rect(position.x + position.width - 30, position.y, 30, position.height);

		// Draw fields - passs GUIContent.none to each so they are drawn without labels
		EditorGUI.LabelField(itemRect, property.FindPropertyRelative("itemNode").FindPropertyRelative("myItem").FindPropertyRelative("uniqueName").stringValue);
		EditorGUI.PropertyField(countRect, property.FindPropertyRelative("count"), GUIContent.none);

		// Set indent back to what it was
		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}
}