using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

public partial struct EnemyMovement : ISystem
{
    [BurstCompile]
    public void OnUpdate (ref SystemState state)
    {

        foreach (var (localTransform, speed, _) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<Speed>, RefRO<EnemyTag>>())
        {
            //float3 movementDirection = math.normalize(playerTransform.ValueRO.Position - localTransform.ValueRW.Position);
            //localTransform.ValueRW.Position += movementDirection * speed.ValueRO.speed * Time.deltaTime;
        }

    }
}
