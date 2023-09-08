using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct XPOrbHasStartMotion : IComponentData
{
    public float currentSpeed;
    public float3 direction;
}
