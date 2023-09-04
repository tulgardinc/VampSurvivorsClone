using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct EnemySpawnSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (playerTransform, _) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<PlayerTag>>())
        {
            foreach (var spawner in SystemAPI.Query<RefRW<EnemySpawner>>())
            {
                if (!spawner.ValueRO.isEnabled)
                {
                    return;
                }

                if (spawner.ValueRO.nextSpawnTime < SystemAPI.Time.ElapsedTime)
                {
                    Entity enemy = state.EntityManager.Instantiate(spawner.ValueRO.enemyPrefab);

                    float3 enemyPosition = new float3(spawner.ValueRW.random.NextFloat(-1, 1), spawner.ValueRW.random.NextFloat(-1, 1), 0);
                    float3 enemyDirection = math.normalize(enemyPosition) * spawner.ValueRO.radius;
                    state.EntityManager.SetComponentData(enemy, LocalTransform.FromPosition(playerTransform.ValueRO.Position + enemyDirection));
                    spawner.ValueRW.nextSpawnTime = (float)SystemAPI.Time.ElapsedTime + spawner.ValueRO.spawnRate;
                }
            }
        }
    }
}
