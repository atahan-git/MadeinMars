using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class DataLogger : ScriptableObject {
    public List<int> totalObjectCount = new List<int>();
    [FormerlySerializedAs("totalBeltItemsCount")] public List<int> totalVisualItemsCount = new List<int>();
    public List<float> averageSimTime = new List<float>();
    public List<float> averageVisTime = new List<float>();
    public List<float> mapGenerationTime = new List<float>();
}
