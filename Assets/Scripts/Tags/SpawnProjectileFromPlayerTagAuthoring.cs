using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpawnProjectileFromPlayerTagAuthoring : MonoBehaviour
{

}

public class BakeSpawnProjectileFromPlayerTag : Baker<SpawnProjectileFromPlayerTagAuthoring>
{
    public override void Bake(SpawnProjectileFromPlayerTagAuthoring authoring)
    {
        AddComponent(GetEntity(TransformUsageFlags.None), new SpawnProjectileFromPlayerTag { });
    }
}
