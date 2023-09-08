using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class GatlingGunWeaponAuthoring : MonoBehaviour
{
    public GameObject projectile;
    public float cooldownDecrease;
    public float minCooldown;
    public float minSpread;
    public float maxSpread;
    public float spreadIncrease;
}

public class GatlingGunWeaponBaker : Baker<GatlingGunWeaponAuthoring>
{
    public override void Bake(GatlingGunWeaponAuthoring authoring)
    {

        AddComponent(GetEntity(TransformUsageFlags.None), new GatlingGunWeapon
        {
            projectile = GetEntity(authoring.projectile, TransformUsageFlags.Dynamic),
            cooldownDecrease = authoring.cooldownDecrease,
            minCooldown = authoring.minCooldown,
            minSpread = authoring.minSpread,
            maxSpread = authoring.maxSpread,
            spreadIncrease = authoring.spreadIncrease,
            random = Unity.Mathematics.Random.CreateFromIndex((uint)UnityEngine.Random.Range(0, 999))
        });
    }
}
