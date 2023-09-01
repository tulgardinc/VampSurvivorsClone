using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;


[RequireMatchingQueriesForUpdate]
public partial struct BulletMovement : ISystem
{
    EntityQuery query;

    public void OnCreate (ref SystemState state)
    {
        query = state.GetEntityQuery(ComponentType.ReadWrite<LocalTransform>(), ComponentType.ReadOnly<Speed>(), ComponentType.ReadOnly<Direction>(), ComponentType.ReadOnly<BulletTag>());
    }

    [BurstCompile]
    public void OnUpdate (ref SystemState state)
    {
        new BulletMovementJob { deltaTime = Time.deltaTime }.ScheduleParallel(query);
    }

    [BurstCompile]
    partial struct BulletMovementJob : IJobEntity
    {
        public float deltaTime;

        void Execute (ref LocalTransform localTransform, ref Speed speed, ref Direction direction)
        {
            localTransform.Position += direction.direction * speed.speed * deltaTime;
        }
    }
}
