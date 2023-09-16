using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class RocketLauncherWeaponAuthoring : MonoBehaviour
{
    public GameObject projectile;
    public int rocketCount;
    public float spreadRange;

    public class RocketLauncherBaker : Baker<RocketLauncherWeaponAuthoring>
    {
        public override void Bake(RocketLauncherWeaponAuthoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.None), new RocketLauncherWeapon
            {
                random = Unity.Mathematics.Random.CreateFromIndex((uint)UnityEngine.Random.Range(0, 9999.0f)),
                rocketCount = authoring.rocketCount,
                spreadRange = authoring.spreadRange,
                projectile = GetEntity(authoring.projectile, TransformUsageFlags.Dynamic)
            });
        }
    }
}
