using System.Runtime.InteropServices;
using Unity.Entities;
using UnityEngine;

public partial struct LifeTimeSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate <EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecbSystem = SystemAPI.GetSingleton <EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSystem.CreateCommandBuffer(state.World.Unmanaged);
        var parallelEcb = ecb.AsParallelWriter();

        state.Dependency = new LifeTimeJob
        {
            ecb = parallelEcb,
            timeDelta = Time.deltaTime
        }.ScheduleParallel(state.Dependency);
    }


    [StructLayout(LayoutKind.Auto)]
    public partial struct LifeTimeJob : IJobEntity
    {

        public EntityCommandBuffer.ParallelWriter ecb;
        public float timeDelta;

        public void Execute([ChunkIndexInQuery] int sortKey, ref LifeTime lifeTime, Entity entity)
        {
            if (lifeTime.lifeTimeLeft <= 0)
            {
                ecb.DestroyEntity(sortKey, entity);
            }
            else
            {
                lifeTime.lifeTimeLeft -= timeDelta;
            }
        }

    }

}