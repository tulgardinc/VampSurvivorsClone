using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[RequireMatchingQueriesForUpdate]
[StructLayout(LayoutKind.Auto)]
public partial struct BulletMovement : ISystem
{

    private EntityQuery query;

    public void OnCreate(ref SystemState state)
    {
        query = state.GetEntityQuery(ComponentType.ReadWrite<LocalTransform>(), ComponentType.ReadOnly<Speed>(),
                                     ComponentType.ReadOnly<Direction>(), ComponentType.ReadOnly<LinearProjectileTag>());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new BulletMovementJob { deltaTime = Time.deltaTime }.ScheduleParallel(query);
    }

    [BurstCompile]
    [StructLayout(LayoutKind.Auto)]
    private partial struct BulletMovementJob : IJobEntity
    {

        public float deltaTime;

        private void Execute(ref LocalTransform localTransform, ref Speed speed, ref Direction direction)
        {
            localTransform.Position += direction.direction * speed.speed * deltaTime;
        }

    }

}