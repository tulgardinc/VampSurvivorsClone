using System.Runtime.InteropServices;
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
        query = state.GetEntityQuery(ComponentType.ReadWrite <LocalTransform>(), ComponentType.ReadOnly <Speed>(),
                                     ComponentType.ReadOnly <LifeTime>(),
                                     ComponentType.ReadOnly <RocketRandomShift>(), ComponentType.ReadOnly <Direction>(),
                                     ComponentType.ReadOnly <Rocket>());
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach (var (rocketTransform, rocketDirection, _) in SystemAPI
                         .Query <RefRO <LocalTransform>, RefRW <Direction>, RefRO <Rocket>>())
        {
            var closestEnemyPosition = new float3(999, 999, 999);
            float minimumDistance = math.INFINITY;
            foreach (var (enemyTransform, _) in SystemAPI.Query <RefRO <LocalTransform>, RefRO <EnemyTag>>())
            {
                if (math.distance(rocketTransform.ValueRO.Position, enemyTransform.ValueRO.Position) <
                    minimumDistance)
                {
                    minimumDistance = math.distance(rocketTransform.ValueRO.Position, enemyTransform.ValueRO.Position);
                    closestEnemyPosition = enemyTransform.ValueRO.Position;
                }
            }

            float3 rocketToEnemyDirection = math.normalizesafe(closestEnemyPosition - rocketTransform.ValueRO.Position);

            rocketDirection.ValueRW.direction =
                    math.normalizesafe(rocketDirection.ValueRO.direction + rocketToEnemyDirection * 0.01f);
        }


        new BulletMovementJob { deltaTime = Time.deltaTime }.ScheduleParallel(query);
    }


    [StructLayout(LayoutKind.Auto)]
    private partial struct BulletMovementJob : IJobEntity
    {

        public float deltaTime;

        private void Execute(ref LocalTransform localTransform, ref Speed speed, ref Direction direction,
                             ref LifeTime lifeTime, ref Rocket rocket, ref RocketRandomShift rocketRandomShift)
        {
            float3 right = math.cross(new float3(0, 0, -1), direction.direction);
            float3 posDelta = math.normalizesafe(right) *
                              math.sin(lifeTime.lifeTimeLeft * rocket.amplitudeSpeed +
                                       math.radians(rocketRandomShift.randomShift)) * rocket.amplitude;
            localTransform.Position += (direction.direction + posDelta) * speed.speed * deltaTime;
        }

    }

}