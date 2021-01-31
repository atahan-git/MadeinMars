using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct Movement : IComponentData
{
    // the target we want to move towards (offset included)
    public float3 targetWithOffset;
}
