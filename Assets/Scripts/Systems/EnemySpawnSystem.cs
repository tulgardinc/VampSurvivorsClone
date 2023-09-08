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
        state.RequireForUpdate <LevellingData>();
        state.RequireForUpdate <EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate <PlayerTag>();
        state.RequireForUpdate <EnemySpawner>();

        enemySpawnerQuery = state.GetEntityQuery(ComponentType.ReadWrite <EnemySpawner>());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var playerTransform = SystemAPI.GetComponent <LocalTransform>(SystemAPI.GetSingletonEntity <PlayerTag>());
        var playerLevel = SystemAPI.GetComponent <LevellingData>(SystemAPI.GetSingletonEntity <PlayerTag>());

        var ecbSystem = SystemAPI.GetSingleton <EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);
        var parallelEcb = ecb.AsParallelWriter();

        var spawningJob = new EnemySpawningJob
        {
            playerTransform = playerTransform.Position,
            playerLevel = playerLevel.currentLevel,
            elapsedTime = SystemAPI.Time.ElapsedTime,
            ecb = parallelEcb
        }.ScheduleParallel(enemySpawnerQuery, state.Dependency);

        spawningJob.Complete();
    }

    [StructLayout(LayoutKind.Auto)]
    private partial struct EnemySpawningJob : IJobEntity
    {

        public float3 playerTransform;
        public float playerLevel;
        public double elapsedTime;
        public EntityCommandBuffer.ParallelWriter ecb;

        private void Execute(ref EnemySpawner enemySpawnerData,
                             [ChunkIndexInQuery] int sortKey)
        {
            enemySpawnerData.spawnAmount = 1 + math.floor((playerLevel - 1) / 5);

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