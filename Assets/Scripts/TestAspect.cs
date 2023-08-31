using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct TestAspect : IAspect
{
    public readonly Entity _entity;
    public readonly RefRO<LocalTransform> _localTransform;
}
