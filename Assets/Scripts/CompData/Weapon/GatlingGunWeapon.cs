using Unity.Entities;
using Unity.Mathematics;

public struct GatlingGunWeapon : IComponentData
{
    public Entity projectile;
    public float cooldownDecrease;
    public float minCooldown;
    public float minSpread;
    public float spread;
    public float maxSpread;
    public float spreadIncrease;
    public Random random;
}
