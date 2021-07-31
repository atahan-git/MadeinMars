using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlanetSchematic : ScriptableObject {
    public string uniqueName = "new Schematic";
    public Sprite previewImage;
    public MapFilter[] filters;
}
