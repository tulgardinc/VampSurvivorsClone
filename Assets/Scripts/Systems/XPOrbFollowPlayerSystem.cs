using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[RequireMatchingQueriesForUpdate]
[StructLayout(LayoutKind.Auto)]
public partial struct XPOrbFollowPlayerSystem : ISystem
{

    private EntityQuery queryFollow;
    private EntityQuery queryAddComponent;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate <OrbSpawner>();
        queryFollow = state.GetEntityQuery(ComponentType.ReadWrite <LocalTransform>(),
                                           ComponentType.ReadWrite <OrbFollowingPlayer>());

        queryAddComponent = state.GetEntityQuery(ComponentType.ReadOnly <LocalTransform>(),
                                                 ComponentType.ReadOnly <Experience>(),
                                                 ComponentType.Exclude <OrbFollowingPlayer>());
        state.RequireForUpdate <EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var orbSpawnerData = SystemAPI.GetSingleton <OrbSpawner>();
        var maxSpeed = orbSpawnerData.maxSpeed;
        var acceleration = orbSpawnerData.acceleration;

        foreach (var (playerTransform, canPickupXP) in SystemAPI.Query <RefRO <LocalTransform>, RefRO <CanPickupXP>>())
        {
            var ecbSystem = SystemAPI.GetSingleton <EndSimulationEntityCommandBufferSystem.Singleton>();
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
    [StructLayout(LayoutKind.Auto)]
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
    [StructLayout(LayoutKind.Auto)]
    private partial struct XPOrbChaseJob : IJobEntity
    {

        public float3 playerPosition;
        public float deltaTime;
        public float maxSpeed;
        public float acceleration;

        private void Execute(ref LocalTransform orbTransform, ref OrbFollowingPlayer orbFollowingPlayer)
        {
            var posDif = playerPosition - orbTransform.Position;
            var movementDirection = math.normalize(posDif);
            orbFollowingPlayer.currentSpeed += acceleration * deltaTime;
            var speedToApply = math.clamp(orbFollowingPlayer.currentSpeed * deltaTime, 0, maxSpeed);
            orbTransform.Position += movementDirection * speedToApply;
        }

    }


}