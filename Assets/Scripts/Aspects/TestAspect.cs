using Unity.Entities;
using UnityEngine;

public readonly partial struct TestAspect : IAspect
{
    readonly public Entity self;
    readonly public RefRW<Direction> Direction;
    readonly public RefRW<Speed> Speed;
}
