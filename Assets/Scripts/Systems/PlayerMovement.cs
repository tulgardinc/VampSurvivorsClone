using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct PlayerMovement : ISystem
{
    [BurstCompile]
    public void OnUpdate (ref SystemState state)
    {
        foreach (var (localTransform, speed, direction, _) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<Speed>, RefRW<Direction>, RefRO<PlayerTag>>())
        {
            float3 movement = new float3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
            float3 dir = math.normalizesafe(movement);

            if (!dir.Equals(float3.zero))
            {
                direction.ValueRW.previousDirection = direction.ValueRO.direction;
                direction.ValueRW.direction = dir;
            }
            else
            {
                direction.ValueRW.direction = dir;
            }

            localTransform.ValueRW.Position += dir * speed.ValueRO.speed * Time.deltaTime;

        }
    }

}
