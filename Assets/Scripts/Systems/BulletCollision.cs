using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct BulletCollision : ISystem
{

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate <SimulationSingleton>();
        state.RequireForUpdate <EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency = new BulletCollisionJob
        {
            bulletLookup = SystemAPI.GetComponentLookup <BulletTag>(true),
            damageLookup = SystemAPI.GetComponentLookup <Damage>(true),
            enemyLookup = SystemAPI.GetComponentLookup <EnemyTag>(true),
            knockbackLookup = SystemAPI.GetComponentLookup <KnockbackData>(true),
            inCollisionWithLookup = SystemAPI.GetBufferLookup <InCollisionWith>(),
            commandBuffer = SystemAPI.GetSingleton <EndSimulationEntityCommandBufferSystem.Singleton>()
                                     .CreateCommandBuffer(state.WorldUnmanaged)
        }.Schedule(
                   SystemAPI.GetSingleton <SimulationSingleton>(), state.Dependency);
    }

    private struct BulletCollisionJob : ITriggerEventsJob
    {

        [ReadOnly] public ComponentLookup <BulletTag> bulletLookup;
        [ReadOnly] public ComponentLookup <EnemyTag> enemyLookup;
        [ReadOnly] public ComponentLookup <Damage> damageLookup;
        [ReadOnly] public ComponentLookup <KnockbackData> knockbackLookup;

        public BufferLookup <InCollisionWith> inCollisionWithLookup;

        public EntityCommandBuffer commandBuffer;

        public void Execute(TriggerEvent triggerEvent)
        {
            var enemy = triggerEvent.EntityA;
            var bullet = triggerEvent.EntityB;

            if (IsEnemy(enemy) && IsBullet(bullet))
            {
                var bulletDamage = GetDamage(bullet).ValueRO.damage;
                var bulletKnockback = GetKnockback(bullet).ValueRO.knockbackAmount;

                inCollisionWithLookup.TryGetBuffer(bullet, out var inCollisionWith);

                var isUnique = true;
                foreach (var entity in inCollisionWith)
                {
                    if (entity.Value.Equals(enemy))
                    {
                        isUnique = false;

                        break;
                    }
                }

                if (isUnique)
                {
                    inCollisionWith.Add(enemy);
                    commandBuffer.AddComponent(enemy, new BulletHit
                    {
                        damage = bulletDamage
                    });
                    commandBuffer.AddComponent(enemy, new WillBeKnockedBack
                    {
                        totalKnockbackAmount = bulletKnockback
                    });
                }
            }
        }

        private bool IsBullet(Entity entity) { return bulletLookup.HasComponent(entity); }

        private bool IsEnemy(Entity entity) { return enemyLookup.HasComponent(entity); }

        private RefRO <Damage> GetDamage(Entity entity) { return damageLookup.GetRefRO(entity); }

        private RefRO <KnockbackData> GetKnockback(Entity entity) { return knockbackLookup.GetRefRO(entity); }

    }

}