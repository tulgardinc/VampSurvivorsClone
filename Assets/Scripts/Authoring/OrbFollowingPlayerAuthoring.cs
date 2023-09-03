
using Unity.Entities;
using UnityEngine;

public class OrbFollowingPlayerAuthoring : MonoBehaviour
{
}

public class OrbFollowingPlayerBaker : Baker<OrbFollowingPlayerAuthoring>
{
    public override void Bake(OrbFollowingPlayerAuthoring authoring)
    {
        AddComponent(GetEntity(TransformUsageFlags.Dynamic), new OrbFollowingPlayer
        {
        });
    }
}

