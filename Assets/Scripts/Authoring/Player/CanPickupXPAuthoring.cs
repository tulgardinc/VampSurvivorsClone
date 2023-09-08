using Unity.Entities;
using UnityEngine;

public class CanPickupXPAuthoring : MonoBehaviour
{
    public float pickupRange;
}

public class CanPickupRangeBaker : Baker<CanPickupXPAuthoring>
{
    public override void Bake(CanPickupXPAuthoring authoring)
    {
        AddComponent(GetEntity(TransformUsageFlags.Dynamic), new CanPickupXP
        {
            pickupRange = authoring.pickupRange
        });
    }
}
