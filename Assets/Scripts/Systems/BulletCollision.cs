using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct BulletCollision : ISystem
{

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate <PlayerTag>();
        state.RequireForUpdate <SimulationSingleton>();
        state.RequireForUpdate <EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var playerTransform = SystemAPI.GetComponent <LocalTransform>(SystemAPI.GetSingletonEntity <PlayerTag>());

        state.Dependency = new BulletCollisionJob
        {
            bulletLookup = SystemAPI.GetComponentLookup <BulletTag>(true),
            damageLookup = SystemAPI.GetComponentLookup <Damage>(true),
            bulletHealthLookup = SystemAPI.GetComponentLookup <BulletHealth>(),
            enemyLookup = SystemAPI.GetComponentLookup <EnemyTag>(true),
            knockbackLookup = SystemAPI.GetComponentLookup <KnockbackData>(true),
            transformLookup = SystemAPI.GetComponentLookup <LocalTransform>(true),
            playerTransform = playerTransform,
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
        [ReadOnly] public ComponentLookup <LocalTransform> transformLookup;
        public ComponentLookup <BulletHealth> bulletHealthLookup;
        public LocalTransform playerTransform;

        public BufferLookup <InCollisionWith> inCollisionWithLookup;

        public EntityCommandBuffer commandBuffer;

        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.EntityA;
            var entityB = triggerEvent.EntityB;

            Entity enemy;
            Entity bullet;

            if (IsEnemy(entityA) && IsBullet(entityB))
            {
                enemy = entityA;
                bullet = entityB;
            }
            else if (IsEnemy(entityB) && IsBullet(entityA))
            {
                enemy = entityB;
                bullet = entityA;
            }
            else
            {
                return;
            }


            var bulletDamage = GetDamage(bullet).ValueRO.damage;
            var bulletKnockback = GetKnockback(bullet).ValueRO.knockbackAmount;
            var bulletHealth = GetBulletHealth(bullet).ValueRW.bulletHealth;

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
                    totalKnockbackAmount = bulletKnockback,
                    knockbackDirection =
                            math.normalizesafe(GetTransform(enemy).ValueRO.Position - playerTransform.Position)
                });
                commandBuffer.AddComponent(enemy, new DamageFlashing
                {
                    flashTimer = 0
                });
                commandBuffer.SetComponent(bullet, new BulletHealth { bulletHealth = bulletHealth - 1 });
            }
        }

        private bool IsBullet(Entity entity) { return bulletLookup.HasComponent(entity); }

        private bool IsEnemy(Entity entity) { return enemyLookup.HasComponent(entity); }

        private RefRO <Damage> GetDamage(Entity entity) { return damageLookup.GetRefRO(entity); }

        private RefRW <BulletHealth> GetBulletHealth(Entity entity) { return bulletHealthLookup.GetRefRW(entity); }

        private RefRO <KnockbackData> GetKnockback(Entity entity) { return knockbackLookup.GetRefRO(entity); }

        private RefRO <LocalTransform> GetTransform(Entity entity) { return transformLookup.GetRefRO(entity); }

    }

}