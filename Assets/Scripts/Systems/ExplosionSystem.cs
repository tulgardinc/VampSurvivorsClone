using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

public partial struct ExplosionSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate <EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate <PhysicsWorldSingleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var physicsWorld = SystemAPI.GetSingleton <PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorld.CollisionWorld;
        var ecbSystem = SystemAPI.GetSingleton <EndSimulationEntityCommandBufferSystem.Singleton>();

        foreach ((var explosionTransform, var explosion, Entity explosionEntity) in
                 SystemAPI.Query <RefRO <LocalTransform>, RefRO <Explosion>>().WithEntityAccess())
        {
            if (explosion.ValueRO.explosionTimer <= 0)
            {
                var enemiesToHit = new NativeList <DistanceHit>(Allocator.TempJob);
                collisionWorld.OverlapSphere(explosionTransform.ValueRO.Position, explosion.ValueRO.explosionRange,
                                             ref enemiesToHit, CollisionFilter.Default);

                EntityCommandBuffer.ParallelWriter ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged)
                                                                  .AsParallelWriter();


                state.Dependency = new ExplosionJob
                {
                    ecb = ecb,
                    enemiesToHit = enemiesToHit,
                    enemyLookup = SystemAPI.GetComponentLookup <EnemyTag>(),
                    transformLookup = SystemAPI.GetComponentLookup <LocalTransform>(),
                    explosionEntity = explosionEntity,
                    explosionDamage = explosion.ValueRO.explosionDamage,
                    explosionKnockback = explosion.ValueRO.explosionKnockback,
                    explosionPosition = explosionTransform.ValueRO.Position
                }.Schedule(enemiesToHit.Length, 64);
            }
        }
    }


    private struct ExplosionJob : IJobParallelFor
    {

        public EntityCommandBuffer.ParallelWriter ecb;
        [ReadOnly] public NativeList <DistanceHit> enemiesToHit;
        [ReadOnly] public ComponentLookup <EnemyTag> enemyLookup;
        [ReadOnly] public ComponentLookup <LocalTransform> transformLookup;
        [ReadOnly] public float explosionDamage;
        [ReadOnly] public float explosionKnockback;
        [ReadOnly] public float3 explosionPosition;
        public Entity explosionEntity;

        public void Execute(int index)
        {
            DistanceHit enemyHit = enemiesToHit[index];

            if (enemyLookup.HasComponent(enemyHit.Entity))
            {
                float3 knockbackDirection = GetTransform(enemyHit.Entity).ValueRO.Position - explosionPosition;

                EntityUtilities.EnemyHitWithEffectsParallel(index, ecb, enemyHit.Entity, explosionDamage,
                                                            explosionKnockback, knockbackDirection);
            }

            ecb.DestroyEntity(index, explosionEntity);
        }

        private RefRO <LocalTransform> GetTransform(Entity entity) { return transformLookup.GetRefRO(entity); }

    }

}