using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
public partial struct CollisionCleanupSystem : ISystem
{

    [BurstCompile] public void OnCreate(ref SystemState state) { state.RequireForUpdate <SimulationSingleton>(); }

    public void OnUpdate(ref SystemState state)
    {
        var collisionList = new NativeList <Entity>(AllocatorManager.TempJob);

        state.Dependency = new CollisionCleanupJob
        {
            bulletLookup = SystemAPI.GetComponentLookup <BulletTag>(true),
            enemyLookup = SystemAPI.GetComponentLookup <EnemyTag>(true),
            collisionList = collisionList
        }.Schedule(
                   SystemAPI.GetSingleton <SimulationSingleton>(), state.Dependency);

        state.Dependency.Complete();

        if (state.Dependency.IsCompleted)
        {
            foreach (var inCollisionWith in SystemAPI.Query <DynamicBuffer <InCollisionWith>>())
            {
                for (var i = inCollisionWith.Length - 1; i >= 0; i--)
                {
                    Entity entity = inCollisionWith[i];
                    var entityIsPresent = false;
                    foreach (var entityInCollision in collisionList)
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

        [ReadOnly] public ComponentLookup <BulletTag> bulletLookup;
        [ReadOnly] public ComponentLookup <EnemyTag> enemyLookup;
        public NativeList <Entity> collisionList;

        private bool IsBullet(Entity entity) { return bulletLookup.HasComponent(entity); }

        private bool IsEnemy(Entity entity) { return enemyLookup.HasComponent(entity); }

        public void Execute(TriggerEvent triggerEvent)
        {
            var enemy = triggerEvent.EntityA;
            var bullet = triggerEvent.EntityB;

            if (IsEnemy(enemy) && IsBullet(bullet))
            {
                collisionList.Add(enemy);
            }
        }

    }

}