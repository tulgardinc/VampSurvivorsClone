using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public partial struct ZweihanderWeaponSystem : ISystem
{

    public void OnCreate(ref SystemState state) { state.RequireForUpdate <PhysicsWorldSingleton>(); }

    public void OnStart(ref SystemState state)
    {
        state.RequireForUpdate <PhysicsWorld>();
        state.RequireForUpdate <EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var physicsWorld = SystemAPI.GetSingleton <PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorld.CollisionWorld;
        var ecbSystem = SystemAPI.GetSingleton <EndSimulationEntityCommandBufferSystem.Singleton>();


        foreach (var (playerTransform, playerDirection, _) in SystemAPI
                         .Query <RefRO <LocalTransform>, RefRO <Direction>, RefRO <PlayerTag>>())

        {
            foreach (var (zweihander, cooldown) in SystemAPI.Query <RefRO <ZweihanderWeapon>, RefRW <Cooldown>>())
            {
                if (cooldown.ValueRO.timer <= 0)
                {
                    float3 attackDirection =
                            math.normalizesafe(new float3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                                                          Camera.main.ScreenToWorldPoint(Input.mousePosition).y,
                                                          playerTransform.ValueRO.Position.z) -
                                               playerTransform.ValueRO.Position);

                    var enemiesToHit = new NativeList <DistanceHit>(Allocator.TempJob);
                    collisionWorld.OverlapSphere(playerTransform.ValueRO.Position, zweihander.ValueRO.radius,
                                                 ref enemiesToHit, CollisionFilter.Default);

                    EntityCommandBuffer.ParallelWriter ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged)
                                                                      .AsParallelWriter();

                    var enemyLookup = SystemAPI.GetComponentLookup <EnemyTag>();

                    state.Dependency = new ZweihanderDamageJob
                    {
                        enemyLookup = enemyLookup,
                        attackDirection = attackDirection,
                        enemiesToHit = enemiesToHit,
                        ecb = ecb,
                        sweepingAngle = zweihander.ValueRO.sweepingAngle,
                        damage = zweihander.ValueRO.damage,
                        attackOrigin = playerTransform.ValueRO.Position - attackDirection * 0.5f,
                        knockbackAmount = zweihander.ValueRO.knockbackAmount
                    }.Schedule(enemiesToHit.Length, 64);


                    cooldown.ValueRW.timer = cooldown.ValueRO.cooldownTime;
                }
            }
        }
    }

    [BurstCompile]
    public struct ZweihanderDamageJob : IJobParallelFor
    {

        [ReadOnly] public ComponentLookup <EnemyTag> enemyLookup;
        [ReadOnly] public float3 attackDirection;
        [ReadOnly] public NativeList <DistanceHit> enemiesToHit;
        [ReadOnly] public float sweepingAngle;
        [ReadOnly] public float damage;
        [ReadOnly] public float3 attackOrigin;
        [ReadOnly] public float knockbackAmount;

        public EntityCommandBuffer.ParallelWriter ecb;

        public void Execute(int index)
        {
            DistanceHit hit = enemiesToHit[index];
            float3 hitDirection = math.normalizesafe(hit.Position - attackOrigin);
            if (math.acos(math.dot(hitDirection, attackDirection)) <= sweepingAngle * (math.PI / 180))
            {
                if (enemyLookup.HasComponent(hit.Entity))
                {
                    EntityUtilities.EnemyHitWithEffectsParallel(index, ecb, hit.Entity, damage, knockbackAmount,
                                                                hitDirection);
                }
            }
        }

    }

}