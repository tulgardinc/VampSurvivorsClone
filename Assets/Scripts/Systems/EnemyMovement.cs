using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[RequireMatchingQueriesForUpdate]
public partial struct EnemyMovement : ISystem
{
    EntityQuery query;

    public void OnCreate (ref SystemState state)
    {
        query = state.GetEntityQuery(ComponentType.ReadWrite<LocalTransform>(), ComponentType.ReadOnly<Speed>(), ComponentType.ReadWrite<PhysicsVelocity>(), ComponentType.ReadOnly<EnemyTag>());
    }

    [BurstCompile]
    public void OnUpdate (ref SystemState state)
    {
        float3 playerPos = float3.zero;
        foreach (var (playerTransform, _) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<PlayerTag>>())
        {
            playerPos = playerTransform.ValueRO.Position;
        }
        new EnemyChaseJob
        {
            playerPosition = playerPos,
            deltaTime = Time.fixedDeltaTime
        }.ScheduleParallel(query);
    }


    [BurstCompile]
    partial struct EnemyChaseJob : IJobEntity
    {
        public float3 playerPosition;
        public float deltaTime;

        void Execute (ref LocalTransform localTransform, ref Speed speed, ref PhysicsVelocity physicsVelocity)
        {
            float3 movementDirection = math.normalize(playerPosition - localTransform.Position);
            float3 velocity = movementDirection * speed.speed * deltaTime;
            physicsVelocity.Linear = velocity;
        }
    }
}
