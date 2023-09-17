using Unity.Entities;

public struct Explosion : IComponentData
{

    public float explosionTimer;
    public float explosionRange;
    public float explosionDamage;
    public float explosionKnockback;

    public static Explosion FromExplosionData(ExplosionData explosionData)
    {
        return new Explosion
        {
            explosionTimer = explosionData.explosionTimer,
            explosionDamage = explosionData.explosionDamage,
            explosionKnockback = explosionData.explosionKnockback,
            explosionRange = explosionData.explosionRange
        };
    }

}