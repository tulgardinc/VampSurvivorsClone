using Unity.Entities;
using UnityEngine;

public class EnemyTagAuthoring : MonoBehaviour
{
}

public class EnemyTagBaker : Baker<EnemyTagAuthoring>
{
    public override void Bake (EnemyTagAuthoring authoring)
    {
        AddComponent(this.GetEntity(TransformUsageFlags.Dynamic), new EnemyTag { });
    }
}