using Unity.Entities;

public struct ExplosionData : IComponentData
{

    public float explosionTimer;
    public float explosionRange;
    public float explosionDamage;
    public float explosionKnockback;

}