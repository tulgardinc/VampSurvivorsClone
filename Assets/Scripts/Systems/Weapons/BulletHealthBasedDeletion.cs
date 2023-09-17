using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[StructLayout(LayoutKind.Auto)]
public partial struct BulletHealthBasedDeletion : ISystem
{

    private EntityQuery bulletQuery;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate <ExplosionController>();
        state.RequireForUpdate <EndSimulationEntityCommandBufferSystem.Singleton>();

        bulletQuery = state.GetEntityQuery(ComponentType.ReadWrite <BulletHealth>(),
                                           ComponentType.ReadOnly <LocalTransform>());
    }


    public void OnUpdate(ref SystemState state)
    {
        var ecbSystem = SystemAPI.GetSingleton <EndSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);
        EntityCommandBuffer.ParallelWriter parallelEcb = ecb.AsParallelWriter();

        var explosionController =
                SystemAPI.GetComponent <ExplosionController>(SystemAPI.GetSingletonEntity <ExplosionController>());

        JobHandle bulletHealthBasedDeletionJob = new BulletHealthBasedDeletionJob
        {
            ecb = parallelEcb,
            explodeOnImpactLookup = SystemAPI.GetComponentLookup <ExplodeOnImpact>(true),
            explosionDataLookup = SystemAPI.GetComponentLookup <ExplosionData>(),
            explosionController = explosionController
        }.ScheduleParallel(bulletQuery, state.Dependency);

        bulletHealthBasedDeletionJob.Complete();
    }

    [StructLayout(LayoutKind.Auto)]
    private partial struct BulletHealthBasedDeletionJob : IJobEntity
    {

        public EntityCommandBuffer.ParallelWriter ecb;
        [ReadOnly] public ComponentLookup <ExplodeOnImpact> explodeOnImpactLookup;
        [ReadOnly] public ComponentLookup <ExplosionData> explosionDataLookup;

        public ExplosionController explosionController;

        private void Execute(ref BulletHealth bulletHealth, ref LocalTransform bulletTransform, Entity bullet,
                             [ChunkIndexInQuery] int sortKey)
        {
            if (bulletHealth.bulletHealth <= 0)
            {
                if (CanExplode(bullet))
                {
                    Entity explosionEntity = ecb.Instantiate(sortKey, explosionController.explosionPrefab);

                    ecb.SetComponent(sortKey, explosionEntity, LocalTransform.FromPosition(bulletTransform.Position));
                    ecb.AddComponent(sortKey, explosionEntity, Explosion.FromExplosionData(GetExplosionData(bullet)));
                }


                ecb.DestroyEntity(sortKey, bullet);
            }
        }

        private bool CanExplode(Entity entity) { return explodeOnImpactLookup.HasComponent(entity); }

        private ExplosionData GetExplosionData(Entity entity) { return explosionDataLookup[entity]; }

    }

}