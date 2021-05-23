using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DataLogger : ScriptableObject {
    public List<int> totalObjectCount = new List<int>();
    public List<int> totalBeltItemsCount = new List<int>();
    public List<float> averageSimTime = new List<float>();
    public List<float> averageVisTime = new List<float>();
}
