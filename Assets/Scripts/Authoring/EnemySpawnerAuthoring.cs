using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class EnemySpawnerAuthoring : MonoBehaviour
{

    public bool isEnabled;
    public float spawnRate;
    public float radius;
    public float spawnAmount;
    public GameObject enemyPrefab;

    [HideInInspector] public float nextSpawnTime;

    [HideInInspector] public Random random;

}

public class EnemySpawnerBaker : Baker <EnemySpawnerAuthoring>
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
                         spawnAmount = authoring.spawnAmount,
                         random = Random.CreateFromIndex((uint)UnityEngine.Random.Range(0, 9999)),
                         enemyPrefab = GetEntity(authoring.enemyPrefab,
                                                 TransformUsageFlags.Dynamic)
                     });
    }

}