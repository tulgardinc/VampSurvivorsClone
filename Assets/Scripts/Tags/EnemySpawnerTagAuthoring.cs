using Unity.Entities;
using UnityEngine;

public class EnemySpawnerTagAuthoring : MonoBehaviour
{
}

public class EnemySpawnerTagBaker : Baker<EnemySpawnerTagAuthoring>
{
    public override void Bake (EnemySpawnerTagAuthoring authoring)
    {
        AddComponent(this.GetEntity(TransformUsageFlags.None), new EnemySpawnerTag { });
    }
}