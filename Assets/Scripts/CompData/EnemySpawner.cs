using Unity.Entities;
using Unity.Mathematics;

public struct EnemySpawner : IComponentData
{
    public float spawnRate;
    public float radius;
    public float nextSpawnTime;
    public bool isEnabled;
    public Random random;
    public Entity enemyPrefab;
}