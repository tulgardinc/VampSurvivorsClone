

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;


[RequireMatchingQueriesForUpdate]
public partial struct XPOrbFollowPlayerSystem : ISystem
{
    private EntityQuery queryFollow;
    private EntityQuery queryAddComponent;

    public void OnCreate(ref SystemState state)
    {
        queryFollow = state.GetEntityQuery(ComponentType.ReadWrite<LocalTransform>(), ComponentType.ReadOnly<Speed>(),
           ComponentType.ReadWrite<OrbFollowingPlayer>());
        queryAddComponent = state.GetEntityQuery(ComponentType.ReadOnly<LocalTransform>(), ComponentType.ReadOnly<Experience>(), ComponentType.Exclude<OrbFollowingPlayer>());
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach (var (playerTransform, canPickupXP) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<CanPickupXP>>())
        {
            var ecbSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);
            EntityCommandBuffer.ParallelWriter parallelEcb = ecb.AsParallelWriter();

            new XPOrbShouldFollowJob
            {
                playerPosition = playerTransform.ValueRO.Position,
                range = canPickupXP.ValueRO.pickupRange,
                ecb = parallelEcb,
                deltaTime = Time.deltaTime
            }.ScheduleParallel(queryAddComponent);

            new XPOrbChaseJob
            {
                playerPosition = playerTransform.ValueRO.Position,
                deltaTime = Time.fixedDeltaTime
            }.ScheduleParallel(queryFollow);
        }
    }

    [BurstCompile]
    private partial struct XPOrbShouldFollowJob : IJobEntity
    {
        public float3 playerPosition;
        public float range;
        public EntityCommandBuffer.ParallelWriter ecb;
        public float deltaTime;

        private void Execute(ref LocalTransform orbTransform, [ChunkIndexInQuery] int sortKey, Entity entity)
        {
            var distance = math.distance(playerPosition, orbTransform.Position);
            if (distance < range)
            {
                ecb.AddComponent(sortKey, entity, new OrbFollowingPlayer { });
            }
        }
    }

    [BurstCompile]
    private partial struct XPOrbChaseJob : IJobEntity
    {
        public float3 playerPosition;
        public float deltaTime;

        private void Execute(ref LocalTransform orbTransform,
            ref Speed speed, ref OrbFollowingPlayer orbFollowingPlayer)
        {
            var movementDirection = math.normalize(playerPosition - orbTransform.Position);
            orbFollowingPlayer.currentSpeed += speed.speed * deltaTime;
            orbTransform.Position += movementDirection * orbFollowingPlayer.currentSpeed * deltaTime;
        }
    }


}