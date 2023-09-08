using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ShootAtMousePositionTagAuthoring : MonoBehaviour
{
}

public class ShootAtMousePositionTagBaker : Baker<ShootAtMousePositionTagAuthoring>
{
    public override void Bake(ShootAtMousePositionTagAuthoring authoring)
    {
        var e = GetEntity(TransformUsageFlags.None);
        AddComponent(e, new ShootAtMousePositionTag
        {
        });
    }
}