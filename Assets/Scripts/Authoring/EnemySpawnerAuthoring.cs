using Unity.Entities;
using UnityEngine;

public class EnemySpawnerAuthoring : MonoBehaviour
{
    public float spawnRate;
    public float radius;
    public GameObject enemyPrefab;

    [HideInInspector]
    public float nextSpawnTime;
    [HideInInspector]
    public Unity.Mathematics.Random random;
}

public class EnemySpawnerBaker : Baker<EnemySpawnerAuthoring>
{
    public override void Bake (EnemySpawnerAuthoring authoring)
    {
        AddComponent(this.GetEntity(TransformUsageFlags.None), new EnemySpawner { spawnRate = authoring.spawnRate, radius = authoring.radius, nextSpawnTime = 0, random = Unity.Mathematics.Random.CreateFromIndex(18123), enemyPrefab = this.GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic) });
    }
}