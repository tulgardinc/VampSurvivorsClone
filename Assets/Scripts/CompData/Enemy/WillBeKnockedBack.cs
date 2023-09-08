using Unity.Entities;
using Unity.Mathematics;

public struct WillBeKnockedBack : IComponentData
{

    public float totalKnockbackAmount;
    public float3 knockbackDirection;

}