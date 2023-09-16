using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct RocketLauncherWeaponSystem : ISystem
{

    public void OnEnable(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    public void OnUpdate(ref SystemState state)
    {

        var ecbSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        foreach (var (playerTransform, playerDirection, _) in SystemAPI
                         .Query<RefRO<LocalTransform>, RefRO<Direction>, RefRO<PlayerTag>>())

        {
            foreach (var (rocketLauncher, cooldown) in SystemAPI.Query<RefRW<RocketLauncherWeapon>, RefRW<Cooldown>>())
            {
                if (cooldown.ValueRO.timer <= 0)
                {
                    var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

                    float3 mouseDirection = math.normalizesafe(new float3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                                                          Camera.main.ScreenToWorldPoint(Input.mousePosition).y,
                                                          playerTransform.ValueRO.Position.z) - playerTransform.ValueRO.Position);

                    uint seed = rocketLauncher.ValueRW.random.NextUInt(0, 999);

                    state.Dependency = new RocketLauncherJob
                    {
                        playerPos = playerTransform.ValueRO.Position,
                        maxAngle = rocketLauncher.ValueRO.spreadRange,
                        mouseDirection = mouseDirection,
                        random = rocketLauncher.ValueRW.random,
                        projectile = rocketLauncher.ValueRO.projectile,
                        seed = seed,
                        ecb = ecb
                    }.Schedule(rocketLauncher.ValueRO.rocketCount, 64);

                    cooldown.ValueRW.timer = cooldown.ValueRO.cooldownTime;
                }
            }
        }
    }

    public partial struct RocketLauncherJob : IJobParallelFor
    {
        [ReadOnly] public float3 playerPos;
        [ReadOnly] public float maxAngle;
        [ReadOnly] public Entity projectile;
        [ReadOnly] public float3 mouseDirection;
        [ReadOnly] public uint seed;

        public Unity.Mathematics.Random random;
        public EntityCommandBuffer.ParallelWriter ecb;

        public void Execute(int index)
        {
            var rocket = ecb.Instantiate(index, projectile);

            var random = Unity.Mathematics.Random.CreateFromIndex((uint)(seed * index));

            float rocketRandomAngle = random.NextFloat(-maxAngle, maxAngle);

            float3 rocketDirection = math.mul(quaternion.AxisAngle(new float3(0, 0, 1), math.radians(rocketRandomAngle)), mouseDirection);

            float3 lookDirection = math.mul(quaternion.Euler(0, 0, 90), rocketDirection);

            ecb.SetComponent(index, rocket, LocalTransform.FromPositionRotation(playerPos, quaternion.LookRotationSafe(new float3(0, 0.0f, 1.0f), lookDirection)));
            ecb.AddComponent(index, rocket, new RocketRandomShift { randomShift = random.NextFloat(0, 359) });
            ecb.AddComponent(index, rocket, new Speed { speed = random.NextFloat(10, 15) });
            ecb.SetComponent(index, rocket, new Direction { direction = rocketDirection });
        }
    }

}
