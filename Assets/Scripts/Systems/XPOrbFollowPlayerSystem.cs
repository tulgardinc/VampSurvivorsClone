using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[RequireMatchingQueriesForUpdate]
public partial struct XPOrbFollowPlayerSystem : ISystem
{

    private EntityQuery queryFollow;
    private EntityQuery queryAddComponent;

    public void OnCreate(ref SystemState state)
    {
        queryFollow = state.GetEntityQuery(ComponentType.ReadWrite<LocalTransform>(), ComponentType.ReadWrite<OrbFollowingPlayer>());

        queryAddComponent = state.GetEntityQuery(ComponentType.ReadOnly<LocalTransform>(),
                                                 ComponentType.ReadOnly<Experience>(),
                                                 ComponentType.Exclude<OrbFollowingPlayer>());
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var orbSpawnerData = SystemAPI.GetSingleton<OrbSpawner>();
        float maxSpeed = orbSpawnerData.maxSpeed;
        float acceleration = orbSpawnerData.acceleration;

        foreach (var (playerTransform, canPickupXP) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<CanPickupXP>>())
        {
            var ecbSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);
            var parallelEcb = ecb.AsParallelWriter();

            state.Dependency = new XPOrbShouldFollowJob
            {
                playerPosition = playerTransform.ValueRO.Position,
                range = canPickupXP.ValueRO.pickupRange,
                ecb = parallelEcb,
                deltaTime = Time.deltaTime
            }.ScheduleParallel(queryAddComponent, state.Dependency);

            new XPOrbChaseJob
            {
                maxSpeed = maxSpeed,
                playerPosition = playerTransform.ValueRO.Position,
                deltaTime = Time.deltaTime,
                acceleration = acceleration
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
                ecb.AddComponent(sortKey, entity, new OrbFollowingPlayer());
            }
        }

    }

    [BurstCompile]
    private partial struct XPOrbChaseJob : IJobEntity
    {

        public float3 playerPosition;
        public float deltaTime;
        public float maxSpeed;
        public float acceleration;

        private void Execute(ref LocalTransform orbTransform, ref OrbFollowingPlayer orbFollowingPlayer)
        {
            float3 posDif = playerPosition - orbTransform.Position;
            float3 movementDirection = math.normalize(posDif);
            orbFollowingPlayer.currentSpeed += acceleration * deltaTime;
            float speedToApply = math.clamp(orbFollowingPlayer.currentSpeed * deltaTime, 0, maxSpeed);
            orbTransform.Position += movementDirection * speedToApply;
        }

    }


}