using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial struct XPOrbSpawnMotionSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate <OrbSpawner>();
        state.RequireForUpdate <EndSimulationEntityCommandBufferSystem.Singleton>();
    }


    public void OnUpdate(ref SystemState state)
    {
        var ecbSystem = SystemAPI.GetSingleton <EndSimulationEntityCommandBufferSystem.Singleton>();
        var deceleration = SystemAPI.GetSingleton <OrbSpawner>().orbSpawnDeceleration;
        var ecb = ecbSystem.CreateCommandBuffer(state.World.Unmanaged);
        var parallelEcb = ecb.AsParallelWriter();

        state.Dependency = new XPOrbSpawnMotionJob
        {
            ecb = parallelEcb,
            timeDelta = Time.deltaTime,
            deceleration = deceleration
        }.ScheduleParallel(state.Dependency);
    }


    public partial struct XPOrbSpawnMotionJob : IJobEntity
    {

        public EntityCommandBuffer.ParallelWriter ecb;
        public float timeDelta;
        public float deceleration;

        private void Execute([ChunkIndexInQuery] int sortKey, ref LocalTransform transform,
                             ref XPOrbHasStartMotion xpOrbHasStartMotion, Entity entity)
        {
            if (xpOrbHasStartMotion.currentSpeed <= 0)
            {
                ecb.RemoveComponent <XPOrbHasStartMotion>(sortKey, entity);
            }
            else
            {
                transform.Position += xpOrbHasStartMotion.direction * xpOrbHasStartMotion.currentSpeed * timeDelta;
                xpOrbHasStartMotion.currentSpeed -= deceleration * timeDelta;
            }
        }

    }

}