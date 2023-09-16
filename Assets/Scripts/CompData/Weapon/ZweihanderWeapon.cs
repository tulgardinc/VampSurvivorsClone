using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


public struct ZweihanderWeapon : IComponentData
{
    public Entity collision;
    public float radius;
    public float sweepingAngle;
    public float damage;
    public float knockbackAmount;
}
