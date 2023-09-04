using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[StructLayout(LayoutKind.Auto)]
public partial struct EnemySpawnSystem : ISystem
{

    private EntityQuery enemySpawnerQuery;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate <EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate <PlayerTag>();
        state.RequireForUpdate <EnemySpawner>();

        enemySpawnerQuery = state.GetEntityQuery(ComponentType.ReadWrite <EnemySpawner>());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var playerTransform = SystemAPI.GetComponent <LocalTransform>(SystemAPI.GetSingletonEntity <PlayerTag>());

        var ecbSystem = SystemAPI.GetSingleton <EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);
        var parallelEcb = ecb.AsParallelWriter();


        var spawningJob = new EnemySpawningJob
        {
            playerTransform = playerTransform.Position,
            elapsedTime = SystemAPI.Time.ElapsedTime,
            ecb = parallelEcb
        }.ScheduleParallel(enemySpawnerQuery, state.Dependency);

        spawningJob.Complete();

        // foreach (var (playerTransform, _) in SystemAPI.Query <RefRO <LocalTransform>, RefRO <PlayerTag>>())
        // {
        //     foreach (var spawner in SystemAPI.Query <RefRW <EnemySpawner>>())
        //     {
        //         if (!spawner.ValueRO.isEnabled)
        //         {
        //             return;
        //         }
        //
        //         if (spawner.ValueRO.nextSpawnTime < SystemAPI.Time.ElapsedTime)
        //         {
        //             var enemy = state.EntityManager.Instantiate(spawner.ValueRO.enemyPrefab);
        //
        //             var enemyPosition = new float3(spawner.ValueRW.random.NextFloat(-1, 1),
        //                                            spawner.ValueRW.random.NextFloat(-1, 1), 0);
        //             var enemyDirection = math.normalize(enemyPosition) * spawner.ValueRO.radius;
        //             state.EntityManager.SetComponentData(enemy,
        //                                                  LocalTransform.FromPosition(playerTransform.ValueRO.Position +
        //                                                      enemyDirection));
        //             spawner.ValueRW.nextSpawnTime = (float)SystemAPI.Time.ElapsedTime + spawner.ValueRO.spawnRate;
        //         }
        //     }
        // }
    }

    [StructLayout(LayoutKind.Auto)]
    private partial struct EnemySpawningJob : IJobEntity
    {

        public float3 playerTransform;
        public double elapsedTime;
        public EntityCommandBuffer.ParallelWriter ecb;


        private void Execute(ref EnemySpawner enemySpawnerData,
                             [ChunkIndexInQuery] int sortKey)
        {
            if (!enemySpawnerData.isEnabled)
            {
                return;
            }

            if (enemySpawnerData.nextSpawnTime < elapsedTime)
            {
                for (var i = 0; i < enemySpawnerData.spawnAmount; i++)
                {
                    var enemy = ecb.Instantiate(sortKey, enemySpawnerData.enemyPrefab);

                    var enemyPosition = new float3(enemySpawnerData.random.NextFloat(-1, 1),
                                                   enemySpawnerData.random.NextFloat(-1, 1), 0);
                    var enemySpawnDirection = math.normalize(enemyPosition) * enemySpawnerData.radius;

                    ecb.SetComponent(sortKey, enemy,
                                     LocalTransform.FromPosition(playerTransform + enemySpawnDirection));
                }

                enemySpawnerData.nextSpawnTime = (float)elapsedTime + enemySpawnerData.spawnRate;
            }
        }

    }

}