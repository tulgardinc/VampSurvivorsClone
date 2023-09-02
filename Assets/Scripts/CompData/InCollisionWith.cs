
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

[InternalBufferCapacity(8)]
public struct InCollisionWith : IBufferElementData
{
    // These implicit conversions are optional, but can help reduce typing.
    public static implicit operator Entity(InCollisionWith e) { return e.Value; }
    public static implicit operator InCollisionWith(Entity e) { return new InCollisionWith { Value = e }; }

    // Actual value each buffer element will store.
    public Entity Value;
}
