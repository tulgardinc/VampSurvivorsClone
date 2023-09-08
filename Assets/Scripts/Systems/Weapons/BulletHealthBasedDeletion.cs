using System.Runtime.InteropServices;
using Unity.Entities;

[StructLayout(LayoutKind.Auto)]
public partial struct BulletHealthBasedDeletion : ISystem
{

    private EntityQuery bulletQuery;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate <EndSimulationEntityCommandBufferSystem.Singleton>();

        bulletQuery = state.GetEntityQuery(ComponentType.ReadWrite <BulletHealth>());
    }


    public void OnUpdate(ref SystemState state)
    {
        var ecbSystem = SystemAPI.GetSingleton <EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);
        var parallelEcb = ecb.AsParallelWriter();

        var bulletHealthBasedDeletionJob = new BulletHealthBasedDeletionJob
        {
            ecb = parallelEcb
        }.ScheduleParallel(bulletQuery, state.Dependency);

        bulletHealthBasedDeletionJob.Complete();
    }

    [StructLayout(LayoutKind.Auto)]
    private partial struct BulletHealthBasedDeletionJob : IJobEntity
    {

        public EntityCommandBuffer.ParallelWriter ecb;


        private void Execute(ref BulletHealth bulletHealth, Entity entity, [ChunkIndexInQuery] int sortKey)
        {
            if (bulletHealth.bulletHealth <= 0)
            {
                ecb.DestroyEntity(sortKey, entity);
            }
        }

    }

}