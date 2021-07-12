using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlanetSchematic : ScriptableObject {
    public PlanetTileSettings tileSettings;
    public PlanetColorSettings colorSettings;
    public PlanetGenerationSettings generationSettings;
}
