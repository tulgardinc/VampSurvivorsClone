using Unity.Entities;
using Unity.Mathematics;

public static class EntityUtilities
{
    public static void EnemyHitWithEffectsParallel(int index, EntityCommandBuffer.ParallelWriter ecb, Entity enemy, float damage, float knockback, float3 knockbackDirection)
    {
        ecb.AddComponent(index, enemy, new BulletHit
        {
            damage = damage
        });
        ecb.AddComponent(index, enemy, new WillBeKnockedBack
        {
            totalKnockbackAmount = knockback,
            knockbackDirection = knockbackDirection
        });
        ecb.AddComponent(index, enemy, new DamageFlashing
        {
            flashTimer = 0
        });
    }
}
