using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct RocketLauncherWeapon : IComponentData
{

    public Entity projectile;
    public int rocketCount;
    public float spreadRange;
    public Unity.Mathematics.Random random;

}
