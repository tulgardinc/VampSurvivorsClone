using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PhysicsSimulationGroup))]
public partial struct BulletCollision : ISystem
{
    public void OnCreate (ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
    }

    public void OnUpdate (ref SystemState state)
    {
        state.Dependency = new CollisionJob().Schedule(
            SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }

    private struct CollisionJob : ICollisionEventsJob
    {
        public void Execute (CollisionEvent collisionEvent)
        {
            Debug.Log($"A: {collisionEvent.EntityA}, B: {collisionEvent.EntityB}");
        }
    }
}
