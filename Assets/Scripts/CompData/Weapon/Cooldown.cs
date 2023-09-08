using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct Cooldown : IComponentData
{
    public float initialCooldown;
    public float cooldownTime;
    public float timer;
}
