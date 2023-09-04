using Unity.Entities;
using UnityEngine;

public class EnemySpawnerAuthoring : MonoBehaviour
{
    public bool isEnabled;
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
    public override void Bake(EnemySpawnerAuthoring authoring)
    {
        AddComponent(GetEntity(TransformUsageFlags.None),
            new EnemySpawner
            {
                isEnabled = authoring.isEnabled,
                spawnRate = authoring.spawnRate,
                radius = authoring.radius,
                nextSpawnTime = 0,
                random = Unity.Mathematics.Random.CreateFromIndex((uint)Random.Range(0, 9999)),
                enemyPrefab = this.GetEntity(authoring.enemyPrefab,
                TransformUsageFlags.Dynamic)
            });
    }
}