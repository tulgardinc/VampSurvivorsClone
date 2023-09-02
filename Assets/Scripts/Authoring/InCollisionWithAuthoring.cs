using Unity.Entities;
using UnityEngine;

public class InCollisionWithAuthoring : MonoBehaviour
{

}

public class InCollisionWithBaker : Baker<InCollisionWithAuthoring>
{
    public override void Bake(InCollisionWithAuthoring authoring)
    {
        AddComponent(GetEntity(TransformUsageFlags.Dynamic), new InCollisoinWith
        {
            entites = { }
        });
    }
}