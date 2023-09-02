using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEditor;
using UnityEngine;


[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
public partial struct CollisionCleanupSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
    }

    public void OnUpdate(ref SystemState state)
    {

        NativeList<Entity> collisionList = new NativeList<Entity>(AllocatorManager.TempJob);

        state.Dependency = new CollisionCleanupJob
        {
            bulletLookup = SystemAPI.GetComponentLookup<BulletTag>(true),
            enemyLookup = SystemAPI.GetComponentLookup<EnemyTag>(true),
            collisionList = collisionList
        }.Schedule(
            SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

        state.Dependency.Complete();

        if (state.Dependency.IsCompleted)
        {
            foreach (var inCollisionWith in SystemAPI.Query<DynamicBuffer<InCollisionWith>>())
            {
                for (int i = inCollisionWith.Length - 1; i >= 0; i--)
                {
                    Entity entity = inCollisionWith[i];
                    bool entityIsPresent = false;
                    foreach (Entity entityInCollision in collisionList)
                    {
                        if (entityInCollision.Equals(entity))
                        {
                            entityIsPresent = true;
                            break;
                        }
                    }

                    if (!entityIsPresent)
                    {
                        inCollisionWith.RemoveAt(i);
                    }
                }
            }
        }

    }

    private struct CollisionCleanupJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<BulletTag> bulletLookup;
        [ReadOnly] public ComponentLookup<EnemyTag> enemyLookup;
        public NativeList<Entity> collisionList;

        bool IsBullet(Entity entity) => bulletLookup.HasComponent(entity);
        bool IsEnemy(Entity entity) => enemyLookup.HasComponent(entity);

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity enemy = triggerEvent.EntityA;
            Entity bullet = triggerEvent.EntityB;

            if (IsEnemy(enemy) && IsBullet(bullet))
            {
                collisionList.Add(enemy);
            }
        }
    }

}
