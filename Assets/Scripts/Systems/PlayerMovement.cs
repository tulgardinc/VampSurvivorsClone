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
        foreach (var (localTransform, speed, _) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<Speed>, RefRO<PlayerTag>>())
        {
            float3 movement = new float3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
            localTransform.ValueRW.Position += math.normalizesafe(movement) * speed.ValueRO.speed * Time.deltaTime;
        }
    }

}
