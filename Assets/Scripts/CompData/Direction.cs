using Unity.Entities;
using Unity.Mathematics;

public struct Direction : IComponentData
{
    public float3 direction;
    public float3 previousDirection;
}
