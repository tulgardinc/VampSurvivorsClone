using Unity.Entities;
using Unity.Mathematics;

public struct BulletSpawner : IComponentData
{
    public float fireInterval;
    public float fireTimer;
    public float3 direction;

    public Entity bullet;
}
