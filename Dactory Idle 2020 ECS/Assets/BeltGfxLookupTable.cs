using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu]
public class BeltGfxLookupTable : ScriptableObject
{
    public Sprite[] sprites;
    public Dictionary<string, Sprite> mapping = new Dictionary<string, Sprite>();

    [Header("This will be set to true automatically if the dictionary is set up, See All Belt Types scene")]
    public bool isSetUp = false;


    public static string beltToKey (BeltObject belt) {
        return string.Join(", ", belt.beltInputs.Select(i => i.ToString()).ToArray()) + "," + string.Join(", ", belt.beltOutputs.Select(i => i.ToString()).ToArray());
    }
}
