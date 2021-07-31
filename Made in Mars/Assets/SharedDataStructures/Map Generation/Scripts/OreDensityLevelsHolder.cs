using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "OreSpawnSettings/OreDensityLevelsHolder")]
public class OreDensityLevelsHolder : ScriptableObject {
    [Header("Put ores here in increasing order of density")]
    public OreSpawnSettings[] oreLevels = new OreSpawnSettings[3];

    public string GetUniqueName() {
        return oreLevels[0].oreUniqueName;
    }
}
