using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct LifeTime : IComponentData
{
    public float totalLifeTime;
    public float lifeTimeLeft;
}
