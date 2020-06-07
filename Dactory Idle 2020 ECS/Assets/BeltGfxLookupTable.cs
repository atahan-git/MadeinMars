using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu]
public class BeltGfxLookupTable : ScriptableObject
{
    public Sprite[] sprites;
    public Dictionary<string, Sprite> mapping { get { 
            if (_mapping != null) return _mapping; 
            else return GenerateDictionary(); 
        } }
    Dictionary<string, Sprite> _mapping;

    // These are for serialization purposes
    [SerializeField]
    string[] keys;
    [SerializeField]
    Sprite[] values;

    [Header("This will be set to true automatically if the dictionary is set up, See All Belt Types scene")]
    public bool isSetUp = false;

    public void SetUpDictionary (string[] _keys, Sprite[] _values) {
        keys = _keys;
        values = _values;
    }

    Dictionary<string, Sprite> GenerateDictionary () {
        _mapping = new Dictionary<string, Sprite>();

        for (int i = 0; i < keys.Length; i++) {
            _mapping.Add(keys[i],values[i]);
        }

        return _mapping;
    }

    public static string beltToKey (BeltObject belt) {
        return string.Join(", ", belt.beltInputs.Select(i => i.ToString()).ToArray()) + "; " + string.Join(", ", belt.beltOutputs.Select(i => i.ToString()).ToArray());
    }
}
