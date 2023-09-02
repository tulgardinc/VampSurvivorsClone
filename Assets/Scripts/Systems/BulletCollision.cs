using System.Buffers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PhysicsSimulationGroup))]
public partial struct BulletCollision : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency = new BulletCollisionJob
        {
            bulletLookup = SystemAPI.GetComponentLookup<BulletTag>(true),
            damageLookup = SystemAPI.GetComponentLookup<Damage>(true),
            enemyLookup = SystemAPI.GetComponentLookup<EnemyTag>(true),
            commandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged)
        }.Schedule(
            SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }

    [BurstCompile]
    private struct BulletCollisionJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<BulletTag> bulletLookup;
        [ReadOnly] public ComponentLookup<EnemyTag> enemyLookup;
        [ReadOnly] public ComponentLookup<Damage> damageLookup;

        public EntityCommandBuffer commandBuffer;

        bool IsBullet(Entity entity) => bulletLookup.HasComponent(entity);
        bool IsEnemy(Entity entity) => enemyLookup.HasComponent(entity);
        RefRO<Damage> GetDamage(Entity entity) => damageLookup.GetRefRO(entity);

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity enemy = triggerEvent.EntityA;
            Entity bullet = triggerEvent.EntityB;

            if (IsEnemy(enemy) && IsBullet(bullet))
            {
                float bulletDamage = GetDamage(bullet).ValueRO.damage;

                commandBuffer.AddComponent(enemy, new BulletHit
                {
                    damage = bulletDamage
                });
            }
        }
    }
}

