using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class DirectionAuthoring : MonoBehaviour
{
    public float3 direction;
    public float3 previousDirection;
}

public class BaseBulletBaker : Baker<DirectionAuthoring>
{
    public override void Bake(DirectionAuthoring authoring)
    {
        AddComponent(GetEntity(TransformUsageFlags.Dynamic), new Direction
        {
            direction = default,
            previousDirection = default
        });
    }
}