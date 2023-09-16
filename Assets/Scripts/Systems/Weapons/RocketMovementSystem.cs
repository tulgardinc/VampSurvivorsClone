using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[RequireMatchingQueriesForUpdate]
[StructLayout(LayoutKind.Auto)]
public partial struct RocketMovementSystem : ISystem
{

    private EntityQuery query;

    public void OnCreate(ref SystemState state)
    {
        query = state.GetEntityQuery(ComponentType.ReadWrite<LocalTransform>(), ComponentType.ReadOnly<Speed>(), ComponentType.ReadOnly<LifeTime>(),
                                     ComponentType.ReadOnly<RocketRandomShift>(), ComponentType.ReadOnly<Direction>(), ComponentType.ReadOnly<Rocket>());
    }

    public void OnUpdate(ref SystemState state)
    {
        new BulletMovementJob { deltaTime = Time.deltaTime }.ScheduleParallel(query);
    }

    [StructLayout(LayoutKind.Auto)]
    private partial struct BulletMovementJob : IJobEntity
    {

        public float deltaTime;

        private void Execute(ref LocalTransform localTransform, ref Speed speed, ref Direction direction, ref LifeTime lifeTime, ref Rocket rocket, ref RocketRandomShift rocketRandomShift)
        {
            float3 right = math.cross(new float3(0, 0, -1), direction.direction);
            float3 posDelta = math.normalizesafe(right) * math.sin(lifeTime.lifeTimeLeft * rocket.amplitudeSpeed + math.radians(rocketRandomShift.randomShift)) * rocket.amplitude;
            localTransform.Position += (direction.direction + posDelta) * speed.speed * deltaTime;
        }

    }

}
