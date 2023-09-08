using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GlockWeaponAuthoring : MonoBehaviour
{
    public GameObject projectile;
}

public class GlockWeaponAuthoringBaker : Baker<GlockWeaponAuthoring>
{
    public override void Bake(GlockWeaponAuthoring authoring)
    {
        AddComponent(GetEntity(TransformUsageFlags.None), new GlockWeapon
        {
            projectile = GetEntity(authoring.projectile, TransformUsageFlags.Dynamic)
        });
    }
}
