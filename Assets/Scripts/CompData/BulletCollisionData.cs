using Unity.Collections;
using Unity.Entities;

public struct BulletCollisionData : IComponentData
{
    public FixedString4096Bytes message;
}
