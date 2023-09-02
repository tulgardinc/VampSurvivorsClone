using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class BulletCollisionDataAuthoring : MonoBehaviour
{
    public FixedString4096Bytes message;
}

public class BulletCollisionDataBaker : Baker<BulletCollisionDataAuthoring>
{
    public override void Bake (BulletCollisionDataAuthoring authoring)
    {
        AddComponent(GetEntity(TransformUsageFlags.Dynamic), new BulletCollisionData { message = authoring.message });
    }
}