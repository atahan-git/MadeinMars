using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This is the holder asset for objectives.
/// </summary>
[CreateAssetMenu(fileName = "New Objective", menuName = "Objective Holder")]
public class ObjectiveHolder : ScriptableObject {
    public string uniqueName;

    public string title;
    [TextArea] public string description;

    
    [HideInInspector]
    // Set during runtime
    public bool isComplete = false;
    
    [Tooltip("Custom Objective Language")]
    public string[] completeReqs;

    [Tooltip("Custom Objective Language")]
    public string[] unlockReqs;
    
    // See Objective Checker for details
}
