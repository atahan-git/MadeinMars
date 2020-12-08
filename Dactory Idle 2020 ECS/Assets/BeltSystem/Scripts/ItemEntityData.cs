using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;



/// <summary>
/// Needed for working with Unity's ECS system
/// Basically, these are the custom data each item gfx entity holds
/// </summary>
[Serializable]
public struct ItemMovement : IComponentData {
    // the target we want to move towards (offset included)
    public float3 targetWithOffset;
}

[Serializable]
public struct ItemID : IComponentData {
    public ushort myItemId;
}
