

using Unity.Entities;
using Unity.Physics;
using UnityEngine;

public partial struct AspectTestSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var ent in SystemAPI.Query<TestAspect>())
        {
            Debug.Log(ent.self);
        }
    }
}