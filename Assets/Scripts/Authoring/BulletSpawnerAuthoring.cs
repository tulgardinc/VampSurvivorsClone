using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BulletSpawnerAuthoring : MonoBehaviour
{
    public float fireInterval;
    [HideInInspector]
    public float fireTimer;
    [HideInInspector]
    public float3 direction;

    public GameObject bullet;
}

public class BulletSpawnerBaker : Baker<BulletSpawnerAuthoring>
{
    public override void Bake (BulletSpawnerAuthoring authoring)
    {
        AddComponent(this.GetEntity(TransformUsageFlags.Dynamic), new BulletSpawner { fireTimer = authoring.fireTimer, fireInterval = authoring.fireInterval, bullet = this.GetEntity(authoring.bullet, TransformUsageFlags.Dynamic) });
    }
}