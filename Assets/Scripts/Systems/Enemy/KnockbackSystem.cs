using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

[StructLayout(LayoutKind.Auto)]
public partial struct KnockbackSystem : ISystem
{

    private EntityQuery queryKnockback;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate <EndSimulationEntityCommandBufferSystem.Singleton>();
        queryKnockback = state.GetEntityQuery(ComponentType.ReadWrite <PhysicsVelocity>(),
                                              ComponentType.ReadWrite <WillBeKnockedBack>(),
                                              ComponentType.ReadOnly <Speed>());
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecbSystem = SystemAPI.GetSingleton <EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);
        var parallelEcb = ecb.AsParallelWriter();


        state.Dependency = new KnockbackJob
        {
            fixedDeltaTime = Time.fixedDeltaTime,
            deltaTime = Time.deltaTime,
            ecb = parallelEcb
        }.ScheduleParallel(queryKnockback, state.Dependency);
    }

    [StructLayout(LayoutKind.Auto)]
    public partial struct KnockbackJob : IJobEntity
    {

        public float fixedDeltaTime;
        public float deltaTime;
        public EntityCommandBuffer.ParallelWriter ecb;

        private void Execute(ref WillBeKnockedBack knockbackInfo,
                             ref PhysicsVelocity enemyPhysics,
                             ref Speed enemySpeed,
                             [ChunkIndexInQuery] int sortKey,
                             Entity enemy)
        {
            var knockbackDirection = knockbackInfo.knockbackDirection;
            var knockbackMagnitude = knockbackDirection * knockbackInfo.totalKnockbackAmount;

            var knockbackSlowdown = 2500f;

            enemyPhysics.Linear = knockbackMagnitude * fixedDeltaTime;

            if (knockbackInfo.totalKnockbackAmount > enemySpeed.speed * 40 / 100)
            {
                knockbackInfo.totalKnockbackAmount -= knockbackSlowdown * deltaTime;
            }
            else
            {
                ecb.RemoveComponent <WillBeKnockedBack>(sortKey, enemy);
            }
        }

    }

}