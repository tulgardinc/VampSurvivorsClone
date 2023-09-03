using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[StructLayout(LayoutKind.Auto)]
public partial struct KnockbackSystem : ISystem
{

    private EntityQuery queryKnockback;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate <EndSimulationEntityCommandBufferSystem.Singleton>();
        queryKnockback = state.GetEntityQuery(ComponentType.ReadWrite <LocalTransform>(),
                                              ComponentType.ReadWrite <PhysicsVelocity>(),
                                              ComponentType.ReadWrite <WillBeKnockedBack>(),
                                              ComponentType.ReadOnly <EnemyTag>());
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach (var (playerTransform, _) in SystemAPI
                         .Query <RefRW <LocalTransform>, RefRO <PlayerTag>>())
        {
            var ecbSystem = SystemAPI.GetSingleton <EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);
            var parallelEcb = ecb.AsParallelWriter();


            state.Dependency = new KnockbackJob
            {
                playerPosition = playerTransform.ValueRO.Position,
                fixedDeltaTime = Time.fixedDeltaTime,
                deltaTime = Time.deltaTime,
                ecb = parallelEcb
            }.ScheduleParallel(queryKnockback, state.Dependency);
        }
    }

    [StructLayout(LayoutKind.Auto)]
    public partial struct KnockbackJob : IJobEntity
    {

        public float3 playerPosition;
        public float fixedDeltaTime;
        public float deltaTime;
        public EntityCommandBuffer.ParallelWriter ecb;

        private void Execute(ref LocalTransform enemyPosition, ref WillBeKnockedBack knockbackAmount,
                             ref PhysicsVelocity enemyPhysics, [ChunkIndexInQuery] int sortKey, Entity enemy)
        {
            var knockbackDirection = math.normalizesafe(enemyPosition.Position - playerPosition);
            var knockbackMagnitude = knockbackDirection * knockbackAmount.totalKnockbackAmount;

            var knockbackSlowdown = 100f;

            enemyPhysics.Linear = knockbackMagnitude * fixedDeltaTime;

            if (knockbackAmount.totalKnockbackAmount > 0)
            {
                knockbackAmount.totalKnockbackAmount -= knockbackSlowdown * deltaTime;
            }
            else
            {
                ecb.RemoveComponent <WillBeKnockedBack>(sortKey, enemy);
            }
        }

    }

}