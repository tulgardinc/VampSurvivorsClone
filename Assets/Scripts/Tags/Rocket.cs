using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct Rocket : IComponentData
{
    public float amplitude;
    public float amplitudeSpeed;
}
