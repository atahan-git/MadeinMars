using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu]
public class PlanetColorSettings : ScriptableObject
{
    public Color[] groundColor = new Color[4];
    public Color[] groundColorAlternative = new Color[4];


    public Color GetColor(int level, int type) {
        Assert.IsTrue(level < groundColor.Length);
        Assert.IsTrue(level >= 0);
        Assert.IsTrue(type == 0 || type == 1);
        if (type == 0) {
            return groundColor[level];
        } else {
            return groundColorAlternative[level];
        }
    }
}
