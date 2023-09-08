using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[RequireMatchingQueriesForUpdate]
[StructLayout(LayoutKind.Auto)]
public partial struct EnemyMovement : ISystem
{
    private EntityQuery query;


    public void OnCreate(ref SystemState state)
    {
        query = state.GetEntityQuery(ComponentType.ReadWrite<LocalTransform>(), ComponentType.ReadOnly<Speed>(),
            ComponentType.ReadWrite<PhysicsVelocity>(), ComponentType.ReadOnly<EnemyTag>());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var playerPos = float3.zero;
        foreach (var (playerTransform, _) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<PlayerTag>>())
            playerPos = playerTransform.ValueRO.Position;
        new EnemyChaseJob
        {
            playerPosition = playerPos,
            deltaTime = Time.fixedDeltaTime
        }.ScheduleParallel(query);
    }


    [BurstCompile]
    [StructLayout(LayoutKind.Auto)]
    private partial struct EnemyChaseJob : IJobEntity
    {
        public float3 playerPosition;
        public float deltaTime;

        private void Execute(ref LocalTransform localTransform, ref Speed speed, ref PhysicsVelocity physicsVelocity)
        {
            var movementDirection = math.normalize(playerPosition - localTransform.Position);
            var velocity = movementDirection * speed.speed * deltaTime;
            physicsVelocity.Linear = velocity;
        }
    }
}