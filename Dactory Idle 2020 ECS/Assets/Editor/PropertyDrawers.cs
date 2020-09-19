using UnityEditor;
using UnityEngine;


// IngredientDrawerUIE
[CustomPropertyDrawer(typeof(Player_CommsController.ShopItem))]
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
        var nameRect = new Rect(position.x, position.y, position.width - 80, position.height);
        var amountRect = new Rect(position.x + position.width - 80, position.y, 35, position.height);
        var costRect = new Rect(position.x + position.width - 80 + 35, position.y, 35, position.height);
        var dollarRect = new Rect(position.x + position.width - 80 + 35 + 35, position.y, 10, position.height);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("uniqueName"), GUIContent.none);
        EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("rewardAmount"), GUIContent.none);
        EditorGUI.PropertyField(costRect, property.FindPropertyRelative("cost"), GUIContent.none);
        EditorGUI.LabelField(dollarRect, "$");

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}